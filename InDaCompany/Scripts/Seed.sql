INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione) VALUES
('Mario', 'Rossi', 'mario.rossi@azienda.com', 'hash123456789', 'Admin', NULL, CONVERT(DATETIME, '01/01/2024', 103)),
('Laura', 'Bianchi', 'laura.bianchi@azienda.com', 'hash987654321', 'Manager', 'Sviluppo', CONVERT(DATETIME, '02/01/2024', 103)),
('Giuseppe', 'Verdi', 'giuseppe.verdi@azienda.com', 'hash456789123', 'Dipendente', 'Sviluppo', CONVERT(DATETIME, '03/01/2024', 103)),
('Anna', 'Neri', 'anna.neri@azienda.com', 'hash789123456', 'Dipendente', 'Marketing', CONVERT(DATETIME, '04/01/2024', 103)),
('Marco', 'Ferrari', 'marco.ferrari@azienda.com', 'hash321654987', 'Manager', 'Marketing', CONVERT(DATETIME, '05/01/2024', 103));

INSERT INTO Forum (Nome, Descrizione, Team) VALUES
('Generale', 'Forum generale aziendale', NULL),
('Sviluppo Software', 'Discussioni tecniche', 'Sviluppo'),
('Marketing Strategico', 'Strategie e campagne', 'Marketing'),
('HR', 'Risorse Umane', 'HR'),
('Supporto Tecnico', 'Assistenza IT', 'Sviluppo');

INSERT INTO ThreadForum (Titolo, Testo, ForumID, AutoreID, DataCreazione) VALUES
('Benvenuto nel forum', 'Benvenuti nel nuovo sistema aziendale!', 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
('Nuova release v2.0', 'Meeting di team previsto per venerdì', 2, 2, CONVERT(DATETIME, '16/01/2024', 103)),
('Campagna Q1 2025', 'Risultati del Q4 2024', 3, 4, CONVERT(DATETIME, '17/01/2024', 103)),
('Policy Smart Working', 'Aggiornamento procedure interne', 4, 5, CONVERT(DATETIME, '18/01/2024', 103)),
('Manutenzione servers', 'RFC server di sviluppo - Week 9', 5, 3, CONVERT(DATETIME, '19/01/2024', 103));

INSERT INTO MessaggiThread (ThreadID, AutoreID, Testo, DataCreazione) VALUES
(1, 1, 'Benvenuti a tutti nel nuovo forum aziendale', CONVERT(DATETIME, '15/01/2024', 103)),
(2, 3, 'Ecco le novità della versione 2.0', CONVERT(DATETIME, '16/01/2024', 103)),
(3, 3, 'Ottimo lavoro!', CONVERT(DATETIME, '16/01/2024', 103)),
(3, 4, 'Obiettivi Q1 2025', CONVERT(DATETIME, '17/01/2024', 103)),
(4, 5, 'Nuove linee guida smart working', CONVERT(DATETIME, '18/01/2024', 103));

INSERT INTO Ticket (Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura) VALUES
('Supporto Email', 'Problema accesso email', 'Aperto', 3, 1, CONVERT(DATETIME, '20/01/2024', 103)),
('Hardware Request', 'Richiesta nuovo laptop', 'In lavorazione', 4, 2, CONVERT(DATETIME, '21/01/2024', 103)),
('Network Issue', 'Configurazione VPN', 'Chiuso', 5, 1, CONVERT(DATETIME, '22/01/2024', 103)),
('Software Update', 'Aggiornamento software', 'Aperto', 2, 3, CONVERT(DATETIME, '23/01/2024', 103)),
('Printer Support', 'Problema stampante', 'In lavorazione', 3, 5, CONVERT(DATETIME, '24/01/2024', 103));

INSERT INTO Likes (UtenteID, ThreadID, MiPiace, DataLike) VALUES
(2, 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
(3, 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
(4, 2, 1, CONVERT(DATETIME, '16/01/2024', 103)),
(5, 3, 1, CONVERT(DATETIME, '17/01/2024', 103)),
(1, 4, 1, CONVERT(DATETIME, '18/01/2024', 103));
