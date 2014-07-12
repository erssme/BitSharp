﻿using BitSharp.Common;
using BitSharp.Core.Domain;
using BitSharp.Core.Storage;
using BitSharp.Core.Storage.Memory;
using BitSharp.Core.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitSharp.Core.Test.Workers
{
    [TestClass]
    public class TargetBlockWorkerTest
    {
        [TestMethod]
        public void TestSimpleTargetBlock()
        {
            // prepare test kernel
            var kernel = new StandardKernel(new ConsoleLoggingModule(), new MemoryStorageModule());
            kernel.Bind<CoreStorage>().ToSelf().InSingletonScope();
            var coreStorage = kernel.Get<CoreStorage>();

            // initialize data
            var fakeHeaders = new FakeHeaders();
            var chainedHeader0 = fakeHeaders.GenesisChained();
            var chainedHeader1 = fakeHeaders.NextChained();

            // initialize the target block watcher
            using (var targetBlockWorker = kernel.Get<TargetBlockWorker>(new ConstructorArgument("workerConfig", new WorkerConfig(initialNotify: false, minIdleTime: TimeSpan.Zero, maxIdleTime: TimeSpan.MaxValue))))
            {
                // monitor event firing
                var workNotifyEvent = new AutoResetEvent(false);
                var workStoppedEvent = new AutoResetEvent(false);
                var onTargetBlockChangedCount = 0;

                targetBlockWorker.OnNotifyWork += () => workNotifyEvent.Set();
                targetBlockWorker.OnWorkStopped += () => workStoppedEvent.Set();
                targetBlockWorker.TargetBlockChanged += () => onTargetBlockChangedCount++;

                // start worker and wait for intial target
                targetBlockWorker.Start();
                workNotifyEvent.WaitOne();
                workStoppedEvent.WaitOne();

                // verify initial state
                Assert.IsNull(targetBlockWorker.TargetBlock);

                // add block 0
                coreStorage.AddGenesisBlock(chainedHeader0);

                // wait for worker
                workNotifyEvent.WaitOne();
                workStoppedEvent.WaitOne();

                // verify block 0
                Assert.AreEqual(chainedHeader0, targetBlockWorker.TargetBlock);
                Assert.AreEqual(1, onTargetBlockChangedCount);

                // add block 1
                coreStorage.TryChainHeader(chainedHeader1.BlockHeader, out chainedHeader1);

                // wait for worker
                workNotifyEvent.WaitOne();
                workStoppedEvent.WaitOne();

                // verify block 1
                Assert.AreEqual(chainedHeader1, targetBlockWorker.TargetBlock);
                Assert.AreEqual(2, onTargetBlockChangedCount);

                // verify no other work was done
                Assert.IsFalse(workNotifyEvent.WaitOne(0));
                Assert.IsFalse(workStoppedEvent.WaitOne(0));
            }
        }
    }
}
