using Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CCliente : MonoBehaviour
{
    //mensajes de error
    public TMP_Text texto;

    //variables para registrarse/loguearse
    public TMP_InputField NameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPasswordInput;

    //variables para las consultas
    public Button Consulta1;
    public Button Consulta2;
    public Button Consulta3;
    public TMP_InputField NombreJugadorInputField;
    public TMP_Text RespuestaConsultas;

    //varible para mostrar el panel usado para la funcion invitiacion
    public PanelInvitacion panelInvitacion1;

    //variables para la funcion invitacion
    public TMP_Text textMensaje;
    public TMP_Text invitacionAceptadaRechazada;
    public TMP_InputField HostInput;
    public TMP_InputField InvitadoInput;

    //variables para el chat
    public TMP_Text Chat;
    public TMP_InputField nameChat;
    public TMP_InputField MensajeInputChat;

    private ConexionServidor conexionServidor;
    private ChatManager chatManager;

    private void Awake()
    {
        conexionServidor = ConexionServidor.GetInstance();
        // Verificar si la instancia de conexionServidor es nula y crearla si es necesario
        if (conexionServidor == null)
        {
            conexionServidor = new ConexionServidor();
        }
    }
    private async void Start()
    {
        // Conectarse al servidor solo una vez
        if (!conexionServidor.IsConnected())
        {
            int connectionResult = conexionServidor.ConnectToServer();
            if (connectionResult == 0)
            {
                // La conexión se realizó correctamente
                Debug.Log("Connected to server");
            }
            else
            {
                // Error al conectar con el servidor
                Debug.Log("Failed to connect to server");
                return;// Sale del método Start si la conexión falla
            }
        }
        // Obtener el componente ChatManager adjunto al GameObject
           chatManager = GetComponent<ChatManager>();

        //continua con el resto del codigo
        await AtenderServidor();
    }

    private async Task AtenderServidor()
    {
        while (true)
        {
            string respuestaServidor = await Task.Run(() => conexionServidor.EscucharMensaje());

            //codigo para separar el mensaje recibido del servidor
            string[] trozos = respuestaServidor.Split('-');
            int codigo = Convert.ToInt32(trozos[0]);
            string mensaje = trozos[1];
            Debug.Log(codigo);
            Debug.Log(mensaje);

            switch (codigo)
            {
                case 0: //login
                    if (mensaje == "SI")
                    {
                        if (conexionServidor.IsLoggedIn() == false)
                        {
                            conexionServidor.SetLoggedIn(true);
                            Debug.Log("Se ha accedido correctamente a la cuenta");
                            SceneManager.LoadScene("MenuJuego");
                        }
                    }
                    else if (mensaje == "Error")
                    {
                        Debug.Log("hola");
                        texto.text = "Error: "+trozos[2];
                    }
                    break;
                case 1: //registrar
                    if (mensaje == "SI")
                    {
                        Debug.Log("Registrado Correctamente");
                        SceneManager.LoadScene("MenuJuego");
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "Error: " + trozos[2];
                        Debug.Log("Problema al crear al usuario");
                    }
                    break;
                case 2://consulta Partidas ganadas del jugador Pere
                    if (mensaje == "SI")
                    {
                        string partidasGanadas = trozos[2];
                        RespuestaConsultas.text = "Las partidas ganadas del jugador llamado Pere son: " + partidasGanadas;
                    }
                    else if (mensaje == "Error")
                    {
                        RespuestaConsultas.text = "Error";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 3://consulta: mostrar todos los jugadores de la base de datos
                    if (mensaje == "SI")
                    {
                        int numeroTotal = int.Parse(trozos[2]);
                        List<string> jugadoresTotales = new List<string>();
                        for (int i = 0; i < numeroTotal; i++)
                        {
                            jugadoresTotales.Add(trozos[i + 2]);
                        }

                        string STRjugadoresTotales = "";
                        foreach (string jugador in jugadoresTotales)
                        {
                            STRjugadoresTotales += jugador + ", ";
                        }

                        RespuestaConsultas.text = "Jugadores totales: " + STRjugadoresTotales;
                    }
                    else if (mensaje == "Error")
                    {

                        RespuestaConsultas.text = "No se encuentran datos que coincidan";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 4://consulta: Dame las partidas ganadas de un jugador introducida por pantalla
                    if (mensaje == "SI")
                    {
                        int partidasGanadas = Convert.ToInt32(trozos[2]);

                        RespuestaConsultas.text = "La ID del jugador es: " + partidasGanadas;
                    }
                    else if (mensaje == "Error")
                    {

                        RespuestaConsultas.text = "No se encuentran datos que coincidan";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 5: //lista jugadores conectados
                    if (mensaje == "SI")
                    {
                        int numeroConectados = int.Parse(trozos[2]);
                        List<string> jugadoresConectados = new List<string>();
                        for (int i = 0; i < numeroConectados; i++)
                        {
                            jugadoresConectados.Add(trozos[i + 2]);
                        }

                        string jugadoresConectadosStr = "";
                        foreach (string jugador in jugadoresConectados)
                        {
                            jugadoresConectadosStr += jugador + ", ";
                        }

                        texto.text = "Jugadores conectados: " + jugadoresConectadosStr;
                    }
                    break;
                case 7: //invitacion a partida
                    if (mensaje == "SI")
                    {
                        panelInvitacion1.AbrirPanel();
                        string nombreHost = trozos[2];
                        textMensaje.text = nombreHost + "te ha invitado a jugar";
                    }
                    break;
                case 8: //respuesta invitacion a partida siendo el host
                    string respuesta = trozos[1];
                    int idP = Convert.ToInt32(trozos[2]);
                    if (respuesta == "SI")
                    {
                        invitacionAceptadaRechazada.text = "El jugador ha aceptado la invitacion.";
                        panelInvitacion1.CerrarPanel();
                        SceneManager.LoadScene("Gameplay");
                    }
                    else if (respuesta == "NO")
                    {
                        panelInvitacion1.CerrarPanel();
                        invitacionAceptadaRechazada.text = "El jugador ha rechazado la invitacion. Vuelve a invitar a alguien para poder jugar.";
                    }
                    break;
                /* case 9: //respuesta invitacion en caso de ser el invitado
                           string respuesta = trozos[1];
                           int idP2 = Convert.ToInt32(trozos[2]);
                           if (respuesta == "SI")
                           {
                               SceneManager.LoadScene("Gameplay");
                           }

                break;*/
                case 10: //recibir mensaje
                    if (chatManager != null)
                    {
                        string message = trozos[2];
                        string usuario = trozos[3];
                        chatManager.ActualizarMensajeChat(message,usuario);
                    }
                    else
                    {
                        Debug.Log("ChatManager no encontrado");
                    }
                    break;
                case -1: //desconectar
                    if (codigo == 10)
                    {
                        Debug.Log("Desconectandose");
                    }
                    break;
            }
        }
    }

    public void Conectarse() //procedimiento para conectarse al servidor
    {

    }

    public void IniciarSesion() //procedimiento para iniciar sesión
    {
        string Name = NameInput.text;
        string Password = PasswordInput.text;

        string mensajeIniciarSesion = "0-" + Name + "-" + Password;
        conexionServidor.EnviarMensajeServidor(mensajeIniciarSesion);
        Debug.Log("Enviado");
    }

    public void Registrar() //procedimiento para registrarse
    {
        string Name = NameInput.text;
        string Password = PasswordInput.text;
        string PasswordConfirm = ConfirmPasswordInput.text;

        if ((Password == PasswordConfirm) && (PasswordInput != null) && (PasswordConfirm != null) && (Name != null))
        {
            string registrar = "1-" + Name + "-" + Password;
            conexionServidor.EnviarMensajeServidor(registrar);
            Debug.Log("Enviado");
        }
        else if (PasswordInput.text != ConfirmPasswordInput.text)
        {
            texto.text = "Las contraseñas no coinciden";
            Debug.Log("Las contraseñas no coinciden");
        }
    }

    public void PartidasGanadasPere() // Realizar consulta 1
    {
        string query1 = "2";
        conexionServidor.EnviarMensajeServidor(query1);
        Debug.Log("Enviado");
    }

    public void JugadoresBaseDeDatos() // Realizar consulta 2
    {
        string query2 = "4";
        conexionServidor.EnviarMensajeServidor(query2);
        Debug.Log("Enviado");
    }

    public void PartidasGanadasJugador() // Realizar consulta 3
    {
        string NameJugadorInputField = NombreJugadorInputField.text;

        string query3 = "8-" + NameJugadorInputField;
        conexionServidor.EnviarMensajeServidor(query3);
        Debug.Log("Enviado");
    }

    public void Invitar() //invitar

    {
        string invitado = InvitadoInput.text;

        string invitar = "11-" + invitado;
        conexionServidor.EnviarMensajeServidor(invitar);
        Debug.Log("Enviado");
    }

    public void AceptaInvitacion() //aceptar invitacion

    {
        string host = NameInput.text;

        string mensajeAceptaInvitacion = "12-" + host + "-SI";
        conexionServidor.EnviarMensajeServidor(mensajeAceptaInvitacion);
        Debug.Log("Enviado");
        panelInvitacion1.CerrarPanel();
    }

    public void RechazarPartida() //denegar invitacion

    {
        string host = NameInput.text;

        string mensajeRechazaInvitacion = "12-" + host + "-NO";
        conexionServidor.EnviarMensajeServidor(mensajeRechazaInvitacion);
        Debug.Log("Enviado");
        panelInvitacion1.CerrarPanel();
    }

    public void Cerrar() //cerrar el juego desde el menú
    {
        conexionServidor.Desconectarse();
        Application.Quit();
    }









    ///////////////////////////////////añadir funcion para eliminar completamente a un jugador de la lista
    ///el login aunque estes registrado te dice que no
}