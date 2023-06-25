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

MYSQL *conn;
int err;
MYSQL_RES *resultado;
MYSQL_ROW row;

typedef struct {
	char nombre[20];
	int socket;
}Conectado;

typedef struct {
	Conectado conectados [100];
	int num;
} ListaConectados;

typedef struct{
	int idP ;
	char usrHost[20];
	int socketHost;
	char usrInvitado[20];
	int socketInvitado;
}Partida;

typedef struct{
	int clientSocket;
	float positionX;
	float positionY;
} Jugador;

Jugador jugadores[10];
ListaConectados lista;
Partida miPartida[100];

int i=0;
int sockets[100];

int Conectar(ListaConectados *lista, char nombre[20], int socket);

void DameConectados(ListaConectados *lista, char conectados[300])//envia lista de conectados
{
	int i;
	sprintf (conectados, "%d", lista->num);
	for (i = 0; i < lista->num; i++)
	{
		sprintf(conectados, "%5-s-%s", conectados, lista->conectados[i].nombre);
	}
}

int AddConectado (ListaConectados *lista, char nombre[20], int socket)
{
	if (lista->num == 100)
	{
		return -1;
	}
	else
	{
		strcpy(lista->conectados[lista->num].nombre, nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num++;
		return 0;
	}
	
}

int DameSocket(ListaConectados *lista, char nombre[20])
{
	int i = 0;
	while(i<lista->num)
	{
		if(strcmp(lista->conectados[i].nombre,nombre)==0)
		{
			return lista->conectados[i].socket;
		}
		else
		{
			i++;
		}
	}
}

int DamePos (ListaConectados *lista,  char nombre[20])
{
	int i=0;
	int terminado=0;
	int pos;
	
	if(lista->conectados[i].nombre==nombre)
	{
		int pos = i;
	}
	return pos;
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

int Conectar(ListaConectados *lista, char nombre[20], int socket)//a\uffc3\uffaf\uffc2\uffbf\uffc2\uffb1ade a la lista de conectados al usuario
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
	
	int login;
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion1: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "M6_BBDD", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion2: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
		
	sprintf(consulta, "SELECT JUGADOR.NOMBRE, JUGADOR.PASSWORD FROM JUGADOR WHERE (JUGADOR.NOMBRE='%s' AND JUGADOR.PASSWORD='%s')", nombre, contrasena);
	err=mysql_query (conn, consulta);
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
		sprintf(respuesta, "0-Error-Al consultar la base de datos");
	}
	else
	{
		MYSQL_RES *resultado;
		resultado=mysql_store_result(conn);
		int num_filas=mysql_num_rows(resultado);
		mysql_free_result(resultado);
		
		if(num_filas>0){
			printf ("Ha iniciado sesion el usuario con nombre: %s\n", nombre);
			sprintf(respuesta, "0-SI");		
		}
		else{
			printf ("No se encontro el usuario con nombre: %s\n", nombre);
			sprintf(respuesta, "0-Error-Usuario o contrasena mal introducida");	
		}
	}
	mysql_close(conn);
}


void Registrar(char nombre[25], char contrasena[25], char respuesta[512])
{
	char consulta[500];
	int numJ;
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion1: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root","mysql", "M6_BBDD", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion2: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	sprintf(consulta, "INSERT INTO JUGADOR VALUES ('%s', '%s', 0)", nombre, contrasena);
	err=mysql_query (conn, consulta);
	if (err!=0)
	{
		printf("Error al insertar datos en la base: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
		sprintf(respuesta, "1-Error");
	}
	
	else
	{
		sprintf(respuesta, "1-SI");
	}
	mysql_close(conn);
}

void JugadoresBaseDeDatos(char respuesta[512])
{
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char nombres[50]="";
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion1: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "M6_BBDD", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion2: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn, consulta);
	
	strcpy (consulta,"SELECT NOMBRE from JUGADOR");
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
		sprintf(respuesta, "3-Error");
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
		printf("%s",nombres);
		sprintf(respuesta, "3-SI-%s", nombres);
	}
	mysql_close (conn);
	exit(0);
}

void PartidasGanadasPere(char respuesta[512])
{
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char pganadas[10];
	char consulta[500];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion1: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "M6_BBDD", 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion2: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT PARTIDAS_GANADAS FROM JUGADOR WHERE NOMBRE = 'Pere'");
	err=mysql_query (conn, consulta);
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL || atoi(row[0]) == 0)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "2-Error");
	}
	else
	{
		strcpy (pganadas, atoi(row[0]));
		sprintf(respuesta, "2-SI-%s", pganadas);
	}
	mysql_close (conn);
	exit(0);
}

void DamePartidasGanadasJugador(char nombre[50], char respuesta[512])
{
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char pganadas[10];
	char consulta [100];
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//inicializar la conexin
	conn = mysql_real_connect (conn,"localhost","root", "mysql", "M6_BBDD", 0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//consulta SQL
	strcpy (consulta, "SELECT PARTIDAS_GANADAS FROM JUGADOR WHERE NOMBRE = '");
	strcat (consulta, nombre);
	strcat (consulta, "'");
	
	//Para comprobar errores en la consulta
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//recogemos el resultado de la consulta
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL || atoi(row[0]) == 0)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "4-Error");
	}
	else
	{
		strcpy (pganadas, atoi(row[0]));
		sprintf(respuesta, "4-SI-%s", pganadas);
	}
	
	mysql_close (conn);
	exit(0);
}

int AddPartida(Partida miPartida[],char nombreHost[20],char nombreInv[20], int socketH,int socketI)
{
	int i =0;
	int terminado=0;
	while(i<100 && terminado==0)
	{
		if(miPartida[i].idP == NULL)
		{
			miPartida[i].idP = i;
			strcpy(miPartida[i].usrHost, nombreHost);
			
			miPartida[i].socketHost = socketH;
			strcpy(miPartida[i].usrInvitado, nombreInv);
			miPartida[i].socketInvitado = socketI;
			terminado =1;
			
		}
		else
		{
			i++;
		}
	}
	if(terminado == 0)
	{
		return -1;
	}
	else
	{
		return i;
	}
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

void* atenderCliente(void* socket)
{
	int jugadorPosicionX = 0;
	int jugadorPosicionY = 0;
	int jugadorIndex = -1;
	
	int ret;
	int sock_conn;
	int* s;
	s = (int*) socket;
	sock_conn = *s;
	
	char peticion[512];
	char respuesta[512];
	char contestacion[512];
	char contrasena[20];
	char consultas[512];
	char numero;
	
	char conectados[300];
	int conexion = 0;
	int r;
	int i;
	char nombre[20];
	char nombre2[20];
	int s1;
	int s2;
	
	while(conexion == 0)
	{
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		peticion[ret]='\0';
		int error = 1;
		int codigo = 9999;
		printf ("Peticion: %s\n",peticion);
		char* p = strtok(peticion, "-");
		codigo = atoi(p);
		
		if ((codigo==20)||(codigo==1))
		{
			
			p = strtok (NULL, "-");
			if(p != NULL){
			strcpy(nombre, p);
			printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
			//Guardamos socket del usuario
			s1 = DameSocket(&lista, nombre);
			}
		}
		
		if(codigo == 0) //LOGIN
		{
			p = strtok(NULL, "-");
			if(p != NULL){
			strcpy(contrasena, p);
			printf("Codigo: %d, Nombre: %s y Contrase￱a: %s\n",codigo, nombre, contrasena);
			Login(nombre, contrasena, contestacion);
			pthread_mutex_lock(&mutex);
			if(strcmp (contestacion, "Error") != 0){
				r = Conectar(&lista, nombre,socket);
				DameConectados(&lista, conectados);
				sprintf(respuesta, "%s", contestacion);
				int j;
				while(j<lista.num){
					write (sockets[j], respuesta, strlen(respuesta));
					j++;
				}
			}
			}
			pthread_mutex_unlock(&mutex);
		}
		else if(codigo == 1) //REGISTRAR
		{
			
			p = strtok(NULL, "-");
			printf("%s\n", p);
			if(p != NULL){
				strcpy(contrasena, p);
				printf("Codigo: %d, Nombre: %s y Contrasena: %s\n", codigo, nombre, contrasena);
				Registrar(nombre, contrasena, contestacion);
				pthread_mutex_lock(&mutex);
				if(strcmp (contestacion, "Error") != 0){
					r = Conectar(&lista, nombre,socket);
					DameConectados(&lista, conectados);
					sprintf(respuesta, "%s", contestacion);
					int j;
					while(j<lista.num){
						write (sockets[j], respuesta, strlen(respuesta));
						j++;
					}
				}
			pthread_mutex_unlock(&mutex);
			}
		}
		else if(codigo == 2)//partidas ganadas Pere
		{
			p = strtok(NULL, "-");
			if(p != NULL){
			strcpy(consultas, p);
			printf("Codigo: %d, Partidas ganadas: %s\n", codigo, consultas);
			PartidasGanadasPere(contestacion);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			}
		}
		else if(codigo == 4) //lista de jugadores de la base de datos
		{
			p = strtok(NULL, "-");
			if(p != NULL){
			strcpy(consultas, p);
			printf("Codigo: %d, Jugadores: %s\n", codigo, consultas);
			JugadoresBaseDeDatos(contestacion);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			}
		}
		else if(codigo == 8) //Partidas ganadas de cada jugador
		{
			p = strtok(NULL, "-");
			if(p != NULL){
			strcpy(consultas, p);
			printf("Codigo: %d, ID: %s\n", codigo, consultas);
			DamePartidasGanadasJugador(nombre, contestacion);
			sprintf(respuesta, "%s", contestacion);
			write (sock_conn,respuesta,strlen(respuesta));
			}
		}
		
		else if(codigo == 5) //DESCONECTAR
		{
			pthread_mutex_lock(&mutex);
			p = strtok(NULL, "-");
			if(p != NULL){
			strcpy(nombre, p);
			conexion = 1;
			printf("Desconectando a %s\n", nombre);
			r = Desconectar(&lista, nombre);
			printf("Codigo de desconexion: %d\n", r);
			DameConectados(&lista, conectados);
			close(sock_conn);
			pthread_mutex_unlock(&mutex);
			}
		}
		else if(codigo==6)//actualiza posicion jugador
		{
			pthread_mutex_lock(&mutex);
			p=strtok(NULL,"-");
			if(p != NULL){
			int nuevaPosicionX=atoi(p);
			p=strtok(NULL,"-");
			if(p != NULL){
			int nuevaPosicionY=atoi(p);
			printf("Actualizar posicion: X=%d, Y=%d\n",nuevaPosicionX,nuevaPosicionY);
			
			//Actualizar la posicion del jugador en la estructura de datos
			jugadores[jugadorIndex].positionX=nuevaPosicionX;
			jugadores[jugadorIndex].positionY=nuevaPosicionY;
			
			pthread_mutex_unlock(&mutex);
			
			//enviar laa posicion nueva a todos los jugadores conectados
			EnviarPosicionJugador(sock_conn);
			}
			}
		}
		else if(codigo==9){ //reenviar mensaje
			char mensaje[200];
			p=strtok(NULL,"-");
			if(p != NULL){
			strcpy(mensaje,p);
			char usuario[200];
			p=strtok(NULL,"-");
			if(p != NULL){
			strcpy(usuario,p);
			sprintf(respuesta, "10-%s-%s", mensaje, usuario);
			printf("Mensaje: %s\n", respuesta);
			int j;
			for (j = 0; j < lista.num; j++)
			{
				write (sockets[j],respuesta,strlen(respuesta));
			}
			}
			}
		}
		
		else if (codigo==11) //reenviar invitacion
		{
			p=strtok(NULL,"-");
			if(p != NULL){
			strcpy(nombre2,p);
			printf("Jugador %s invita a %s", nombre, nombre2);
			sprintf(respuesta, "7-%s", nombre);
			write (DameSocket(&lista,nombre2), respuesta, strlen(respuesta));
			}
		}
		else if (codigo==12) //respuesta del invitado al host
		{
			p=strtok(NULL,"-");
			if(p != NULL){
			strcpy(nombre2,p);
			char resp[200];
			p=strtok(NULL,"-");
			if(p != NULL){
			strcpy(resp,p);
			if(strcmp(resp,"SI")==0)
			{
				int idP = AddPartida (miPartida,nombre,nombre2,DameSocket(&lista, nombre),DameSocket(&lista, nombre2));
				sprintf(respuesta,"8-SI-%d", idP);
				write (DameSocket(&lista, nombre), respuesta, strlen(respuesta));
				sprintf(respuesta,"9-SI-%d", idP);
				write (DameSocket(&lista, nombre2), respuesta, strlen(respuesta));
			}
			else
			{
				sprintf(respuesta,"8-NO");
				write (sock_conn,respuesta,strlen(respuesta));
			}
			}
			}
		}
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
	int puerto = 5054;
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
	
	while(conexion == 0)
	{
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida\n");
		
		
		sockets[i] = sock_conn;
		
		pthread_create (&thread, NULL, atenderCliente , &sockets[i]);
		
		i++;
	}
	return 0;
	
}

