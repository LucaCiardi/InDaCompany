INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, Ruolo, Team, FotoProfilo, DataCreazione) VALUES
('Mario', 'Rossi', 'mario.rossi@azienda.com', 'hash123456789', 'Admin', NULL, NULL, CONVERT(DATETIME, '01/01/2024', 103)),
('Laura', 'Bianchi', 'laura.bianchi@azienda.com', 'hash987654321', 'Manager', 'Sviluppo', NULL, CONVERT(DATETIME, '02/01/2024', 103)),
('Giuseppe', 'Verdi', 'giuseppe.verdi@azienda.com', 'hash456789123', 'Dipendente', 'Sviluppo', NULL, CONVERT(DATETIME, '03/01/2024', 103)),
('Anna', 'Neri', 'anna.neri@azienda.com', 'hash789123456', 'Dipendente', 'Marketing', NULL, CONVERT(DATETIME, '04/01/2024', 103)),
('Marco', 'Ferrari', 'marco.ferrari@azienda.com', 'hash321654987', 'Manager', 'Marketing', NULL, CONVERT(DATETIME, '05/01/2024', 103));

INSERT INTO Forum (Nome, Descrizione, Team) VALUES
('Generale', 'Forum generale aziendale', NULL),
('Sviluppo Software', 'Discussioni tecniche', 'Sviluppo'),
('Marketing Strategico', 'Strategie e campagne', 'Marketing'),
('HR', 'Risorse Umane', 'HR'),
('Supporto Tecnico', 'Assistenza IT', 'Sviluppo');

INSERT INTO ThreadForum (Titolo, Testo, Immagine, ForumID, AutoreID, DataCreazione) VALUES
('Benvenuto nel forum', 'Benvenuti nel nuovo sistema aziendale!', NULL, 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
('Nuova release v2.0', 'Meeting di team previsto per venerdì', NULL, 2, 2, CONVERT(DATETIME, '16/01/2024', 103)),
('Campagna Q1 2025', 'Risultati del Q4 2024', NULL, 3, 4, CONVERT(DATETIME, '17/01/2024', 103)),
('Policy Smart Working', 'Aggiornamento procedure interne', NULL, 4, 5, CONVERT(DATETIME, '18/01/2024', 103)),
('Manutenzione servers', 'RFC server di sviluppo - Week 9', NULL, 5, 3, CONVERT(DATETIME, '19/01/2024', 103));

INSERT INTO MessaggiThread (ThreadID, AutoreID, Testo, DataCreazione) VALUES
(1, 1, 'Benvenuti a tutti nel nuovo forum aziendale', CONVERT(DATETIME, '15/01/2024', 103)),
(2, 3, 'Ecco le novità della versione 2.0', CONVERT(DATETIME, '16/01/2024', 103)),
(3, 3, 'Ottimo lavoro!', CONVERT(DATETIME, '16/01/2024', 103)),
(3, 4, 'Obiettivi Q1 2025', CONVERT(DATETIME, '17/01/2024', 103)),
(4, 5, 'Nuove linee guida smart working', CONVERT(DATETIME, '18/01/2024', 103));

INSERT INTO Ticket (Titolo, Descrizione, Soluzione, Stato, CreatoDaID, AssegnatoAID, DataApertura, DataChiusura) VALUES
('Supporto Email', 'Problema accesso email', NULL, 'Aperto', 3, 1, CONVERT(DATETIME, '20/01/2024', 103), NULL),
('Hardware Request', 'Richiesta nuovo laptop', NULL, 'In lavorazione', 4, 2, CONVERT(DATETIME, '21/01/2024', 103), NULL),
('Network Issue', 'Configurazione VPN', 'Configurazione completata e testata con successo', 'Chiuso', 5, 1, CONVERT(DATETIME, '22/01/2024', 103), CONVERT(DATETIME, '23/01/2024', 103)),
('Software Update', 'Aggiornamento software', NULL, 'Aperto', 2, 3, CONVERT(DATETIME, '23/01/2024', 103), NULL),
('Printer Support', 'Problema stampante', NULL, 'In lavorazione', 3, 5, CONVERT(DATETIME, '24/01/2024', 103), NULL);

INSERT INTO Likes (UtenteID, ThreadID, MiPiace, DataLike) VALUES
(2, 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
(3, 1, 1, CONVERT(DATETIME, '15/01/2024', 103)),
(4, 2, 1, CONVERT(DATETIME, '16/01/2024', 103)),
(5, 3, 1, CONVERT(DATETIME, '17/01/2024', 103)),
(1, 4, 1, CONVERT(DATETIME, '18/01/2024', 103));
