﻿CREATE TABLE Utenti (
    ID INT IDENTITY PRIMARY KEY,
    Nome NVARCHAR(50) NOT NULL,
    Cognome NVARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(128) NOT NULL,
    Ruolo VARCHAR(20) CHECK (Ruolo IN ('Dipendente', 'Manager', 'Admin')) NOT NULL,
    Team VARCHAR(50) NULL,
    DataCreazione DATETIME DEFAULT GETDATE()
);

CREATE TABLE Post (
    ID INT IDENTITY PRIMARY KEY,
    Testo NVARCHAR(MAX) NOT NULL,
    DataCreazione DATETIME DEFAULT GETDATE(),
    AutoreID INT NOT NULL,
    FOREIGN KEY (AutoreID) REFERENCES Utenti(ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Forum (
    ID INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descrizione VARCHAR(255) NULL,
    Team VARCHAR(50) NULL
);

CREATE TABLE ThreadForum (
    ID INT IDENTITY PRIMARY KEY,
    Titolo NVARCHAR(255) NOT NULL,
    ForumID INT NOT NULL,
    AutoreID INT NOT NULL,
    DataCreazione DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ForumID) REFERENCES Forum(ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (AutoreID) REFERENCES Utenti(ID) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE MessaggiThread (
    ID INT IDENTITY PRIMARY KEY,
    ThreadID INT NOT NULL,
    AutoreID INT NOT NULL,
    Testo NVARCHAR(MAX) NOT NULL,
    DataCreazione DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ThreadID) REFERENCES ThreadForum(ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (AutoreID) REFERENCES Utenti(ID) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE Ticket (
    ID INT IDENTITY PRIMARY KEY,
    Descrizione NVARCHAR(MAX) NOT NULL,
    Stato VARCHAR(20) CHECK (Stato IN ('Aperto', 'In lavorazione', 'Chiuso')) NOT NULL DEFAULT 'Aperto',
    CreatoDaID INT NOT NULL,
    AssegnatoAID INT NULL,
    DataApertura DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CreatoDaID) REFERENCES Utenti(ID) ON DELETE NO ACTION ON UPDATE NO ACTION,
    FOREIGN KEY (AssegnatoAID) REFERENCES Utenti(ID) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE TABLE Likes (
    ID INT IDENTITY PRIMARY key,
    UtenteID INT NOT NULL,
    PostID INT NOT NULL,
    MiPiace BIT NOT NULL,
    FOREIGN KEY (UtenteID) REFERENCES Utenti(ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (PostID) REFERENCES Post(ID) ON DELETE CASCADE ON UPDATE CASCADE
);
