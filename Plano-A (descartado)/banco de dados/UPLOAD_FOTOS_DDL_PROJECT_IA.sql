USE PROJECT_IA;
GO

CREATE TABLE CADCLIENTE(
	idCliente INT PRIMARY KEY IDENTITY(1,1),
	nome VARCHAR(20) NOT NULL UNIQUE,
	email VARCHAR(75) NOT NULL,
	senha VARCHAR (8) NOT NULL
)
GO