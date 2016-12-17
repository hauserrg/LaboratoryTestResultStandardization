IF OBJECT_ID('{0}','U') IS NOT NULL DROP TABLE {0}

CREATE TABLE {0}(
	Id int identity(1,1),
		PRIMARY KEY(Id),		
	[Loinc] [varchar](50) NULL,
	[Result] [varchar](254) NULL,
	[LoincScale] [varchar](254) NULL,
	[MappedYN] [varchar](1) NULL,
	[MapFunc] [varchar](50) NULL,
	[Inequality] [varchar](2) NULL,
	[Number] [varchar](100) NULL,
	[AfterDecimal] [int] NULL,
	[Field1] [varchar](100) NULL,
	[Field2] [varchar](100) NULL,
	[General] [varchar](30) NULL,
	[Pretty] [varchar](100) NULL,
) 
