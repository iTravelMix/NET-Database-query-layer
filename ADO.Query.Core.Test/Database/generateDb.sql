CREATE TABLE [dbo].[User] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Phones] (
    [Id]    INT            NOT NULL,
    [usid]  INT            NOT NULL,
    [phone] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Phones] FOREIGN KEY ([usid]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

GO

CREATE TABLE [dbo].[Emails] (
    [Id]    INT            NOT NULL,
    [usid]  INT            NOT NULL,
    [email] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Emails] FOREIGN KEY ([usid]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

GO

INSERT INTO [dbo].[User] (Id,Name) VALUES (1,'Diana Hendrix'),(2,'Sopoline Maxwell'),(3,'Richard Owens');

GO

INSERT INTO [dbo].[Phones] (Id, usid, phone) VALUES (1, 1,'123456'),(2, 1,'987654');

GO

INSERT INTO [dbo].[emails] (Id, usid, email) VALUES (1, 1,'a@b.com'),(2, 1,'c@d.com');
