/*

DROP TABLE [zeta].[NumericData]

DROP TABLE [zeta].[DateData]

DROP TABLE [zeta].[HealthInfo]

DROP TABLE [zeta].[TaxInfo]

DROP TABLE [zeta].[WorkInfo]

DROP TABLE [zeta].[PersonData]

DROP TABLE [zeta].[Person]

DROP SCHEMA [zeta]

CREATE SCHEMA [zeta]
GO

*/

/*** Person ***/
CREATE TABLE [zeta].[Person](
	[MetaKey] [smallint] NOT NULL,
	[OrdNumber] [tinyint] NOT NULL,
	[Id] [bigint] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Birthday] [date] NOT NULL,
	[Gender] [bit] NULL,
	[Initials] [nchar](2) NULL,
	CONSTRAINT [PK_person] PRIMARY KEY([Id], [MetaKey], [OrdNumber])
)

/*** PersonData ***/
CREATE TABLE [zeta].[PersonData](
	[Id] [uniqueidentifier] NOT NULL,
	[Height] [int] NULL,
	[Photo] [varbinary](max) NULL,
	[EnglishDescription] [varchar](max) NOT NULL,
	[UnicodeDescription] [nvarchar](max) NOT NULL,
	[PersonMetaKey] [smallint] NOT NULL,
	[PersonOrdNumber] [tinyint] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	CONSTRAINT [PK_personData] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_personData_Person] FOREIGN KEY([PersonId], [PersonMetaKey], [PersonOrdNumber]) REFERENCES [zeta].[Person]([Id], [MetaKey], [OrdNumber])
)

/*** WorkInfo ***/
CREATE TABLE [zeta].[WorkInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[Position] [varchar](20) NOT NULL,
	[HireDate] [smalldatetime] NOT NULL,
	[Code] [char](3) NULL,
	[PersonMetaKey] [smallint] NOT NULL,
	[DigitalSignature] [binary](16) NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[PersonOrdNumber] [tinyint] NOT NULL,
	[Hash] [uniqueidentifier] NOT NULL,
	[Salary] [smallmoney] NULL,
	[VaryingSignature] [varbinary](100) NULL,
	CONSTRAINT [PK_workInfo] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_workInfo_Person] FOREIGN KEY([PersonId], [PersonMetaKey], [PersonOrdNumber]) REFERENCES [zeta].[Person]([Id], [MetaKey], [OrdNumber])
)

/*** WorkInfo - index on [Hash] ***/
CREATE UNIQUE INDEX [UX_workInfo_Hash] ON [zeta].[WorkInfo]([Hash])

/*** TaxInfo ***/
CREATE TABLE [zeta].[TaxInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Tax] [money] NOT NULL,
	[Ratio] [float] NULL,
	[PersonMetaKey] [smallint] NOT NULL,
	[SmallRatio] [real] NOT NULL,
	[RecordDate] [datetime] NULL,
	[CreatedAt] [datetimeoffset] NOT NULL,
	[PersonOrdNumber] [tinyint] NOT NULL,
	[DueDate] [datetime2] NULL,
	CONSTRAINT [PK_taxInfo] PRIMARY KEY([Id]),
	CONSTRAINT [FK_taxInfo_Person] FOREIGN KEY([PersonId], [PersonMetaKey], [PersonOrdNumber]) REFERENCES [zeta].[Person]([Id], [MetaKey], [OrdNumber]))

/*** HealthInfo ***/
CREATE TABLE [zeta].[HealthInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Weight] [decimal](8, 2) NOT NULL,
	[PersonMetaKey] [smallint] NOT NULL,
	[IQ] [numeric](8, 2) NULL,
	[Temper] [smallint] NULL,
	[PersonOrdNumber] [tinyint] NOT NULL,
	[MetricB] [int] NULL,
	[MetricA] [int] NULL,
	CONSTRAINT [PK_healthInfo] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_healthInfo_Person] FOREIGN KEY([PersonId], [PersonMetaKey], [PersonOrdNumber]) REFERENCES [zeta].[Person]([Id], [MetaKey], [OrdNumber])
)

/*** HealthInfo - index on [MetricA], [MetricB] ***/
CREATE INDEX [IX_healthInfo_metricAmetricB] ON [zeta].[HealthInfo]([MetricA] ASC, [MetricB] DESC)

/*** NumericData ***/
CREATE TABLE [zeta].[NumericData](
	[Id] [int] NOT NULL IDENTITY(15, 99),
	[BooleanData] [bit] NULL,
	[ByteData] [tinyint] NULL,
	[Int16] [smallint] NULL,
	[Int32] [int] NULL,
	[Int64] [bigint] NULL,
	[NetDouble] [float] NULL,
	[NetSingle] [real] NULL,
	[NumericData] [numeric](10, 6) NULL,
	[DecimalData] [decimal](11, 5) NULL,
	CONSTRAINT [PK_numericData] PRIMARY KEY ([Id])
)

/*** DateData ***/
CREATE TABLE [zeta].[DateData](
	[Id] [uniqueidentifier] NOT NULL,
	[Moment] [datetimeoffset] NULL,
	CONSTRAINT [PK_dateData] PRIMARY KEY ([Id])
)
