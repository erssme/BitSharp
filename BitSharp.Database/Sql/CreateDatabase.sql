CREATE TABLE IF NOT EXISTS [BlockData](
	[BlockHash] [binary](32) NOT NULL,
	[RawBytes] [varbinary](100000000) NOT NULL,
	CONSTRAINT [PK_Blocks] PRIMARY KEY
	(
		[BlockHash] ASC
	)
);

CREATE TABLE IF NOT EXISTS [BlockMetadata](
	[BlockHash] [binary](32) NOT NULL,
	[PreviousBlockHash] [binary](32) NOT NULL,
	[Work] [binary](64) NOT NULL,
	[Height] [bigint] NULL,
	[TotalWork] [binary](64) NULL,
	[IsValid] [bit] NULL,
	CONSTRAINT [PK_BlockMetaData] PRIMARY KEY
	(
		[BlockHash] ASC
	)
);

CREATE INDEX IF NOT EXISTS [IX_BlockMetadata_PreviousBlockHash] ON [BlockMetadata] ( [PreviousBlockHash] );

CREATE INDEX IF NOT EXISTS [IX_BlockMetadata_Height] ON [BlockMetadata] ( [Height] );

CREATE INDEX IF NOT EXISTS [IX_BlockMetadata_TotalWork] ON [BlockMetadata] ( [TotalWork] );

CREATE TABLE IF NOT EXISTS [KnownAddresses](
	[IPAddress] [binary](16) NOT NULL,
	[Port] [binary](2) NOT NULL,
	[Services] [binary](8) NOT NULL,
	[Time] [binary](4) NOT NULL,
	CONSTRAINT [PK_KnownAddresses] PRIMARY KEY
	(
		[IPAddress] ASC,
		[Port] ASC
	)
);

CREATE TABLE IF NOT EXISTS [Transactions](
	[TransactionHash] [binary](32) NOT NULL,
	[TransactionData] [varbinary](100000) NOT NULL,
	CONSTRAINT [PK_Transactions] PRIMARY KEY
	(
		[TransactionHash] ASC
	)
);

CREATE TABLE IF NOT EXISTS [UtxoData](
	[RootBlockHash] [binary](32) NOT NULL,
	[PreviousTransactionHash] [binary](32) NOT NULL,
	[PreviousTransactionOutputIndex] [binary](4) NOT NULL,
	CONSTRAINT [PK_UtxoData] PRIMARY KEY
	(
		[RootBlockHash] ASC,
		[PreviousTransactionHash] ASC,
		[PreviousTransactionOutputIndex] ASC
	)
);
