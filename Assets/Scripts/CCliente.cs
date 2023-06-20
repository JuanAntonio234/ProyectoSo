using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CCliente : MonoBehaviour
{
    Socket servidor;
    Thread atender;

    int puerto = 50016;

    public PanelInvitacion panelInvitacion1;

    public TMP_Text texto;
    public TMP_Text textMensaje;
    public TMP_Text invitacionAceptadaRechazada;
    public TMP_Text Chat;
    public TMP_Text RespuestaConsultas;


    private byte[] recibirbuffer = new byte[1024];

    public TMP_InputField NameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPasswordInput;
    public TMP_InputField IdInput;
    public TMP_InputField HostInput;
    public TMP_InputField InvitadoInput;
    public TMP_InputField Mensaje;
    public TMP_InputField NombreJugadorInputField;

    public Button Consulta1;
    public Button Consulta2;
    public Button Consulta3;

    private GameObject prefabJugador; // Asigna el prefab del jugador 
    private GameObject jugadorLocal;

    private string nombreJugador;

    public static Socket clienteSocket;

    private byte[] buffer;

    private void Start()
    {

    }
    private void AtenderServidor()
    {
        while (true)
        {
            byte[] msg = new byte[1024];
            servidor.Receive(msg);
            string mensaje = Encoding.ASCII.GetString(msg).Split('\0')[0];
            string[] trozos = mensaje.Split('-');
            int codigo = 0;

            switch (codigo)
            {
                case 0: //login
                    if (mensaje == "0")
                    {
                        texto.text = "Se ha accedido correctamente a la cuenta";
                        Debug.Log("Se ha accedido correctamente a la cuenta");
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "Error";
                        Debug.Log("Usuario o contraseña incorrecta");
                    }
                    break;
                case 1: //registrar
                    if (mensaje == "1")
                    {

                        texto.text = "Registrado correctamente";
                        Debug.Log("Registrado Correctamente");
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "Error";
                        Debug.Log("Problema al crear al usuario");
                    }
                    break;
                case 2://consulta Partidas ganadas del jugador Pere
                    if (mensaje == "2")
                    {
                        string partidasGanadas = trozos[1];
                        RespuestaConsultas.text = "Las partidas ganadas del jugador llamado Pere son: " + partidasGanadas;
                    }
                    else if (mensaje == "Error")
                    {
                        RespuestaConsultas.text = "Error";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 3://consulta: mostrar todos los jugadores de la base de datos
                    if (mensaje == "4")
                    {
                        int numeroTotal = int.Parse(trozos[1]);
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
                case 4://consulta: Dame la id de un jugador introducida por pantalla
                    if (mensaje == "8")
                    {
                        string jugadorID = trozos[1];

                        RespuestaConsultas.text = "La ID del jugador es: " + jugadorID;
                    }
                    else if (mensaje == "Error")
                    {

                        RespuestaConsultas.text = "No se encuentran datos que coincidan";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 5: //lista jugadores conectados
                    if (mensaje == "6")
                    {
                        int numeroConectados = int.Parse(trozos[1]);
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
                case 6: //invitacion a partida
                    if (mensaje == "7")
                    {
                        panelInvitacion1.AbrirPanel();
                        string nombreHost = trozos[1];
                        textMensaje.text = nombreHost + "te ha invitado a jugar";
                    }
                    break;
                case 7: //respuesta invitacion a partida
                    string host = trozos[1];
                    string respuesta = trozos[2];
                    if (respuesta == "SI")
                    {
                        panelInvitacion1.CerrarPanel();
                        SceneManager.LoadScene("Juego");
                    }
                    else if (respuesta == "NO")
                    {
                        panelInvitacion1.CerrarPanel();
                        invitacionAceptadaRechazada.text = "El jugador ha rechazado la invitacion. Vuelve a invitar a alguien para poder jugar.";
                    }
                    break;
                case 8: //recibir mensaje
                    if (mensaje == "9")
                    {
                        string message = trozos[1];
                        string usuario = trozos[2];

                        Chat.text = usuario + ": " + message + "\n";
                    }
                    break;
                case -1: //desconectar
                    if (mensaje == "-1")
                    {
                        servidor.Shutdown(SocketShutdown.Both);
                        texto.text = "Desconectandose del servidor";
                        Debug.Log("Desconectandose del servidor");
                    }
                    break;
            }
        }
    }
    public void Conectarse() //procedimiento para conectarse al servidor
    {
        Socket servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress direccion = IPAddress.Parse("147.83.117.22");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        //Creamos el socket 

        try
        {
            servidor.Connect(ip);

            Debug.Log("Conectado"); //mensaje en consola de conexión

            //jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);

        }
        catch (SocketException ex)
        {

            Debug.Log("no se ha podido conectar con el servidor:" + ex);
            return;
        }
    }

    public void Desconectarse() //procedimiento para desconectarse del servidor
    {
        //mensaje de desconexión
        string mensaje = "5";

        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        servidor.Send(msg);

        // nos desconectamos

        servidor.Shutdown(SocketShutdown.Both);
        servidor.Close();
    }

    public void IniciarSesion() //procedimiento para iniciar sesión
    {
        Socket servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress direccion = IPAddress.Parse("147.83.117.22");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        try
        {
            servidor.Connect(ip);

            Debug.Log("Sesión iniciada"); //mensaje en consola de sesión iniciada

            string Name = NameInput.text;
            string Password = PasswordInput.text;

            string iniciarSesion = "0-" + Name + "-" + Password;
            byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(iniciarSesion);
            servidor.Send(mensaje1);

            SceneManager.LoadScene("MenuJuego");

            ThreadStart t = delegate { AtenderServidor(); };
            atender = new Thread(t);
            atender.Start();
            // Crea una instancia del prefab del jugador en la posición inicial
            //jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);
        }
        catch (SocketException e)
        {
            Debug.Log("no se ha podido conectar con el servidor:" + e);
            return;
        }
    }

    public void Registrar() //procedimiento para registrarse
    {
        try
        {
            string Name = NameInput.text;
            string Password = PasswordInput.text;
            string PasswordConfirm = ConfirmPasswordInput.text;
            string ID = IdInput.text;

            if (Password == PasswordConfirm)
            {
                string registrar = "1-" + ID + "-" + Name + "-" + Password;
                byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(registrar);
                servidor.Send(mensaje1);

                Debug.Log("Registrado"); //mensaje en consola de registro
                SceneManager.LoadScene("MenuJuego");

                // jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);
            }
            else if (PasswordInput.text != ConfirmPasswordInput.text)
            {
                texto.text = "Las contraseñas no coinciden";
            }
        }
        catch (SocketException ex)
        {
            Debug.Log("no se ha podido conectar con el servidor:" + ex);

            Debug.Log("No se ha podido conectar con el servidor: " + ex.Message);

            return;
        }
    }

    public void EnviarMensaje() //procedimiento para enviar un mensaje del chat al servidor
    {
        try
        {
            string msg = Mensaje.text;
            string Name = NameInput.text;

            string mensaje = "9-" + msg + "-" + Name;
            byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(mensaje);
            servidor.Send(mensaje1);
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor:" + ex);
            return;
        }
    }

    // Realizar consulta 1
    public void PartidasGanadasPere()
    {
        try
        {
            string query = "2";
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(query);
            servidor.Send(mensaje);

        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor:" + ex);
            return;
        }
    }

    // Realizar consulta 2
    public void JugadoresBaseDeDatos()
    {
        try
        {
            string query = "4";
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(query);
            servidor.Send(mensaje);
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex);
            return;
        }
    }

    // Realizar consulta 3
    public void IDJugador()
    {
        try
        {
            string Name = NombreJugadorInputField.text;

            string query = "8-" + Name;
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(query);
            servidor.Send(mensaje);
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex);
            return;
        }
    }

    //invitar
    public void Invitar()
    {
        try
        {
            string host = HostInput.text;
            string invitado = InvitadoInput.text;

            string invitar = "11-" + host + "-" + invitado;
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(invitar);
            servidor.Send(mensaje);
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex);
            return;
        }
    }

    //cerrar el juego desde el menú
    public void Cerrar()
    {
        Application.Quit();
    }

}