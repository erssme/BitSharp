﻿using BitSharp.Common.ExtensionMethods;
using BitSharp.Core.Test;
using BitSharp.Wallet.Address;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;

namespace BitSharp.Wallet.Test
{
    [TestClass]
    public class WalletMonitorTest
    {
        [TestMethod]
        [Timeout(5 * /*minutes*/(60 * 1000))]
        public void TestMonitorAddress()
        {
            var publicKey =
                "04f9804cfb86fb17441a6562b07c4ee8f012bdb2da5be022032e4b87100350ccc7c0f4d47078b06c9d22b0ec10bdce4c590e0d01aed618987a6caa8c94d74ee6dc"
                .HexToByteArray().ToImmutableArray();

            using (var simulator = new MainnetSimulator())
            using (var walletMonitor = new WalletMonitor(simulator.CoreDaemon))
            {
                walletMonitor.AddAddress(new PublicKeyAddress(publicKey));
                walletMonitor.Start();

                var block9999 = simulator.BlockProvider.GetBlock(9999);

                simulator.AddBlockRange(0, 9999);
                simulator.WaitForUpdate();
                simulator.AssertAtBlock(9999, block9999.Hash);

                walletMonitor.WaitForUpdate();
                Assert.AreEqual(9999, walletMonitor.WalletHeight);

                var minedTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Mine).ToList();
                var receivedTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Receive).ToList();
                var spentTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Spend).ToList();

                var actualMinedBtc = minedTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();
                var actualReceivedBtc = receivedTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();
                var actualSpentBtc = spentTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();

                Assert.AreEqual(0, minedTxOutputs.Count);
                Assert.AreEqual(16, receivedTxOutputs.Count);
                Assert.AreEqual(14, spentTxOutputs.Count);
                Assert.AreEqual(0M, actualMinedBtc);
                Assert.AreEqual(569.44M, actualReceivedBtc);
                Assert.AreEqual(536.52M, actualSpentBtc);
            }
        }

        [TestMethod]
        [Timeout(5 * /*minutes*/(60 * 1000))]
        public void TestMonitorAddressRollback()
        {
            var publicKey =
                "04f9804cfb86fb17441a6562b07c4ee8f012bdb2da5be022032e4b87100350ccc7c0f4d47078b06c9d22b0ec10bdce4c590e0d01aed618987a6caa8c94d74ee6dc"
                .HexToByteArray().ToImmutableArray();

            using (var simulator = new MainnetSimulator())
            using (var walletMonitor = new WalletMonitor(simulator.CoreDaemon))
            {
                walletMonitor.AddAddress(new PublicKeyAddress(publicKey));
                walletMonitor.Start();

                var block0 = simulator.BlockProvider.GetBlock(0);
                var block9999 = simulator.BlockProvider.GetBlock(9999);

                simulator.AddBlockRange(0, 9999);
                simulator.WaitForUpdate();
                simulator.AssertAtBlock(9999, block9999.Hash);

                // verify initial wallet state
                walletMonitor.WaitForUpdate();
                Assert.AreEqual(9999, walletMonitor.WalletHeight);

                var minedTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Mine).ToList();
                var receivedTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Receive).ToList();
                var spentTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Spend).ToList();

                var actualMinedBtc = minedTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();
                var actualReceivedBtc = receivedTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();
                var actualSpentBtc = spentTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();

                Assert.AreEqual(0, minedTxOutputs.Count);
                Assert.AreEqual(16, receivedTxOutputs.Count);
                Assert.AreEqual(14, spentTxOutputs.Count);
                Assert.AreEqual(0M, actualMinedBtc);
                Assert.AreEqual(569.44M, actualReceivedBtc);
                Assert.AreEqual(536.52M, actualSpentBtc);

                // mark chain as invalid back to genesis
                simulator.CoreDaemon.CoreStorage.MarkBlockInvalid(simulator.BlockProvider.GetBlock(1).Hash, simulator.CoreDaemon.TargetChain);

                // verify chain state reset to genesis
                simulator.WaitForUpdate();
                simulator.AssertAtBlock(0, block0.Hash);

                // verify wallet state rolled back to genesis
                walletMonitor.WaitForUpdate();
                Assert.AreEqual(0, walletMonitor.WalletHeight);

                var unminedTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.UnMine).ToList();
                var unreceivedTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Receive).ToList();
                var unspentTxOutputs = walletMonitor.Entries.Where(x => x.Type == EnumWalletEntryType.Spend).ToList();

                var actualUnminedBtc = unminedTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();
                var actualUnreceivedBtc = unreceivedTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();
                var actualUnspentBtc = unspentTxOutputs.Sum(x => (decimal)x.Value) / 100.MILLION();

                Assert.AreEqual(minedTxOutputs.Count, unminedTxOutputs.Count);
                Assert.AreEqual(receivedTxOutputs.Count, unreceivedTxOutputs.Count);
                Assert.AreEqual(spentTxOutputs.Count, unspentTxOutputs.Count);
                Assert.AreEqual(actualMinedBtc, actualUnminedBtc);
                Assert.AreEqual(actualReceivedBtc, actualUnreceivedBtc);
                Assert.AreEqual(actualSpentBtc, actualUnspentBtc);
            }
        }
    }
}
