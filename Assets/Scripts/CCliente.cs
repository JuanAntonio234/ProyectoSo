using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CCliente : MonoBehaviour
{
    public TMP_Text texto;
    public TextMeshProUGUI texto1;
    Socket servidor;
    Thread atender;

    int puerto=5052;

    private byte[] recibirbuffer = new byte[1024];
    private StringBuilder recibirData = new StringBuilder();
    ////////////////////////////////////////////////////////
    public TMP_InputField NameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPasswordInput;
    public TMP_InputField IdInput;

    public Button Consulta1;
    public Button Consulta2;
    public Button Consulta3;


    public Grid jugadoresGrid;


    ///////////////////////////////////////////////////////
    private void AtenderServidor()
    {
        while (true)
        {
            byte[] msg = new byte[1024];
            servidor.Receive(msg);
            string[] trozos = Encoding.ASCII.GetString(msg).Split('-');
            int codigo = Convert.ToInt32(trozos[0]);
            string mensaje = Encoding.ASCII.GetString(msg).Split('\0')[0];

            switch (codigo)
            {
                case 0:
                    if (mensaje == "0-0")
                    {
                        texto.text = "Se ha accedido correctamente a la cuenta";
                        Debug.Log("Se ha accedido correctamente a la cuenta");
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "Error";
                        Debug.Log("Usuario o contrase�a incorrecta");
                    }
                    break;
                case 1:
                    if (mensaje == "1-0")
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
               case 2:
                    if (mensaje == "2")
                    {
                        string tiempoJugado = mensaje.Split('-')[1];
                        string partidasTotales = mensaje.Split('-')[1];

                        texto.text = "El tiempo medio jugado es: " + tiempoJugado + " y las partidas totales de Pere son: " + partidasTotales;
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "Error";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 3:
                    if (mensaje == "3")
                    {
                        string jugador = mensaje.Split("-")[1];
                        texto.text = "El nombre del jugador con una duraci�n mayor que tres es " + jugador;
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "No se encuentran datos que coincidan";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 4:
                    if (mensaje == "4")
                    {
                        string jugador = mensaje.Split("-")[1];
                        string idJugador = mensaje.Split("-")[1];
                        texto.text = "Id del " + jugador + "es: " + idJugador;
                    }
                    else if (mensaje == "Error")
                    {
                        texto.text = "No se encuentran datos que coincidan";
                        Debug.Log("No se encuentran datos que coincidan");
                    }
                    break;
                case 5:
                    int numeroConectados = int.Parse(trozos[1]);
                    List<string> jugadoresConectados = GetConnectedPlayersList(mensaje);
                    
                    for(int i = 0; i < numeroConectados; i++)
                    {
                        foreach (string jugador in jugadoresConectados)
                        {
                            texto.text=jugador;
                        }
                    }
                    ///creo que mejor poner text y asignar uno a uno//////////////////////////////////////////
                    ////////////////////////////////////////////
                    ///////////////////////////////////////////
                    ///////////////////////////////////////////
                    //////////////////////////////////////////
                    
                    break;
                case 6:
                    if (mensaje != "6-ERROR")
                    {
                        Convert.ToString(mensaje);
                        string Partida = mensaje.Split('-')[1];
                        string invitacion = mensaje.Split('-')[2];
                        texto.text = "El jugador te ha invitado a jugar";
                    }
                    break;
                case -1:
                    servidor.Shutdown(SocketShutdown.Both);
                    texto.text = "Desconectandose del servidor";
                    Debug.Log("Desconectandose del servidor");
                    break;
            }
        }
    }
    public void IniciarSesion()
    {
        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            servidor.Connect(ip);
            string Name = NameInput.text;
            string Password = PasswordInput.text;
            string PasswordConfirm = ConfirmPasswordInput.text;
            string ID = IdInput.text;

            string iniciarSesion = "0" + "/" + Name + "-" + Password;
            byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(iniciarSesion);
            servidor.Send(mensaje1);

            ThreadStart t = delegate { AtenderServidor(); };
            atender = new Thread(t);
            atender.Start();
        }
        catch (SocketException e)
        {
            texto.text = "no se ha podido conectar con el servidor:";
            Debug.Log("no se ha podido conectar con el servidor:" + e);
            return;
        }
    }
    ////////////////////////////////////
    public void Conectarse()
    {
        servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        //Creamos el socket 

        try
        {
            servidor.Connect(ip);
            texto.text = "conectado correctamente";
            Debug.Log("conectado");
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex.Message);

            return;
        }
    }
    /////////////////////////////////////////
        public void Registrar()
    {
        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            servidor.Connect(ip);
            string Name = NameInput.text;
            string Password = PasswordInput.text;
            string PasswordConfirm = ConfirmPasswordInput.text;
            string ID = IdInput.text;
            
            if (Password == PasswordConfirm)
            {
                string registrar = "1" + "/" + Name + "-" + Password + "-" + ID;
                byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(registrar);
                servidor.Send(mensaje1);
                SceneManager.LoadScene("MenuJuego");
            }
            else if (PasswordInput.text != ConfirmPasswordInput.text)
            {
                texto1.text = "Las contrase�as no coinciden";
            }
        }
        catch (SocketException e)
        {
            texto.text = "no se ha podido conectar con el servidor:";
            Debug.Log("no se ha podido conectar con el servidor:" + e.Message);
            return;
        }
    }
    ///////////////////////////////////////////////////////////////

    // Realizar consulta 1
    public void Query1()
    {
        try
        {
            string QUEry1 = "1";
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(QUEry1);
            servidor.Send(mensaje);
        }
        catch (SocketException ex)
        {
            Debug.Log("no se ha podido conectar con el servidor:" + ex);
            return;
        }
    }

    // Realizar consulta 2
    public void Query2()
    {
        try
        {
            string QUEry2 = "2";
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(QUEry2);
            servidor.Send(mensaje);
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex);
            return;
        }
    }
    // Realizar consulta 3
    public void Query3()
    {
        try
        {
            string QUEry3 = "3";
            byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(QUEry3);
            servidor.Send(mensaje);
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex);
            return;
        }
    }

    public List<string> GetConnectedPlayersList(string stringJugador)
    {
        List<string> jugadoresConectados = new List<string>();
        string[] ArrayJugadores = stringJugador.Split('-');
        int numeroConectados = int.Parse(ArrayJugadores[1]);

        for (int i = 0; i < numeroConectados; i++)
        {
            string Jugador = ArrayJugadores[i + 2];
            string []informacionJugador = Jugador.Split('|');
            string nombreJugador = informacionJugador[0];
            string estadoJugador = informacionJugador[1];
            jugadoresConectados.Add(nombreJugador);
        }
        return jugadoresConectados;
    }
}
