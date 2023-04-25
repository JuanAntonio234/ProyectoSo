#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

//Estructura necessaria para login excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

typedef struct {
	char nombre[20];
	int socket;
}Conectado;

typedef struct {
	Conectado conectados [100];
	int num;
} ListaConectados;

ListaConectados lista;

int sockets[100];

void *atenderCliente(void *socket)
{
	int sock_conn, ret;
	int *s;
	s = (int *) socket;
	sock_conn = *s;	
	
	char peticion[512];
	char respuesta[512];
	char contestacion[512];
	char contrasena[20];
	char nombre[25];
	char fecha[11];
	char conectados[300];
	int conexion = 0;
	int r;
	int i;
	
	while(conexion == 0)
	{
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		peticion[ret]='\0';
		int error = 1;
		int codigo = 9999;
		char *p;
		
		if(strlen(peticion) < 2){
			error = 0;
		}
		
		if (strcmp(peticion, "") != 0){
			printf ("Peticion: %s\n",peticion);
			p = strtok(peticion, "-");
			codigo = atoi(p);
		}	
		
		if (error == 0){
			codigo = 9999;
		}
		
		if(codigo == 0) //LOGIN
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			strcpy(nombre, p);
			printf("Codigo de conexion: %d\n", r);
			p = strtok(NULL, "-");
			strcpy(contrasena, p);
			printf("Codigo: %d, Nombre: %s y Contrase￱a: %s\n", codigo, nombre, contrasena);
			Login(nombre, contrasena, contestacion);
			if(strcmp (contestacion, "Error") != 0)
				r = Conectar(&lista, nombre, socket);
			DameConectados(&lista, conectados);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			pthread_mutex_unlock(&mutex);
		}
		
		else if(codigo == 4) //REGISTRAR
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			strcpy(nombre, p);
			p = strtok(NULL, "-");
			strcpy(contrasena, p);
			printf("Codigo: %d, Fecha: %s y Nombre: %s\n", codigo, contrasena, nombre);
			Registrar(nombre, contrasena, contestacion);
			if(strcmp (contestacion, "Error") != 0)
				r = Conectar(&lista, nombre, socket);
			DameConectados(&lista, conectados);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			pthread_mutex_unlock(&mutex);
		}
		
		else if(codigo == 5) //DESCONECTAR
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			strcpy(nombre, p);
			conexion = 1;
			printf("Desconectando a %s\n", nombre);
			r = Desconectar(&lista, nombre);
			printf("Codigo de desconexion: %d\n", r);
			DameConectados(&lista, conectados);
			close(sock_conn);
			pthread_mutex_unlock(&mutex);
		}
		
		else if(codigo == 1)
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			strcpy(fecha, p);
			printf("Codigo: %d, Fecha: %s\n", codigo, fecha);
			NombreJugadorDuracionMayor3(contestacion);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			pthread_mutex_unlock(&mutex);
		}
		
		else if(codigo == 2)
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			strcpy(fecha, p);		
			printf("Codigo: %d, Fecha: %s\n", codigo, fecha);
			JugadoresJugadoMismoDiaPere(contestacion);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			pthread_mutex_unlock(&mutex);
		}
		
		/*else if(codigo == 3)
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			strcpy(fecha, p);		
			printf("Codigo: %d, Fecha: %s\n", codigo, fecha);
			DameID(nombre, contestacion);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			pthread_mutex_unlock(&mutex);
		}*/
		
		else if (codigo == 1 || codigo == 4 || codigo == 5)
		{
			pthread_mutex_lock(&mutex);
			int j;
			DameConectados(&lista, conectados);
			sprintf(respuesta, "6-%s", conectados);
			printf("%s\n", respuesta);
			pthread_mutex_unlock(&mutex);
			for (j = 0; j < lista.num; j++)
			{
				write (sockets[j],respuesta,strlen(respuesta));
			}
		}
		else{
			printf("Peticion no existe\n");
		}
	}		
}	

int main(int argc, char *argv[]) 
{
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	pthread_t thread;
	lista.num = 0;
	int conexion = 0;
	int puerto = 5050;
	int i = 0;
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error al crear socket\n");
	
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY); 
	serv_adr.sin_port = htons(puerto);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error en el bind\n");
	
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen\n");
	int rc;
	while(conexion == 0)
	{
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida\n");
		
		sockets[i] = sock_conn;
		
		rc = pthread_create (&thread, NULL, *atenderCliente , &sockets[i]);
		printf("Codigo %d = %s\n", rc, strerror(rc));
		
		i++;
	}
	return 0;
}

void DameConectados(ListaConectados * lista, char conectados[300])
{
	int i;
	sprintf (conectados, "%d", lista->num);
	for (i = 0; i < lista->num; i++)
	{
		sprintf(conectados, "%s-%s", conectados, lista->conectados[i].nombre);
	}
}

int DamePos(ListaConectados * lista, char nombre[20])
{
	int i = 0;
	int encontrado = 0;
	printf("Lista nombre : %s\n", lista->conectados[i].nombre);
	while ((i<lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
			encontrado = 1;
		if (!encontrado)
			i++;
	}
	if (encontrado)
		return i;
	else 
		return -1;
}

int Desconectar(ListaConectados * lista, char nombre[20])
{
	int pos = DamePos(lista, nombre);
	if (pos == -1)
		return -1;
	else
	{
		int i;
		for (i = pos; i < lista->num-1; i++)
		{
			lista->conectados[i] = lista->conectados[i+1];
		}
		lista->num--;
		return 0;
	}
}

int Conectar(ListaConectados *lista, char nombre[20], int socket)
{
	if (lista->num == 100)
		return -1;
	else
	{
		strcpy(lista->conectados[lista->num].nombre, nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num++;
		printf("Lista numero : %d\n", lista->num);
		return 0;
	}
}


	
void Login(char nombre[25], char contrasena[25], char respuesta[512])
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	int login;
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Stick Fight Game", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use BasedeDatos;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT ID from JUGADOR where NOMBRE = '");
	strcat (consulta, nombre);
	strcat (consulta,"' and PASSWORD = '");
	strcat (consulta, contrasena);
	strcat (consulta,"';");
	err=mysql_query (conn, consulta); 
	if (err!=0) 
	{
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("Nombre o contrase￱a incorrectos\n");
		login = -1;
		sprintf(respuesta, "Error");
	}	
	
	else
	{
		printf ("Ha iniciado sesi￳n el usuario con id: %s\n", row[0]);
		login = 0;
		sprintf(respuesta, "0-%d", login);
	}
}

void Registrar(char nombre[25], char contrasena[25], char respuesta[512])
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char consulta[500];
	int registro;
	int numJ;
	
	conn = mysql_init(NULL);
	if (conn==NULL) 
	{
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Stick Fight Game", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use BasedeDatos;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query (conn, "SELECT count(ID) from JUGADOR;"); 
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	numJ = atoi(row[0]);
	numJ++;
	printf("Este es el numero: %d\n", numJ);
	
	int numero = numJ;
	sprintf(consulta, "insert into JUGADOR values (%d,'%s','%s');", numJ, nombre, contrasena);
	err=mysql_query (conn, consulta); 
	if (err!=0) 
	{
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
		sprintf(respuesta, "Error");
	}
	
	else
	{
		sprintf(respuesta, "4-0");
	}
}

void NombreJugadorDuracionMayor3(char respuesta[512])
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char nombres[50];
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Stick Fight Game", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use BasedeDatos;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT JUGADOR.NOMBRE from JUGADOR, PARTIDA where PARTIDA.DURACION > 3 and JUGADOR.ID = PARTIDA.GANADOR");
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "Error");
	}
	
	else
	{
		strcpy(nombres, row[0]);
		row = mysql_fetch_row (resultado);
		while(row!=NULL)
		{
			strcat (nombres, "-");
			strcat (nombres, row[0]);
			row = mysql_fetch_row (resultado);
		}
		strcat (nombres, "\n");
		printf(nombres);
		sprintf(respuesta, "1-%s", nombres);
	}
}

void JugadoresJugadoMismoDiaPere(char respuesta[512])
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char nombres[50];
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Stick Fight Game", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use BasedeDatos;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT JUGADOR.NOMBRE from JUGADOR, PARTIDA, Lista_Partidas where JUGADOR.ID = Lista_Partidas.idJ and PARTIDA.ID = Lista_Partidas.idP and PARTIDA.FECHA = (SELECT FECHA from PARTIDA, JUGADOR, Lista_Partidas where JUGADOR.NOMBRE = 'Pere' and JUGADOR.ID = Lista_Partidas.MVP and PARTIDA.ID = Lista_Partidas.idP");
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "Error");
	}
	
	else
	{
		strcpy(nombres, row[0]);
		row = mysql_fetch_row (resultado);
		while(row!=NULL)
		{
			strcat (nombres, "-");
			strcat (nombres, row[0]);
			row = mysql_fetch_row (resultado);
		}
		strcat (nombres, "\n");
		printf(nombres);
		sprintf(respuesta, "2-%s", nombres);
	}
}

/*void DameID(char nombre[50], char respuesta[512})
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char ID[10];
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Stick Fight Game", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use BasedeDatos;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT ID FROM JUGADOR WHERE NOMBRE = '"); 
	strcat (consulta, nombre);
	strcat (consulta,"'");
	err=mysql_query (conn, consulta); 
	
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "Error");
	}
	
	else
	{
		strcpy(ID, row[0]);
		sprintf(respuesta, "3-%d", ID);
	}
}*/
