/* create table: fragment_type */
CREATE TABLE [fragment_type](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    [is_default] [bit] NOT NULL,
    CONSTRAINT [PK_fragmentType] PRIMARY KEY([id]))

/* create unique index UX_fragmentType_code: fragment_type(code) */
CREATE UNIQUE INDEX [UX_fragmentType_code] ON [fragment_type]([code])

/* create table: language */
CREATE TABLE [language](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](2) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_language] PRIMARY KEY([id]))

/* create unique index UX_language_code: language(code) */
CREATE UNIQUE INDEX [UX_language_code] ON [language]([code])

/* create table: note */
CREATE TABLE [note](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [created_at] [datetime] NOT NULL,
    CONSTRAINT [PK_note] PRIMARY KEY([id]))

/* create unique index UX_note_code: note(code) */
CREATE UNIQUE INDEX [UX_note_code] ON [note]([code])

/* create unique index UX_note_createdAt: note(created_at) */
CREATE UNIQUE INDEX [UX_note_createdAt] ON [note]([created_at])

/* create table: tag */
CREATE TABLE [tag](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_tag] PRIMARY KEY([id]))

/* create unique index UX_tag_code: tag(code) */
CREATE UNIQUE INDEX [UX_tag_code] ON [tag]([code])

/* create table: user */
CREATE TABLE [user](
    [id] [uniqueidentifier] NOT NULL,
    [username] [nvarchar](255) NOT NULL,
    [email] [nvarchar](255) NOT NULL,
    [password_hash] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_user] PRIMARY KEY([id]))

/* create unique index UX_user_email: user(email) */
CREATE UNIQUE INDEX [UX_user_email] ON [user]([email])

/* create unique index UX_user_username: user(username) */
CREATE UNIQUE INDEX [UX_user_username] ON [user]([username])

/* create table: fragment_sub_type */
CREATE TABLE [fragment_sub_type](
    [id] [uniqueidentifier] NOT NULL,
    [type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    [is_default] [bit] NOT NULL,
    CONSTRAINT [PK_fragmentSubType] PRIMARY KEY([id]))

/* create unique index UX_fragmentSubType_typeId_code: fragment_sub_type(type_id, code) */
CREATE UNIQUE INDEX [UX_fragmentSubType_typeId_code] ON [fragment_sub_type]([type_id], [code])

/* create foreign key FK_fragmentSubType_fragmentType: fragment_sub_type(type_id) -> fragment_type(id) */
ALTER TABLE [fragment_sub_type] ADD CONSTRAINT [FK_fragmentSubType_fragmentType] FOREIGN KEY([type_id])
REFERENCES [fragment_type]([id])

/* create table: note_tag */
CREATE TABLE [note_tag](
    [id] [uniqueidentifier] NOT NULL,
    [note_id] [uniqueidentifier] NOT NULL,
    [tag_id] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_noteTag] PRIMARY KEY([id]))

/* create foreign key FK_noteTag_note: note_tag(note_id) -> note(id) */
ALTER TABLE [note_tag] ADD CONSTRAINT [FK_noteTag_note] FOREIGN KEY([note_id])
REFERENCES [note]([id])

/* create foreign key FK_noteTag_tag: note_tag(tag_id) -> tag(id) */
ALTER TABLE [note_tag] ADD CONSTRAINT [FK_noteTag_tag] FOREIGN KEY([tag_id])
REFERENCES [tag]([id])

/* create table: note_translation */
CREATE TABLE [note_translation](
    [id] [uniqueidentifier] NOT NULL,
    [note_id] [uniqueidentifier] NOT NULL,
    [language_id] [uniqueidentifier] NOT NULL,
    [is_default] [bit] NOT NULL,
    [is_published] [bit] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [last_updated] [datetime] NOT NULL,
    CONSTRAINT [PK_noteTranslation] PRIMARY KEY([id]))

/* create index IX_note_noteId: note_translation(note_id) */
CREATE INDEX [IX_note_noteId] ON [note_translation]([note_id])

/* create foreign key FK_noteTranslation_language: note_translation(language_id) -> language(id) */
ALTER TABLE [note_translation] ADD CONSTRAINT [FK_noteTranslation_language] FOREIGN KEY([language_id])
REFERENCES [language]([id])

/* create foreign key FK_noteTranslation_note: note_translation(note_id) -> note(id) */
ALTER TABLE [note_translation] ADD CONSTRAINT [FK_noteTranslation_note] FOREIGN KEY([note_id])
REFERENCES [note]([id])

/* create table: fragment */
CREATE TABLE [fragment](
    [id] [uniqueidentifier] NOT NULL,
    [note_translation_id] [uniqueidentifier] NOT NULL,
    [sub_type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NULL,
    [order] [int] NOT NULL,
    [content] [ntext] NOT NULL,
    CONSTRAINT [PK_fragment] PRIMARY KEY([id]))

/* create unique index UX_fragment_noteTranslationId_code: fragment(note_translation_id, code) */
CREATE UNIQUE INDEX [UX_fragment_noteTranslationId_code] ON [fragment]([note_translation_id], [code])

/* create unique index UX_fragment_noteTranslationId_order: fragment(note_translation_id, order) */
CREATE UNIQUE INDEX [UX_fragment_noteTranslationId_order] ON [fragment]([note_translation_id], [order])

/* create foreign key FK_fragment_noteTranslation: fragment(note_translation_id) -> note_translation(id) */
ALTER TABLE [fragment] ADD CONSTRAINT [FK_fragment_noteTranslation] FOREIGN KEY([note_translation_id])
REFERENCES [note_translation]([id])

/* create foreign key FK_fragment_subType: fragment(sub_type_id) -> fragment_sub_type(id) */
ALTER TABLE [fragment] ADD CONSTRAINT [FK_fragment_subType] FOREIGN KEY([sub_type_id])
REFERENCES [fragment_sub_type]([id])

/* create table: foo */
CREATE TABLE [foo](
    [id] int NOT NULL PRIMARY KEY,
    [name] nvarchar(100) NULL,
    [enum_int32] int NULL,
    [enum_string] varchar(100) NULL
)
