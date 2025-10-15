CREATE TABLE MailPushOrderDraft (
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	OrdId INT NOT NULL,
	Email VARCHAR(100) NOT NULL,
	DraftDate DATE NOT NULL,
	CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE INDEX idx_Email ON MailPushOrderDraft (Email);
CREATE INDEX idx_OrdId ON MailPushOrderDraft (OrdId);
CREATE INDEX idx_DraftDate ON MailPushOrderDraft (DraftDate);