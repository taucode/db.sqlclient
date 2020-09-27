/* create table: user */
CREATE TABLE [user](
	[id] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[birthday] [datetime] NOT NULL,
	[gender] [bit] NOT NULL,
	[picture] varbinary(4000) NOT NULL,
	CONSTRAINT [PK_user] PRIMARY KEY([id]))


/* create table: user_info */
CREATE TABLE [user_info](
	[id] uniqueidentifier NOT NULL,
	[user_id] uniqueidentifier NOT NULL,
	[tax_number] char(10) NOT NULL,
	[code] nchar(20) NOT NULL,
	[ansi_name] varchar(200) NULL,
	[ansi_description] varchar(max) NOT NULL,
	[unicode_description] nvarchar(max) NOT NULL,
	[height] float NOT NULL,
	[weight] double precision NOT NULL,
	[weight2] real NOT NULL,
	[salary] money NOT NULL,
	[rating_decimal] decimal (19, 7) NOT NULL,
	[rating_numeric] numeric (18, 5) NOT NULL,
	[num8] tinyint NOT NULL,
	[num16] smallint NOT NULL,
	[num32] int NOT NULL,
	[num64] bigint NOT NULL,

	CONSTRAINT [PK_userInfo] PRIMARY KEY([id] DESC),
	CONSTRAINT [FK_userInfo_user] FOREIGN KEY([user_id]) REFERENCES [user]([id]))

/* create index: UX_userInfo_taxNumber */
CREATE UNIQUE INDEX [UX_userInfo_taxNumber] ON [user_info]([tax_number])