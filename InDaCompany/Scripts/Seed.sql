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
('HR', 'Risorse Umane', NULL),
('Supporto Tecnico', 'Assistenza IT', 'Sviluppo');

INSERT INTO Post (Testo, DataCreazione, AutoreID) VALUES
('Benvenuti nel nuovo sistema aziendale!', CONVERT(DATETIME, '10/01/2024', 103), 1),
('Meeting di team previsto per venerdì', CONVERT(DATETIME, '11/01/2024', 103), 2),
('Nuovo progetto in partenza!', CONVERT(DATETIME, '12/01/2024', 103), 3),
('Risultati del Q4 2024', CONVERT(DATETIME, '13/01/2024', 103), 4),
('Aggiornamento procedure interne', CONVERT(DATETIME, '14/01/2024', 103), 5);

INSERT INTO ThreadForum (Titolo, ForumID, AutoreID, DataCreazione) VALUES
('Benvenuto nel forum', 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
('Nuova release v2.0', 2, 2, CONVERT(DATETIME, '16/01/2024', 103)),
('Campagna Q1 2025', 3, 4, CONVERT(DATETIME, '17/01/2024', 103)),
('Policy Smart Working', 4, 5, CONVERT(DATETIME, '18/01/2024', 103)),
('Manutenzione servers', 5, 3, CONVERT(DATETIME, '19/01/2024', 103));

INSERT INTO MessaggiThread (ThreadID, AutoreID, Testo, DataCreazione) VALUES
(1, 1, 'Benvenuti a tutti nel nuovo forum aziendale', CONVERT(DATETIME, '15/01/2024', 103)),
(2, 2, 'Ecco le novità della versione 2.0', CONVERT(DATETIME, '16/01/2024', 103)),
(2, 3, 'Ottimo lavoro!', CONVERT(DATETIME, '16/01/2024', 103)),
(3, 4, 'Obiettivi Q1 2025', CONVERT(DATETIME, '17/01/2024', 103)),
(4, 5, 'Nuove linee guida smart working', CONVERT(DATETIME, '18/01/2024', 103));

INSERT INTO Ticket (Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura) VALUES
('Supporto Email', 'Problema accesso email', 'Aperto', 3, 1, CONVERT(DATETIME, '20/01/2024', 103)),
('Hardware Request', 'Richiesta nuovo laptop', 'In lavorazione', 4, 2, CONVERT(DATETIME, '21/01/2024', 103)),
('Network Issue', 'Configurazione VPN', 'Chiuso', 5, 1, CONVERT(DATETIME, '22/01/2024', 103)),
('Software Update', 'Aggiornamento software', 'Aperto', 2, 3, CONVERT(DATETIME, '23/01/2024', 103)),
('Printer Support', 'Problema stampante', 'In lavorazione', 3, 5, CONVERT(DATETIME, '24/01/2024', 103));

INSERT INTO Likes (UtenteID, PostID, MiPiace) VALUES
(2, 1, 1),
(3, 1, 1),
(4, 2, 1),
(5, 3, 1),
(1, 4, 1);
