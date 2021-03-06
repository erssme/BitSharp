﻿using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace BitSharp.Core.Builders
{
    public class DeferredDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<TKey, TValue> read = new Dictionary<TKey, TValue>();
        private readonly HashSet<TKey> missing = new HashSet<TKey>();
        private readonly Dictionary<TKey, TValue> updated = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, TValue> added = new Dictionary<TKey, TValue>();
        private readonly HashSet<TKey> deleted = new HashSet<TKey>();

        private readonly Func<TKey, Tuple<bool, TValue>> parentTryGetValue;
        private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> parentEnumerator;

        private ConcurrentDictionary<TKey, Lazy<Tuple<bool, TValue>>> parentValues = new ConcurrentDictionary<TKey, Lazy<Tuple<bool, TValue>>>();

        public DeferredDictionary(Func<TKey, Tuple<bool, TValue>> parentTryGetValue, Func<IEnumerable<KeyValuePair<TKey, TValue>>> parentEnumerator = null)
        {
            this.parentTryGetValue = parentTryGetValue;
            this.parentEnumerator = parentEnumerator;
        }

        public IDictionary<TKey, TValue> Updated => updated;

        public IDictionary<TKey, TValue> Added => added;

        public ISet<TKey> Deleted => deleted;

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                    return value;
                else
                    throw new KeyNotFoundException();
            }
            set
            {
                AddOrUpdate(key, value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (!missing.Contains(key) && !deleted.Contains(key))
            {
                if (read.ContainsKey(key) || updated.ContainsKey(key) || added.ContainsKey(key))
                    return true;

                TValue value;
                if (TryGetParentValue(key, out value))
                {
                    read.Add(key, value);
                    return true;
                }
                else
                {
                    missing.Add(key);
                    return false;
                }
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!missing.Contains(key) && !deleted.Contains(key))
            {
                if (read.TryGetValue(key, out value) || updated.TryGetValue(key, out value) || added.TryGetValue(key, out value))
                    return true;

                if (TryGetParentValue(key, out value))
                {
                    read.Add(key, value);
                    return true;
                }
                else
                {
                    missing.Add(key);
                    return false;
                }
            }

            value = default(TValue);
            return false;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (missing.Contains(key))
            {
                missing.Remove(key);
                added.Add(key, value);
                return true;
            }
            else if (deleted.Contains(key))
            {
                deleted.Remove(key);
                updated.Add(key, value);
                return true;
            }
            else if (read.ContainsKey(key))
            {
                return false;
            }
            else if (!added.ContainsKey(key) && !updated.ContainsKey(key))
            {
                TValue existingValue;
                if (!TryGetParentValue(key, out existingValue))
                {
                    added.Add(key, value);
                    return true;
                }
                else
                {
                    read.Add(key, existingValue);
                    return false;
                }
            }
            else
                return false;
        }

        public bool TryRemove(TKey key)
        {
            TValue ignore;

            if (missing.Contains(key) || deleted.Contains(key))
            {
                return false;
            }
            else if (read.ContainsKey(key) || updated.ContainsKey(key) || added.ContainsKey(key) || TryGetParentValue(key, out ignore))
            {
                deleted.Add(key);
                read.Remove(key);
                updated.Remove(key);
                added.Remove(key);
                return true;
            }
            else
                return false;
        }

        public void Remove(TKey key)
        {
            deleted.Add(key);
            read.Remove(key);
            updated.Remove(key);
            added.Remove(key);
        }

        public bool TryUpdate(TKey key, TValue value)
        {
            TValue ignore;

            if (missing.Contains(key) || deleted.Contains(key))
            {
                return false;
            }
            else if (read.ContainsKey(key))
            {
                Debug.Assert(!updated.ContainsKey(key));
                Debug.Assert(!added.ContainsKey(key));

                updated.Add(key, value);
                read.Remove(key);
                return true;
            }
            else if (updated.ContainsKey(key))
            {
                Debug.Assert(!read.ContainsKey(key));
                Debug.Assert(!added.ContainsKey(key));

                updated[key] = value;
                return true;
            }
            else if (added.ContainsKey(key))
            {
                Debug.Assert(!read.ContainsKey(key));
                Debug.Assert(!updated.ContainsKey(key));

                added[key] = value;
                return true;
            }
            else if (TryGetParentValue(key, out ignore))
            {
                updated[key] = value;
                return true;
            }
            else
                return false;
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                if (!TryAdd(key, value))
                    throw new InvalidOperationException();
            }
            else
            {
                if (!TryUpdate(key, value))
                    throw new InvalidOperationException();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (parentEnumerator == null)
                throw new NotSupportedException();

            foreach (var kvPair in parentEnumerator())
            {
                Debug.Assert(!missing.Contains(kvPair.Key));
                Debug.Assert(!added.ContainsKey(kvPair.Key));

                TValue currentValue;
                if (deleted.Contains(kvPair.Key))
                {
                    continue;
                }
                else if (updated.TryGetValue(kvPair.Key, out currentValue))
                {
                    yield return new KeyValuePair<TKey, TValue>(kvPair.Key, currentValue);
                }
                else
                {
                    yield return kvPair;
                }
            }

            foreach (var kvPair in added)
                yield return kvPair;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void WarmupValue(TKey key)
        {
            var lazyValue = parentValues.GetOrAdd(key, _ => new Lazy<Tuple<bool, TValue>>(() => parentTryGetValue(key)));
            lazyValue.Force();
        }

        private bool TryGetParentValue(TKey key, out TValue value)
        {
            Lazy<Tuple<bool, TValue>> lazyValue;
            if (parentValues.TryGetValue(key, out lazyValue))
            {
                value = lazyValue.Value.Item2;
                return lazyValue.Value.Item1;
            }
            else
            {
                var result = parentTryGetValue(key);
                if (result.Item1)
                    value = result.Item2;
                else
                    value = default(TValue);

                var valueLocal = value;
                parentValues.TryAdd(key, new Lazy<Tuple<bool, TValue>>(() => result));

                return result.Item1;
            }
        }
    }
}
