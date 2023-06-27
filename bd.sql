DROP DATABASE IF EXISTS M6_BBDD;
CREATE DATABASE M6_BBDD;
USE M6_BBDD;

CREATE TABLE JUGADOR(
    NOMBRE VARCHAR (50) PRIMARY KEY NOT NULL,
    PASSWORD VARCHAR (15) NOT NULL,
    PARTIDAS_GANADAS INT
) ENGINE = InnoDB;

CREATE TABLE PARTIDA(
    Usuario1 VARCHAR(30) NOT NULL,
    Usuario2 VARCHAR(40) NOT NULL,
    ID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY(ID),
    FOREIGN KEY (Usuario1) REFERENCES JUGADOR(NOMBRE),
    FOREIGN KEY (Usuario2) REFERENCES JUGADOR(NOMBRE)
) ENGINE = InnoDB;

INSERT INTO JUGADOR VALUES("Pere","12D",5);
INSERT INTO JUGADOR VALUES("Jose","85f",10);
INSERT INTO JUGADOR VALUES("Maria","38F",8);
INSERT INTO JUGADOR VALUES("Marta","12gh",2);

INSERT INTO PARTIDA VALUES("Pere","Maria",123);
INSERT INTO PARTIDA VALUES("Marta","Maria",783);
INSERT INTO PARTIDA VALUES("Jose","Pere",859);
	
