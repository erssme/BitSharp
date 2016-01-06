﻿using BitSharp.Common;
using BitSharp.Core.Domain;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSharp.Core.Workers
{
    internal class StatsWorker : Worker
    {
        private const int REORG_BUFFER = 6;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ICoreDaemon coreDaemon;

        private readonly Dictionary<int, ConfirmedBlockStats> confirmedBlockStatsByHeight = new Dictionary<int, ConfirmedBlockStats>();
        private int unconfirmedTxesLogHeight = -1;

        public StatsWorker(WorkerConfig workerConfig, ICoreDaemon coreDaemon)
            : base("StatsWorker", workerConfig.initialNotify, workerConfig.minIdleTime, workerConfig.maxIdleTime)
        {
            this.coreDaemon = coreDaemon;

            coreDaemon.UnconfirmedTxAdded += OnUnconfirmedTxAdded;
            coreDaemon.TxesConfirmed += OnTxesConfirmed;
            coreDaemon.TxesUnconfirmed += OnTxesUnconfirmed;
        }

        protected override void SubDispose()
        {
            coreDaemon.UnconfirmedTxAdded -= OnUnconfirmedTxAdded;
            coreDaemon.TxesConfirmed -= OnTxesConfirmed;
            coreDaemon.TxesUnconfirmed -= OnTxesUnconfirmed;
        }

        protected override Task WorkAction()
        {
            var currentChain = coreDaemon.CurrentChain;
            if (currentChain == null)
                return Task.CompletedTask;

            // init stats logging at current chain height on first run
            if (unconfirmedTxesLogHeight == -1)
                unconfirmedTxesLogHeight = currentChain.Height;

            lock (confirmedBlockStatsByHeight)
            {
                // log confirmed txes stats at each height, up to the re-org buffer allowance
                while (unconfirmedTxesLogHeight < currentChain.Height - REORG_BUFFER)
                {
                    var nextLogHeight = unconfirmedTxesLogHeight + 1;

                    ConfirmedBlockStats confirmedBlockStats;
                    if (confirmedBlockStatsByHeight.TryGetValue(nextLogHeight, out confirmedBlockStats))
                    {
                        var chainedHeader = confirmedBlockStats.ChainedHeader;
                        var confirmedTxesStats = confirmedBlockStats.ConfirmedTxesStats;
                        var confirmTime = confirmedBlockStats.ConfirmTime;

                        // don't log stats on initial sync, only log blocks that have been processed within an hour of their header time
                        var blockTime = DateTimeOffset.FromUnixTimeSeconds(currentChain.Blocks[nextLogHeight].Time);
                        if (confirmTime - blockTime <= TimeSpan.FromHours(1))
                        {
                            // log confirmed txes stats
                            var statsString = new StringBuilder();
                            statsString.AppendLine($"[StatsWorker] Block {chainedHeader.Height}, {chainedHeader.Hash} confirmed at: {confirmTime}");
                            foreach (var confirmedTxStats in confirmedTxesStats)
                            {
                                statsString.AppendLine(
                                    string.Format(@"{{ ""hash"": ""{0}"", ""confirmationMilliseconds"": {1}, ""fee"": {2}, ""txByteSize"": {3} }}",
                                        confirmedTxStats.Hash, (int)confirmedTxStats.ConfirmationTimeSpan.TotalMilliseconds,
                                        confirmedTxStats.Fee, confirmedTxStats.TxByteSize));
                            }

                            logger.Info(statsString);
                        }

                        confirmedBlockStatsByHeight.Remove(nextLogHeight);
                    }
                    else
                    {
                        logger.Warn($"[StatsWorker] Missing block confirmation stats at height {nextLogHeight}");
                    }

                    unconfirmedTxesLogHeight = nextLogHeight;
                }
            }

            return Task.CompletedTask;
        }

        private void OnUnconfirmedTxAdded(object sender, UnconfirmedTxAddedEventArgs e)
        {
        }

        private void OnTxesConfirmed(object sender, TxesConfirmedEventArgs e)
        {
            var now = DateTime.UtcNow;

            // collect the stats on the confirmed txes
            var confirmedTxes = e.ConfirmedTxes.Values.Select(
                confirmedTx => new ConfirmedTxStats(confirmedTx.Hash, now - confirmedTx.DateSeen, confirmedTx.Fee, confirmedTx.TxByteSize))
                .ToList();

            // keep track of the stats for logging
            lock (confirmedBlockStatsByHeight)
                confirmedBlockStatsByHeight[e.ConfirmBlock.Height] = new ConfirmedBlockStats(e.ConfirmBlock, confirmedTxes, now);

            NotifyWork();
        }

        private void OnTxesUnconfirmed(object sender, TxesUnconfirmedEventArgs e)
        {
            // re-org, remove stats at height on a roll back
            lock (confirmedBlockStatsByHeight)
                confirmedBlockStatsByHeight.Remove(e.UnconfirmBlock.Height);

            // detect re-org that went past the re-org buffer allowance
            if (e.UnconfirmBlock.Height <= unconfirmedTxesLogHeight - REORG_BUFFER)
                logger.Warn($"StatsWorker re-orged past its allowance of {REORG_BUFFER} blocks at height {e.UnconfirmBlock.Height}");
        }

        private class ConfirmedBlockStats
        {
            public ConfirmedBlockStats(ChainedHeader chainedHeader, List<ConfirmedTxStats> confirmedTxesStats, DateTime confirmTime)
            {
                ChainedHeader = chainedHeader;
                ConfirmedTxesStats = confirmedTxesStats;
                ConfirmTime = confirmTime;
            }

            public ChainedHeader ChainedHeader { get; }

            public List<ConfirmedTxStats> ConfirmedTxesStats { get; }

            public DateTime ConfirmTime { get; }
        }

        private class ConfirmedTxStats
        {
            public ConfirmedTxStats(UInt256 hash, TimeSpan confirmationTimeSpan, ulong fee, int txByteSize)
            {
                Hash = hash;
                ConfirmationTimeSpan = confirmationTimeSpan;
                Fee = fee;
                TxByteSize = txByteSize;
            }

            public UInt256 Hash { get; }

            public TimeSpan ConfirmationTimeSpan { get; }

            public ulong Fee { get; }

            public int TxByteSize { get; }
        }
    }
}
