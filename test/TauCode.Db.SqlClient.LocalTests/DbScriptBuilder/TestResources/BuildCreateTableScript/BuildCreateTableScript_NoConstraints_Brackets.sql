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
    [DueDate] [datetime2] NULL)