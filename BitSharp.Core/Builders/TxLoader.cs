﻿using BitSharp.Core.Domain;
using BitSharp.Core.Storage;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace BitSharp.Core.Builders
{
    internal static class TxLoader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static ISourceBlock<LoadedTx> LoadTxes(ICoreStorage coreStorage, ISourceBlock<LoadingTx> loadingTxes, CancellationToken cancelToken = default(CancellationToken))
        {
            // split incoming LoadingTx by its number of inputs
            var createTxInputList = InitCreateTxInputList(cancelToken);

            // link the loading txes to the input splitter
            loadingTxes.LinkTo(createTxInputList, new DataflowLinkOptions { PropagateCompletion = true });

            // load each input, and return and fully loaded txes
            var loadTxInputAndReturnLoadedTx = InitLoadTxInputAndReturnLoadedTx(coreStorage, cancelToken);

            // link the input splitter to the input loader
            createTxInputList.LinkTo(loadTxInputAndReturnLoadedTx, new DataflowLinkOptions { PropagateCompletion = true });

            return loadTxInputAndReturnLoadedTx;
        }

        private static TransformManyBlock<LoadingTx, Tuple<LoadingTx, int>> InitCreateTxInputList(CancellationToken cancelToken)
        {
            return new TransformManyBlock<LoadingTx, Tuple<LoadingTx, int>>(
                loadingTx =>
                {
                    // queue up inputs to be loaded for non-coinbase transactions
                    if (!loadingTx.IsCoinbase)
                    {
                        return Enumerable.Range(0, loadingTx.PrevOutputTxKeys.Length).Select(
                            inputIndex => Tuple.Create(loadingTx, inputIndex));
                    }
                    // queue coinbase transaction with no inputs to load
                    else
                    {
                        return new[] { Tuple.Create(loadingTx, -1) };
                    }
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancelToken, SingleProducerConstrained = true });
        }

        private static TransformManyBlock<Tuple<LoadingTx, int>, LoadedTx> InitLoadTxInputAndReturnLoadedTx(ICoreStorage coreStorage, CancellationToken cancelToken)
        {
            var txCache = new ConcurrentDictionary<TxLookupKey, Transaction>();
            return new TransformManyBlock<Tuple<LoadingTx, int>, LoadedTx>(
                tuple =>
                {
                    var loadingTx = tuple.Item1;
                    var inputIndex = tuple.Item2;

                    var loadedTx = LoadTxInput(coreStorage, txCache, loadingTx, inputIndex);
                    if (loadedTx != null)
                        return new[] { loadedTx };
                    else
                        return new LoadedTx[0];
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancelToken, MaxDegreeOfParallelism = 16, SingleProducerConstrained = true });
        }

        private static LoadedTx LoadTxInput(ICoreStorage coreStorage, ConcurrentDictionary<TxLookupKey, Transaction> txCache, LoadingTx loadingTx, int inputIndex)
        {
            var txIndex = loadingTx.TxIndex;
            var transaction = loadingTx.Transaction;
            var chainedHeader = loadingTx.ChainedHeader;

            if (!loadingTx.IsCoinbase)
            {
                var prevOutputTxKey = loadingTx.PrevOutputTxKeys[inputIndex];

                // load previous transactions for each input, unless this is a coinbase transaction
                var input = transaction.Inputs[inputIndex];
                var inputPrevTxHash = input.PreviousTxOutputKey.TxHash;

                Transaction inputPrevTx;
                if (txCache.TryGetValue(prevOutputTxKey, out inputPrevTx))
                {
                    if (input.PreviousTxOutputKey.TxHash != inputPrevTx.Hash)
                        throw new Exception("TODO");
                }
                else if (coreStorage.TryGetTransaction(prevOutputTxKey.BlockHash, prevOutputTxKey.TxIndex, out inputPrevTx))
                {
                    if (input.PreviousTxOutputKey.TxHash != inputPrevTx.Hash)
                        throw new Exception("TODO");

                    txCache[prevOutputTxKey] = inputPrevTx;
                }
                else
                    throw new Exception("TODO");

                if (loadingTx.InputTxes.TryComplete(inputIndex, inputPrevTx))
                    return loadingTx.ToLoadedTx();
                else
                    return null;
            }
            else
            {
                Debug.Assert(inputIndex == -1);
                return new LoadedTx(transaction, txIndex, ImmutableArray.Create<Transaction>());
            }
        }
    }
}
