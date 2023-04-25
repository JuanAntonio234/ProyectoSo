DROP DATABASE IF EXISTS bd;
CREATE DATABASE bd;
USE bd;
CREATE TABLE JUGADOR(
ID INT NOT NULL,
NOMBRE VARCHAR(50),
PASSWORD VARCHAR(15) NOT NULL,
PRIMARY KEY(ID)
)ENGINE = InnoDB;
CREATE TABLE PARTIDA(
ID INT NOT NULL,
DURACION INT,
GANADOR VARCHAR(50),
FECHA VARCHAR(50),
PRIMARY KEY(ID)
)ENGINE = InnoDB;
Create Table Lista_Partidas(
PARTIDASTOTALES INTEGER NOT NULL,
MVP VARCHAR(20) NOT NULL,
TIEMPOMEDIO INTEGER NOT NULL
FOREIGN KEY (MVP) REFERENCES JUGADOR(ID)
FOREIGN KEY (TIEMPOMEDIO) REFERENCES PARTIDA(DURACION)
idJ VARCHAR(10),
idP VARCHAR(10),
FOREIGN KEY (idJ) REFERENCES JUGADOR(ID),
FOREIGN KEY (idP) REFERENCES PARTIDA(ID)
)ENGINE = InnoDB;

INSERT INTO JUGADOR VALUES("123S","Juan","12D");
INSERT INTO JUGADOR VALUES("783P","Pere","85f");
INSERT INTO JUGADOR VALUES("756J","Oriol","8e9");
<<<<<<< HEAD
INSERT INTO PARTIDA VALUES(1,3,"Juan","2/3/23");
INSERT INTO PARTIDA VALUES(2,5,"Pere","15/2/23");
INSERT INTO PARTIDA VALUES(3,3,"Oriol","18/2/23");
=======

INSERT INTO PARTIDA VALUES(1,3,"Juan","2/3/23");
INSERT INTO PARTIDA VALUES(2,5,"Pere","15/2/23");
INSERT INTO PARTIDA VALUES(3,3,"Oriol","18/2/23");

>>>>>>> ee3f62f09b54bd58f9a7c1146ac7f2c042524dfd
INSERT INTO Lista_Partidas VALUES (6, 'Juan', 120);
INSERT INTO Lista_Partidas VALUES (5, 'Pere', 240);
INSERT INTO Lista_Partidas VALUES (4, 'Oriol', 480);
