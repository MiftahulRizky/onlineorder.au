CREATE TABLE OrderHeaders (
	Id INT NOT NULL,
	JoNumber nvarchar(100),
	UserId uniqueidentifier,
	StoreId nvarchar(100),
	OrderNo nvarchar(200),
	OrderCust nvarchar(200),
	Delivery nvarchar(50),
	Note nvarchar(max),
	Address nvarchar(max),
	Suburb nvarchar(max),
	States nvarchar(255),
	PostCode nvarchar(255),
	Phone nvarchar(255),
	Email nvarchar(255),
	QuoteGST nvarchar(255),
	QuoteDisc INT,
	QuoteInstall INT,
	QuoteMeasure INT,
	Status nvarchar(50),
	StatusDescription nvarchar(max),
	CreatedDate DATE,
	SubmittedDate DATE,
	CanceledDate DATE,
	CompletedDate DATE,
	Active INT
);
CREATE INDEX idx_JoNumber ON OrderHeaders (JoNumber);
CREATE INDEX idx_Id ON OrderHeaders (Id);