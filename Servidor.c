#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <pthread.h>
#include <mysql.h>
#include <arpa/inet.h>

//Estructura necessaria para login excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
#define MAX_CLIENTS 4

typedef struct {
	char nombre[20];
	int socket;
}Conectado;

typedef struct {
	Conectado conectados [100];
	int num;
} ListaConectados;
	
	
	typedef struct{
	char jugado[4][15];
	int socket[5];
	int ocupado;
}Partida;
typedef struct{
	Partida partida[100];
	int num;
}listPartidas;
typedef struct{
	int clientSocket;
	float positionX;
	float positionY;
} Jugador;

Jugador jugadores[10];
ListaConectados lista;
listPartidas listapartidas;

int i=0;
int sockets[100];

int Conectar(ListaConectados *lista, char nombre[20], int socket);

void DameConectados(ListaConectados *lista, char conectados[300])//envia lista de conectados
{
	int i;
	sprintf (conectados, "%d", lista->num);
	for (i = 0; i < lista->num; i++)
	{
		sprintf(conectados, "%s-%s", conectados, lista->conectados[i].nombre);
	}
}

int DamePos(ListaConectados *lista, char nombre[20])
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

int Desconectar(ListaConectados *lista, char nombre[20])//desconecta de la lista de conectados al usuario
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

int Conectar(ListaConectados *lista, char nombre[20], int socket)//a￱ade a la lista de conectados al usuario
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//enviar el id del jugador que se acaba e registar/iicia sesion,por lo tanto el servidro debe envia r la id




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
		printf ("Error al crear la conexion1: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion2: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use bd;");
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
		printf ("Nombre o contrasena incorrectos\n");
		login = -1;
		sprintf(respuesta, "Error");
	}
	
	else
	{
		printf ("Ha iniciado sesion el usuario con id: %s\n", row[0]);
		login = 0;
		sprintf(respuesta, "0-%d", login);
	}
// Se libera el resultado de la consulta
	mysql_free_result(resultado);
	
	mysql_close(conn);
}
int verificarID(MYSQL *conn, int id){
	char consulta[500];
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int numFilas=0;
	
	sprintf(consulta, "SELECT COUNT(ID) FROM JUGADOR WHERE ID = %d;",id);
	if(mysql_query(conn,consulta)!=0){
		printf("Error al consultar los datos de la base de datos: %u %s\n",
			   mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	
	resultado =mysql_store_result(conn);
	if(resultado==NULL){
		printf("Error al almacenar el resultado de la consulta: %u %s\n",
			   mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	row =mysql_fetch_row(resultado);
	numFilas=atoi(row[0]);
	
	mysql_free_result(resultado);
	
	return numFilas;
}
void Registrar(int id,char nombre[25], char contrasena[25], char respuesta[512])
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char consulta[500];
	int numJ;
	
	conn = mysql_init(NULL);
	if (conn==NULL)
	{
		printf ("Error al crear la conexion3: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion4: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use bd;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	if(verificarID(conn,id)>0){
		sprintf(respuesta, "Error: La ID ya existe");
	}else{
	//numero total de registros de jugadores en la lista
	err=mysql_query (conn, "select count(ID) from JUGADOR;");
	if (err != 0)
	{
		printf("Error al consultar datos de la base: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	resultado = mysql_store_result (conn);
	if (resultado == NULL)
	{
		printf("Error al almacenar el resultado de la consulta: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	row = mysql_fetch_row (resultado);
	
	numJ = atoi(row[0]);
	numJ++;
	printf("Es la persona numero: %d\n", numJ);
	
	sprintf(consulta, "INSERT INTO JUGADOR (ID, NOMBRE, PASSWORD, PARTIDAS_GANADAS, PARTIDAS_PERDIDAS) VALUES (%d, '%s', '%s', 0, 0);", id, nombre, contrasena);
	err=mysql_query (conn, consulta);
	if (err!=0)
	{
		printf("Error al insertar datos en la base: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
		sprintf(respuesta, "Error");
	}
	
	else
	{
		sprintf(respuesta, "1-0");
	}
	
	}
	mysql_free_result(resultado);
	mysql_close(conn);
}

void NombreJugadorDuracionMayor3(char respuesta[512])
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char nombres[50]="";
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion5: %d %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion6: %d %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use bd;");
	if (err!=0)
	{
		printf ("Error al acceder a la base de datos %d %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT JUGADOR.NOMBRE from JUGADOR, PARTIDA where PARTIDA.DURACION > 3 and JUGADOR.ID = PARTIDA.GANADOR");
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %d %s\n",
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
		printf("%s",nombres);
		sprintf(respuesta, "3-%s", nombres);
	}
	mysql_free_result(resultado); // Liberar la memoria asignada al resultado
	mysql_close(conn); // cerrar conexion
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
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion7: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use bd;");
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
		printf("%s",nombres);
		sprintf(respuesta, "2-%s", nombres);
	}
}

void DameID(char nombre[50], char respuesta[512])
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
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, "use bd;");
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
		sprintf(respuesta, "3-%s", ID);
	}
	mysql_free_result(resultado);
	mysql_close(conn);
}

void EnviarPosicionJugador(int clientSocket) 
{
	char buffer[1024] = {0};
	int i;
	for (i = 0; i < 3 ; i++) {
		if (jugadores[i].clientSocket != 0 && jugadores[i].clientSocket != clientSocket) {
			sprintf(buffer, "%f,%f", jugadores[i].positionX, jugadores[i].positionY);
			send(jugadores[i].clientSocket, buffer, strlen(buffer), 0);
		}
	}
}

void *atenderCliente(void *socket)
{
	int jugadorPosicionX = 0;
	int jugadorPosicionY = 0;
	int jugadorIndex = -1;
	
	int sock_conn, ret;
	int *s;
	s = (int *) socket;
	int temp_sock = *s; // Variable temporal para desreferenciar el puntero
	sock_conn = temp_sock;
	
	char peticion[512];
	char respuesta[512];
	char contestacion[512];
	char contrasena[20];
	char nombre[25];
	char fecha[11];
	int id;
	char numero;
	
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
		printf ("Peticion: %s\n",peticion);
		char *p = strtok(peticion, "-");
		codigo = atoi(p);
		
		
		if(codigo == 0) //LOGIN
		{
				p = strtok(NULL, "-");
					strcpy(nombre, p);
					p = strtok(NULL, "-");
						strcpy(contrasena, p);
						printf("Codigo: %d, Nombre: %s y Contrase￱a: %s\n",codigo, nombre, contrasena);
						Login(nombre, contrasena, contestacion);
						pthread_mutex_lock(&mutex);
						if(strcmp (contestacion, "Error") != 0)
						r = Conectar(&lista, nombre, socket);
						DameConectados(&lista, conectados);
						sprintf(respuesta, "%s", contestacion);
					write (sock_conn,respuesta,strlen(respuesta));
			
			pthread_mutex_unlock(&mutex);
		}
		else if(codigo == 1) //REGISTRAR
		{
			p = strtok(NULL, "-");
			printf("%s\n", p);
			id=atoi(p);
			p = strtok(NULL, "-");
			strcpy(nombre, p);
			printf("%s\n", nombre);
			p = strtok(NULL, "-");
			printf("%s\n", p);
			strcpy(contrasena, p);
			printf("Codigo: %d, Nombre: %s y Contrase￱a: %s\n", codigo, nombre, contrasena);
			Registrar(id,nombre, contrasena, contestacion);
			pthread_mutex_lock(&mutex);
			if(strcmp (contestacion, "Error") != 0)
				r = Conectar(&lista, nombre,(void*) socket);
			DameConectados(&lista, conectados);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));			
			pthread_mutex_unlock(&mutex);
		}
		else if(codigo == 2)//Consulta (numero de consulta)
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
		else if(codigo == 3)//Consulta (numero de consulta)
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
		else if(codigo==6)//actualiza posicion jugador
		{
			pthread_mutex_lock(&mutex);
			p=strtok(NULL,"-");
			int nuevaPosicionX=atoi(p);
			p=strtok(NULL,"-");
			int nuevaPosicionY=atoi(p);
			printf("Actualizar posicion: X=%d, Y=%d\n",nuevaPosicionX,nuevaPosicionY);
			
			//Actualizar la posicion del jugador en la estructura de datos
			jugadores[jugadorIndex].positionX=nuevaPosicionX;
			jugadores[jugadorIndex].positionY=nuevaPosicionY;
			
			pthread_mutex_unlock(&mutex);
			
			//enviar laa posicion nueva a todos los jugadores conectados
			EnviarPosicionJugador(sock_conn);
		}
		else if(codigo==7){//listaConectados
			
		}
		else if(codigo==9){ //reenviar mensaje
			char mensaje[200];
			p=strtok(NULL,"/");
			strcpy(mensaje,p);
			char usuario[200];
			p=strtok(NULL,"/");
			strcpy(usuario,p);
			pthread_mutex_lock(&mutex);
			sprintf(respuesta, "9-%s-%s", mensaje, usuario);
			printf("%s\n", respuesta);
			int j;
			for (j = 0; j < lista.num; j++)
			{
				write (sockets[j],respuesta,strlen(respuesta));
			}
			pthread_mutex_unlock(&mutex);
		}
		else if (codigo == 0 || codigo == 1 || codigo == 5)//envia la lista de conectados cada vez que se modifica
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
			pthread_mutex_unlock(&mutex);
		}
		printf("Nombre: %s\n", nombre);
	}
	close(sock_conn);
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	pthread_t thread;
	lista.num = 0;
	int conexion = 0;
	int puerto = 5062;
	int i = 0;
	
	//abrimos el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0){
		printf("Error al crear socket\n");
		exit(EXIT_FAILURE);	
	}
	//Bind en el puerto
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	//establecemos puerto de escucha
	serv_adr.sin_port = htons(puerto);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		printf ("Error en el bind\n");
		exit(EXIT_FAILURE);	
	}
	
	if (listen(sock_listen, 3) < 0){
		printf("Error en el Listen\n");
		exit(EXIT_FAILURE);	
	}
	
	int rc;
	
	while(conexion == 0)
	{
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida\n");
		
		
		sockets[i] = sock_conn;
		
		rc = pthread_create (&thread, NULL, atenderCliente , &sockets[i]);
		printf("Codigo %d = %s\n", rc, strerror(rc));
		
		i++;
	}
	return 0;
	
}
