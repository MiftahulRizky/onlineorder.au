CREATE TABLE HardwareKitsNew (
	Id uniqueidentifier   PRIMARY KEY NOT NULL,
	SoeId INT NOT NULL,
	DesignId uniqueidentifier  NOT NULL,
	BlindId uniqueidentifier  NOT NULL,
	Name nvarchar(max),
	BracketType nvarchar(max),
	TubeType nvarchar(max),
	ControlType nvarchar(max),
	ColourType nvarchar(max),
	Description nvarchar(max),
	Active BIT
);