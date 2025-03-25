--Aqui va el codigo para generar las tablas
CREATE DATABASE APP_ROBERTO
GO

USE APP_ROBERTO
GO

SET DATEFORMAT YMD
GO

CREATE TABLE TIPO_USUARIO (
    ID_TIPO_USUARIO INT IDENTITY(1,1),
    DESCRIPCION VARCHAR(255) NOT NULL,
	CONSTRAINT [PK_TIPO_USUARIO] PRIMARY KEY (ID_TIPO_USUARIO ASC)
);
GO

CREATE TABLE PRIORIDAD (
    ID_PRIORIDAD INT IDENTITY(1,1),
    DESCRIPCION VARCHAR(255) NOT NULL,
	CONSTRAINT [PK_PRIORIDAD] PRIMARY KEY (ID_PRIORIDAD ASC)
);
GO

CREATE TABLE ESTADO (
    ID_ESTADO INT IDENTITY(1,1),
    DESCRIPCION VARCHAR(255) NOT NULL,
	CONSTRAINT [PK_ESTADO] PRIMARY KEY (ID_ESTADO ASC)
);
GO

CREATE TABLE USUARIO (
    ID_USUARIO INT IDENTITY(1,1),
    NOMBRE VARCHAR(100) NOT NULL,
    CORREO_ELECTRONICO NVARCHAR(255) NOT NULL,
    CONTRASENA NVARCHAR(255) NOT NULL,
    FECHA_NACIMIENTO DATE NOT NULL,
    FOTO_PERFIL VARBINARY(MAX) NULL,
    CODIGO VARCHAR(6) NULL,
    DIRECCION VARCHAR(255) NULL,
    ID_TIPO_USUARIO INT NOT NULL,
	CONSTRAINT [PK_USUARIO] PRIMARY KEY (ID_USUARIO ASC),
    CONSTRAINT FK_USUARIO_TIPOUSUARIO FOREIGN KEY (ID_TIPO_USUARIO) REFERENCES TIPO_USUARIO(ID_TIPO_USUARIO),
	CONSTRAINT CHK_FC_NACIMIENTO CHECK (FECHA_NACIMIENTO > '1900-01-01')
);
GO

CREATE TABLE EVENTO (
    ID_EVENTO INT IDENTITY(1,1),
    TITULO VARCHAR(255) NOT NULL,
    DESCRIPCION VARCHAR(255) NULL,
    FECHA_HORA DATETIME NOT NULL,
    ID_PRIORIDAD INT NOT NULL,
    ID_USUARIO INT NOT NULL,
	CONSTRAINT [PK_EVENTO] PRIMARY KEY (ID_EVENTO ASC),
	CONSTRAINT FK_EVENTO_PRIORIDAD FOREIGN KEY (ID_PRIORIDAD) REFERENCES PRIORIDAD(ID_PRIORIDAD),
    CONSTRAINT FK_EVENTO_USUARIO FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO),
	CONSTRAINT CHK_FECHA_HORA CHECK (FECHA_HORA > GETDATE())
);
GO

CREATE TABLE JUEGO (
    ID_JUEGO INT IDENTITY(1,1),
    NOMBRE VARCHAR(255) NOT NULL,
    ID_USUARIO_CREADOR INT NOT NULL,
	CONSTRAINT [PK_JUEGO] PRIMARY KEY (ID_JUEGO ASC),
    CONSTRAINT FK_JUEGO_CREADOR FOREIGN KEY (ID_USUARIO_CREADOR) REFERENCES USUARIO(ID_USUARIO),
);
GO

CREATE TABLE JUEGO_PACIENTE(
    ID_JUEGO INT NOT NULL,
    ID_PACIENTE INT NOT NULL,
    CONSTRAINT PK_JUEGO_PACIENTE PRIMARY KEY (ID_JUEGO, ID_PACIENTE),
    CONSTRAINT FK_JUEGO_JUEGO_PACIENTE FOREIGN KEY (ID_JUEGO) REFERENCES JUEGO(ID_JUEGO),
    CONSTRAINT FK_PACIENTE_JUEGO_PACIENTE FOREIGN KEY (ID_PACIENTE) REFERENCES USUARIO(ID_USUARIO)
);
GO

CREATE TABLE IMAGEN (
    ID_IMAGEN INT IDENTITY(1,1),
    BINARIO_FOTO VARBINARY(MAX) NOT NULL,
	CONSTRAINT [PK_IMAGEN] PRIMARY KEY (ID_IMAGEN ASC)
);
GO

CREATE TABLE PREGUNTA (
    ID_PREGUNTA INT IDENTITY(1,1),
    DESCRIPCION VARCHAR(MAX) NOT NULL,
    ID_JUEGO INT NOT NULL,
    ID_IMAGEN INT NOT NULL,
	CONSTRAINT [PK_PREGUNTA] PRIMARY KEY (ID_PREGUNTA ASC),
    CONSTRAINT FK_PREGUNTA_JUEGO FOREIGN KEY (ID_JUEGO) REFERENCES JUEGO(ID_JUEGO),
    CONSTRAINT FK_PREGUNTA_IMAGEN FOREIGN KEY (ID_IMAGEN) REFERENCES IMAGEN(ID_IMAGEN)
);
GO

CREATE TABLE OPCION (
    ID_OPCION INT IDENTITY(1,1),
    DESCRIPCION VARCHAR(255) NOT NULL,
    CONDICION BIT NOT NULL,
    ID_PREGUNTA INT NOT NULL,
	CONSTRAINT [PK_OPCION] PRIMARY KEY (ID_OPCION ASC),
    CONSTRAINT FK_OPCION_PREGUNTA FOREIGN KEY (ID_PREGUNTA) REFERENCES PREGUNTA(ID_PREGUNTA)
);
GO

CREATE TABLE EVENTO_USUARIO (
    ID_EVENTO INT NOT NULL,
    ID_USUARIO INT NOT NULL,
    ID_ESTADO INT NOT NULL,
    CONSTRAINT [PK_EVENTO_USUARIO] PRIMARY KEY (ID_EVENTO, ID_USUARIO),
    CONSTRAINT FK_EVENTO_USUARIO_EVENTO FOREIGN KEY (ID_EVENTO) REFERENCES EVENTO(ID_EVENTO),
    CONSTRAINT FK_EVENTO_USUARIO_USUARIO FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO),
    CONSTRAINT FK_EVENTO_USUARIO_ESTADO FOREIGN KEY (ID_ESTADO) REFERENCES ESTADO(ID_ESTADO)
);
GO

CREATE TABLE MENSAJE (
    ID_MENSAJE INT IDENTITY(1,1),
    CONTENIDO VARCHAR(255)NOT NULL,
    FECHA_ENVIADO DATETIME DEFAULT GETDATE(),
    FECHA_RECIBIDO DATETIME NULL,
    ID_USUARIO_CUIDADOR INT NOT NULL,
    ID_USUARIO_PACIENTE INT NOT NULL,
    ID_ESTADO INT NOT NULL,
	CONSTRAINT [PK_MENSAJE] PRIMARY KEY (ID_MENSAJE ASC),
    CONSTRAINT FK_MENSAJE_USUARIO_PACIENTE FOREIGN KEY (ID_USUARIO_PACIENTE) REFERENCES USUARIO(ID_USUARIO),
    CONSTRAINT FK_MENSAJE_USUARIO_CUIDADOR FOREIGN KEY (ID_USUARIO_CUIDADOR) REFERENCES USUARIO(ID_USUARIO),
    CONSTRAINT FK_MENSAJE_ESTADO FOREIGN KEY (ID_ESTADO) REFERENCES ESTADO(ID_ESTADO)
);
GO


CREATE TABLE CUIDADOR_PACIENTE (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_USUARIO_CUIDADOR INT NOT NULL,
    ID_USUARIO_PACIENTE INT NOT NULL,
    FEC_INICIO DATE DEFAULT GETDATE(),
    FEC_FIN DATE NULL,
    CONSTRAINT FK_CUIDADOR_PACIENTE_USUARIO_CUIDADOR 
    FOREIGN KEY (ID_USUARIO_CUIDADOR) REFERENCES USUARIO(ID_USUARIO),
    CONSTRAINT FK_CUIDADOR_PACIENTE_USUARIO_PACIENTE 
    FOREIGN KEY (ID_USUARIO_PACIENTE) REFERENCES USUARIO(ID_USUARIO)
);
GO

CREATE TABLE PING (
    ID_PING INT  IDENTITY(1,1),
    CODIGO VARCHAR(6) NOT NULL,
    FECHA DATETIME DEFAULT GETDATE(),
    ESTADO BIT  NOT NULL,
    ID_USUARIO INT  NOT NULL,
	CONSTRAINT [PK_PING] PRIMARY KEY (ID_PING ASC),
    CONSTRAINT FK_PING_USUARIO FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO)
);
GO

CREATE TABLE PUNTAJE (
    ID_PUNTAJE INT  IDENTITY(1,1),
    FECHA_HORA DATETIME  DEFAULT GETDATE(),
    PUNTAJE INT  NOT NULL,
    ID_JUEGO INT  NOT NULL,
    ID_USUARIO INT  NOT NULL,
	CONSTRAINT [PK_PUNTAJE] PRIMARY KEY (ID_PUNTAJE ASC),
    CONSTRAINT FK_PUNTAJE_JUEGO FOREIGN KEY (ID_JUEGO) REFERENCES JUEGO(ID_JUEGO),
    CONSTRAINT FK_PUNTAJE_USUARIO FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO)
);
GO

CREATE TABLE SESION (
    ID_SESION INT IDENTITY(1,1) NOT NULL,
    TOKEN NVARCHAR(255) NOT NULL, 
    ID_USUARIO INT NOT NULL,
    ORIGEN NVARCHAR(MAX) NOT NULL,
    FECHA_INICIO DATETIME NOT NULL DEFAULT GETDATE(), 
    FECHA_EXPIRACION DATETIME NOT NULL, 
    FECHA_FINAL DATETIME NULL,
    CONSTRAINT PK_SESION PRIMARY KEY (ID_SESION),
    CONSTRAINT FK_SESION_USUARIO FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO)
);
GO


CREATE TABLE ERROR_CATALOG (
    NUM_ERROR INT PRIMARY KEY,           
    CODE_ERROR NVARCHAR(50) NOT NULL,     
    DESCRIPTION_ERROR NVARCHAR(MAX) NOT NULL 
);
GO