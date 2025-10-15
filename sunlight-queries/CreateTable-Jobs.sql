CREATE TABLE Jobs (
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	JoNumber VARCHAR(100) NOT NULL,
	HeaderId INT NOT NULL,
	TotalQty INT,
	CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE INDEX idx_JoNumber ON Jobs (JoNumber);
CREATE INDEX idx_HeaderId ON Jobs (HeaderId);
