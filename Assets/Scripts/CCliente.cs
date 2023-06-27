using Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCliente : MonoBehaviour
{
    //mensajes
    public TextMeshProUGUI text1;

    //variables para registrarse/loguearse
    public TMP_InputField NameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPasswordInput;

    //variables para las consultas
    public TMP_InputField NombreJugadorInputField;
    public TextMeshProUGUI notificacion;

    //varible para mostrar el panel usado para la funcion invitiacion
    public PanelInvitacion panelInvitacion1;

    //variables para la funcion invitacion
    public TextMeshProUGUI textMensaje;
    public TMP_InputField HostInput;
    public TMP_InputField InvitadoInput;
    private string host;
    private string usuario;
    private string contrasena;

    //variables para el chat
    public TextMeshProUGUI Chat;
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

        //continua con el resto del codigo
        await AtenderServidor();
    }

    private async Task AtenderServidor()
    {
        while (true)
        {
            string respuestaServidor = await Task.Run(() => conexionServidor.EscucharMensaje());
            Debug.Log(respuestaServidor);

            //codigo para separar el mensaje recibido del servidor
            string[] trozos = respuestaServidor.Split('-');
            int codigo = Convert.ToInt32(trozos[0]);
            string mensaje = trozos[1];

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
                        text1 = GameObject.Find("ErrorTxt").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        Debug.Log(trozos[2]);
                        text1.text = "Error: " + trozos[2];
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
                        text1 = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        text1.text = "Error: " + trozos[2];
                        Debug.Log("Problema al crear al usuario");
                    }
                    break;
                case 2://consulta Partidas ganadas del jugador Pere
                    if (mensaje == "SI")
                    {
                        int partidasGanadas = Convert.ToInt32(trozos[2]);
                        Debug.Log("Partidas ganadas: " + partidasGanadas);
                        text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        text1.text = "Las partidas ganadas del jugador llamado Pere son: " + trozos[2];
                    }
                    else if (mensaje == "Error")
                    {
                        text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        text1.text = "Error";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 3://consulta: mostrar todos los jugadores de la base de datos
                    if (mensaje == "SI")
                    {
                        List<string> jugadoresTotales = new List<string>();
                        for (int i = 2; i < trozos.Length; i++)
                        {
                            jugadoresTotales.Add(trozos[i]);
                        }
                        string STRjugadoresTotales = "";

                        foreach (string jugador in jugadoresTotales)
                        {
                            STRjugadoresTotales += jugador + ", ";
                        }

                        text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        text1.text = "Jugadores totales: " + STRjugadoresTotales;
                    }
                    else if (mensaje == "Error")
                    {
                        text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        text1.text = "No se encuentran datos que coincidan";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 4://consulta: Dame las partidas ganadas de un jugador introducida por pantalla
                    if (mensaje == "SI")
                    {
                        string nombre = trozos[2];
                        int partidasGanadas = Convert.ToInt32(trozos[3]);
                        Debug.Log("Partidas ganadas: " + partidasGanadas);
                        text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        text1.text = "Partidas ganadas de " + nombre + " = " + partidasGanadas;
                    }
                    else if (mensaje == "Error")
                    {

                        text1.text = "No se encuentran datos que coincidan";
                        text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                        text1.text = "";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 5: //lista jugadores conectados

                    List<string> jugadoresConectados = new List<string>();
                    for (int i = 2; i < trozos.Length; i++)
                    {
                        jugadoresConectados.Add(trozos[i]);
                    }
                    string STRjugadoresConectados = "";

                    foreach (string jugador in jugadoresConectados)
                    {
                        STRjugadoresConectados += jugador + ", ";
                    }
                    text1 = GameObject.Find("Respuestas").GetComponent<TextMeshProUGUI>();
                    text1.text = "";
                    text1.text = "Jugadores conectados: " + STRjugadoresConectados;
                    break;
                case 7: //invitacion a partida
                    if (mensaje == "No existe")
                    {
                        textMensaje = GameObject.Find("MensajeInvitacion").GetComponent<TextMeshProUGUI>();
                        textMensaje.text = mensaje;
                    }
                    else if (mensaje == "Invitacion enviada")
                    {
                        textMensaje = GameObject.Find("MensajeInvitacion").GetComponent<TextMeshProUGUI>();
                        textMensaje.text = "Invitación enviada, esperando respuesta";
                    }
                    else if (mensaje == "No puedes invitarte a ti mismo")
                    {
                        textMensaje = GameObject.Find("MensajeInvitacion").GetComponent<TextMeshProUGUI>();
                        textMensaje.text = mensaje;
                    }
                    break;
                case 8: //respuesta invitacion a partida siendo el invitado
                    panelInvitacion1.AbrirPanel();
                    host = mensaje;
                    textMensaje = GameObject.Find("MensajeInvitacion").GetComponent<TextMeshProUGUI>();
                    textMensaje.text = "Nueva invitación de " + mensaje;
                    break;
                case 9: //respuesta invitacion en caso de ser el host
                    if (mensaje == "Invitacion aceptada")
                    {
                        textMensaje = GameObject.Find("MensajeInvitacion").GetComponent<TextMeshProUGUI>();
                        textMensaje.text = mensaje;
                        EnviarID(trozos[2]);

                        SceneManager.LoadScene("Gameplay");
                    }
                    else if (mensaje == "Invitacion rechazada")
                    {
                        textMensaje = GameObject.Find("MensajeInvitacion").GetComponent<TextMeshProUGUI>();
                        textMensaje.text = mensaje;
                    }
                    break;
                case 10: //recibir mensaje
                    string usuario = trozos[2];

                    Chat = GameObject.Find("Chat").GetComponent<TextMeshProUGUI>();
                    Chat.text += ">>" + usuario + "-" + mensaje + "\n";
                    break;
                case 11: //abre juego en invitado
                    Debug.Log(mensaje);
                    SceneManager.LoadScene("Gameplay");
                    break;
                case 15:
                    if (mensaje == "SI")
                    {
                        Debug.Log("Eliminado Correctamente");
                        SceneManager.LoadScene("MenuPrincipal");
                    }
                    else if (mensaje == "Error")
                    {
                        Debug.Log("Problema al eliminar al usuario");
                    }
                    break;
                case 12: //desconectar
                    if (mensaje == "Desconectando")
                    {
                        Debug.Log("Cerrando sesión");
                        SceneManager.LoadScene("MenuPrincipal");
                    }
                    else if (mensaje == "Error al desconectar")
                        notificacion.text = "Error al desconectar";
                    break;
            }
        }
    }

    public void IniciarSesion() //procedimiento para iniciar sesión
    {
        string Name = NameInput.text;
        string Password = PasswordInput.text;

        if ((Password != "") || Name != "")
        {
            string mensajeIniciarSesion = "0-" + Name + "-" + Password;
            conexionServidor.EnviarMensajeServidor(mensajeIniciarSesion);
            Debug.Log("Enviado");
            usuario = NameInput.text;
            contrasena = PasswordInput.text;
        }
        else
        {
            text1 = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
            text1.text = "No has rellenado todos los apartados";
        }
    }

    public void Registrar() //procedimiento para registrarse
    {
        string Name = NameInput.text;
        string Password = PasswordInput.text;
        string PasswordConfirm = ConfirmPasswordInput.text;

        if ((Password != "") || (PasswordConfirm != "") || Name != "")
        {
            if ((Password == PasswordConfirm) && (PasswordInput != null) && (PasswordConfirm != null) && (Name != null))
            {
                string registrar = "1-" + Name + "-" + Password;
                conexionServidor.EnviarMensajeServidor(registrar);
                Debug.Log("Enviado");
                usuario = NameInput.text;
                contrasena = PasswordInput.text;
            }
            else if (PasswordInput.text != ConfirmPasswordInput.text)
            {
                text1.text = "Las contraseñas no coinciden";
                Debug.Log("Las contraseñas no coinciden");
            }
        }
        else
        {
            text1 = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
            text1.text = "No has rellenado todos los apartados";
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

    public void Desconectar() //desconectar/log out
    {
        string mensaje = "5-" + usuario + "-" + contrasena;
        conexionServidor.EnviarMensajeServidor(mensaje);
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
        string invitar = "11-" + HostInput.text + "-" + InvitadoInput.text;
        conexionServidor.EnviarMensajeServidor(invitar);
        Debug.Log("Enviado");
    }
    public void AceptaInvitacion() //aceptar invitacion
    {
        string mensajeAceptaInvitacion = "12-SI-" + host;
        conexionServidor.EnviarMensajeServidor(mensajeAceptaInvitacion);
        Debug.Log("Enviado");
        panelInvitacion1.CerrarPanel();
    }

    public void RechazarPartida() //denegar invitacion
    {
        string mensajeRechazaInvitacion = "12-NO-" + host;
        conexionServidor.EnviarMensajeServidor(mensajeRechazaInvitacion);
        Debug.Log("Enviado");
        panelInvitacion1.CerrarPanel();
    }

    public void EnviarID(string ID)
    {
        string mensaje = "13-" + ID;
        conexionServidor.EnviarMensajeServidor(mensaje);
        Debug.Log("Enviado");
    }

    public void Cerrar() //cerrar el juego desde el menú
    {
        conexionServidor.Desconectarse();
        Application.Quit();
    }

    public void DarseDeBaja()
    {
        string Name = NameInput.text;
        string Password = PasswordInput.text;

        string mensajeEliminar = "15-" + Name + "-" + Password;
        conexionServidor.EnviarMensajeServidor(mensajeEliminar);
        Debug.Log("Enviado");
    }

    public void ListaConectados()
    {
        string conectados = "7";
        conexionServidor.EnviarMensajeServidor(conectados);
        Debug.Log("Enviado");
    }
}