/* Table: 'Person' */
CREATE TABLE [zeta].[Person](
    [Id] [int] NOT NULL,
    [Tag] [uniqueidentifier] NULL,
    [IsChecked] [bit] NULL,
    [Birthday] [smalldatetime] NULL,
    [FirstName] [nvarchar](100) NULL,
    [LastName] [nvarchar](100) NULL,
    [Initials] [nchar](2) NULL,
    [Gender] [tinyint] NULL,
    CONSTRAINT [PK_person] PRIMARY KEY([Id]))

/* Index: 'UX_person_tag' on table 'Person' */
CREATE UNIQUE INDEX [UX_person_tag] ON [zeta].[Person]([Tag] ASC)

/* Table: 'PersonData' */
CREATE TABLE [zeta].[PersonData](
    [Id] [smallint] NOT NULL,
    [PersonId] [int] NOT NULL,
    [BestAge] [tinyint] NULL,
    [Hash] [bigint] NULL,
    [Height] [decimal](10, 2) NULL,
    [Weight] [numeric](10, 2) NULL,
    [UpdatedAt] [datetime2] NULL,
    [Signature] [binary](4) NULL,
    CONSTRAINT [PK_personData] PRIMARY KEY([Id]),
    CONSTRAINT [FK_personData_person] FOREIGN KEY([PersonId]) REFERENCES [zeta].[Person]([Id]))

/* Table: 'WorkInfo' */
CREATE TABLE [zeta].[WorkInfo](
    [Id] [int] NOT NULL,
    [PersonId] [int] NOT NULL,
    [PositionCode] [varchar](100) NOT NULL,
    [PositionDescription] [nvarchar](max) NULL,
    [PositionDescriptionEn] [varchar](max) NULL,
    [HiredOn] [datetime] NULL,
    [WorkStartDayTime] [time] NULL,
    [Salary] [money] NULL,
    [Bonus] [smallmoney] NULL,
    [OvertimeCoef] [real] NULL,
    [WeekendCoef] [float] NULL,
    [Url] [varchar](200) NULL,
    CONSTRAINT [PK_workInfo] PRIMARY KEY([Id]),
    CONSTRAINT [FK_workInfo_person] FOREIGN KEY([PersonId]) REFERENCES [zeta].[Person]([Id]))

/* Index: 'IX_workInfo_salary_bonus' on table 'WorkInfo' */
CREATE INDEX [IX_workInfo_salary_bonus] ON [zeta].[WorkInfo]([Salary] ASC, [Bonus] DESC)

/* Table: 'Photo' */
CREATE TABLE [zeta].[Photo](
    [Id] [char](4) NOT NULL,
    [PersonDataId] [smallint] NOT NULL,
    [Content] [varbinary](max) NOT NULL,
    [ContentThumbnail] [varbinary](4000) NULL,
    [TakenAt] [datetimeoffset] NULL,
    [ValidUntil] [date] NULL,
    CONSTRAINT [PK_photo] PRIMARY KEY([Id]),
    CONSTRAINT [FK_photo_personData] FOREIGN KEY([PersonDataId]) REFERENCES [zeta].[PersonData]([Id]))

