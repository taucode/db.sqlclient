CREATE TABLE [zeta].[SuperTable](
	[Id] int NOT NULL PRIMARY KEY IDENTITY(1, 1),

	[TheGuid] uniqueidentifier NULL,

	[TheBit] bit NULL,

	[TheTinyInt] tinyint NULL,
	[TheSmallInt] smallint NULL,
	[TheInt] int NULL,
	[TheBigInt] bigint NULL,

	[TheDecimal] decimal(10, 2) NULL,
	[TheNumeric] decimal(10, 2) NULL,

	[TheSmallMoney] smallmoney NULL,
	[TheMoney] money NULL,

	[TheReal] real NULL,
	[TheFloat] float NULL,

	[TheDate] date NULL,
	[TheDateTime] datetime NULL,
	[TheDateTime2] datetime2 NULL,
	[TheDateTimeOffset] datetimeoffset NULL,
	[TheSmallDateTime] smalldatetime NULL,
	[TheTime] time NULL,

	[TheChar] char(10) NULL,
	[TheVarChar] varchar(100) NULL,
	[TheVarCharMax] varchar(max) NULL,

	[TheNChar] nchar(10) NULL,
	[TheNVarChar] nvarchar(100) NULL,
	[TheNVarCharMax] nvarchar(max) NULL,

	[TheBinary] binary(4) NULL,
	[TheVarBinary] varbinary(1000) NULL,
	[TheVarBinaryMax] varbinary(max) NULL)
