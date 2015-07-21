﻿using BitSharp.Common.ExtensionMethods;
using BitSharp.Core;
using BitSharp.Core.Domain;
using BitSharp.Core.Storage;
using BitSharp.Node;
using BitSharp.Wallet;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace BitSharp.Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IKernel kernel;
        private readonly CoreDaemon coreDaemon;
        private readonly CoreStorage coreStorage;
        private readonly LocalClient localClient;

        private readonly DateTime startTime;
        private string runningTime;
        private readonly DispatcherTimer runningTimeTimer;
        private readonly DispatcherTimer updateTimer;

        private long _winningBlockchainHeight;
        private long _currentBlockchainHeight;
        private long _downloadedBlockCount;

        private float blockRate;
        private float transactionRate;
        private float inputRate;
        private float blockDownloadRate;
        private int duplicateBlockDownloadCount;
        private int blockMissCount;

        private readonly WalletMonitor walletMonitor;
        private int walletHeight;
        private int walletEntriesCount;
        private decimal bitBalance;
        private decimal btcBalance;

        private readonly Dispatcher dispatcher;

        public MainWindowViewModel(IKernel kernel, WalletMonitor walletMonitor = null)
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;

            this.kernel = kernel;
            this.coreDaemon = kernel.Get<CoreDaemon>();
            this.coreStorage = this.coreDaemon.CoreStorage;
            this.localClient = kernel.Get<LocalClient>();
            this.walletMonitor = walletMonitor;

            this.startTime = DateTime.UtcNow;
            this.runningTimeTimer = new DispatcherTimer();
            runningTimeTimer.Tick += (sender, e) =>
            {
                var runningTime = (DateTime.UtcNow - this.startTime);
                this.RunningTime = $"{Math.Floor(runningTime.TotalHours):#,#00}:{runningTime:mm':'ss}";
            };
            runningTimeTimer.Interval = TimeSpan.FromMilliseconds(100);
            runningTimeTimer.Start();

            this.updateTimer = new DispatcherTimer();
            updateTimer.Tick += (sender, e) =>
            {
                this.WinningBlockchainHeight = this.coreDaemon.TargetChainHeight;
                this.CurrentBlockchainHeight = this.coreDaemon.CurrentChain.Height;
                this.DownloadedBlockCount = this.coreStorage.BlockWithTxesCount;

                this.BlockRate = this.coreDaemon.GetBlockRate();
                this.TransactionRate = this.coreDaemon.GetTxRate();
                this.InputRate = this.coreDaemon.GetInputRate();
                
                this.BlockDownloadRate = this.localClient.GetBlockDownloadRate();
                this.DuplicateBlockDownloadCount = this.localClient.GetDuplicateBlockDownloadCount();
                this.BlockMissCount = this.localClient.GetBlockMissCount();

                if (walletMonitor != null)
                {
                    this.WalletHeight = this.walletMonitor.WalletHeight;
                    this.WalletEntriesCount = this.walletMonitor.EntriesCount;
                    this.BitBalance = this.walletMonitor.BitBalance;
                    this.BtcBalance = this.walletMonitor.BtcBalance;
                }
            };
            updateTimer.Interval = TimeSpan.FromSeconds(1);
            updateTimer.Start();
        }

        public string RunningTime
        {
            get { return this.runningTime; }
            set { SetValue(ref this.runningTime, value); }
        }

        public long WinningBlockchainHeight
        {
            get { return this._winningBlockchainHeight; }
            set { SetValue(ref this._winningBlockchainHeight, value); }
        }

        public long CurrentBlockchainHeight
        {
            get { return this._currentBlockchainHeight; }
            set { SetValue(ref this._currentBlockchainHeight, value); }
        }

        public long DownloadedBlockCount
        {
            get { return this._downloadedBlockCount; }
            set { SetValue(ref this._downloadedBlockCount, value); }
        }

        public float BlockRate
        {
            get { return this.blockRate; }
            set { SetValue(ref this.blockRate, value); }
        }

        public float TransactionRate
        {
            get { return this.transactionRate; }
            set { SetValue(ref this.transactionRate, value); }
        }

        public float InputRate
        {
            get { return this.inputRate; }
            set { SetValue(ref this.inputRate, value); }
        }

        public float BlockDownloadRate
        {
            get { return this.blockDownloadRate; }
            set { SetValue(ref this.blockDownloadRate, value); }
        }

        public int DuplicateBlockDownloadCount
        {
            get { return this.duplicateBlockDownloadCount; }
            set { SetValue(ref this.duplicateBlockDownloadCount, value); }
        }

        public int BlockMissCount
        {
            get { return this.blockMissCount; }
            set { SetValue(ref this.blockMissCount, value); }
        }

        public int WalletHeight
        {
            get { return this.walletHeight; }
            set { SetValue(ref this.walletHeight, value); }
        }

        public int WalletEntriesCount
        {
            get { return this.walletEntriesCount; }
            set { SetValue(ref this.walletEntriesCount, value); }
        }

        public decimal BitBalance
        {
            get { return this.bitBalance; }
            set { SetValue(ref this.bitBalance, value); }
        }

        public decimal BtcBalance
        {
            get { return this.btcBalance; }
            set { SetValue(ref this.btcBalance, value); }
        }

        private void SetValue<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = "") where T : IEquatable<T>
        {
            if (currentValue == null || !currentValue.Equals(newValue))
            {
                currentValue = newValue;

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
