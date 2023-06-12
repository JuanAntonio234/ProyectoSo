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
using UnityEditor.VersionControl;
using System.Net.WebSockets;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;
    private readonly Queue<Action> actionQueue = new Queue<Action>();

    private void Update()
    {
        lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                Action action = actionQueue.Dequeue();
                action.Invoke();
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (actionQueue)
        {
            actionQueue.Enqueue(action);
        }
    }

    public static UnityMainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<UnityMainThreadDispatcher>();
            if (instance == null)
            {
                GameObject obj = new GameObject("UnityMainThreadDispatcher");
                instance = obj.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(obj);
            }
        }
        return instance;
    }
}

public class CCliente : MonoBehaviour
{

    Socket servidor;
    Thread atender;

    public TMP_Text texto;
    public TMP_Text textMensaje;
    public TMP_Text invitacionAceptadaRechazada;

    private byte[] recibirbuffer = new byte[1024];

    public TMP_InputField NameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPasswordInput;
    public TMP_InputField IdInput;
    public TMP_InputField HostInput;
    public TMP_InputField InvitadoInput;

    public Button Consulta1;
    public Button Consulta2;
    public Button Consulta3;

    private GameObject prefabJugador; // Asigna el prefab del jugador 
    private GameObject jugadorLocal;
    public GameObject PanelInvitacion;

    private string nombreJugador;

    public static Socket clienteSocket;

    private byte[] buffer;

   private void Start()
    {
        CerrarPanel();
    }
    private void AtenderServidor()
    {
        while (true)
        {
            byte[] msg = new byte[1024];
            servidor.Receive(msg);
            string mensaje = Encoding.ASCII.GetString(msg).Split('\0')[0];
            string[] trozos = Encoding.ASCII.GetString(msg).Split('-');
            int codigo;
            try
            {
                codigo = Convert.ToInt32(trozos[0]);
                mensaje = trozos[1];

            }catch(System.FormatException)
            {
                codigo=0;
            }

            switch (codigo)
            {
                case 0://Login
                    if (mensaje == "0-0")
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
                case 1://Registrar
                    if (mensaje == "1-0")
                    {

                            texto.text = "Registrado correctamente";
                            Debug.Log("Registrado Correctamente");
                    }
                    else if (mensaje == "Error")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "Error";
                            Debug.Log("Problema al crear al usuario");
                        });
                    }
                    break;

                    //Modificar todas las consultas
/*
                case 2://consulta (numero consulta)
                    if (mensaje == "2")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            string tiempoJugado = mensaje.Split('-')[1];
                            string partidasTotales = mensaje.Split('-')[1];
                            texto.text = "El tiempo medio jugado es: " + tiempoJugado + " y las partidas totales de Pere son: " + partidasTotales;
                        });

                    }
                    else if (mensaje == "Error")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "Error";
                            Debug.Log("No se encuentran datos que coincidan");

                        });
                    }
                    break;
                case 3://consulta (numero consulta)
                    if (mensaje == "3")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            string jugador = mensaje.Split("-")[1];
                            texto.text = "El nombre del jugador con una duración mayor que tres es " + jugador;
                        });
                    }
                    else if (mensaje == "Error")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "No se encuentran datos que coincidan";
                            Debug.Log("No se encuentran datos que coincidan");
                        });
                    }
                    break;
                case 4://consulta (numero consulta)
                    if (mensaje == "4")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            string jugador = mensaje.Split("-")[1];
                            string idJugador = mensaje.Split("-")[1];
                            texto.text = "Id del " + jugador + "es: " + idJugador;
                        });
                    }
                    else if (mensaje == "Error")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "No se encuentran datos que coincidan";
                            Debug.Log("No se encuentran datos que coincidan");
                        });
                    }
                    break;*/


                case 5:
                    if (codigo == 6)//lista jugadores conectados
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
                case 6://invitacion a partida
                    if (codigo==7)
                    {
                        AbrirPanel();
                        string nombreHost = mensaje.Split('-')[0];
                        textMensaje.text = nombreHost + "te ha invitado a jugar";
                    }
                    break;
                case 7://respuesta invitacion a partida


                    string host = mensaje.Split('-')[0];
                    string respuesta = mensaje.Split('-')[1];
                    if (respuesta == "SI")
                    {
                        CerrarPanel();
                        SceneManager.LoadScene("Juego"); 
                    }
                    else if (respuesta == "NO")
                    {
                        CerrarPanel();
                        invitacionAceptadaRechazada.text = "El jugador ha rechazado la invitacion. Vuelve a invitar a alguien para poder jugar.";
                    }
                    break;
                case -1://desconectar
                    servidor.Shutdown(SocketShutdown.Both);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        texto.text = "Desconectandose del servidor";
                        Debug.Log("Desconectandose del servidor");
                    });
                    break;
            }
        }
    }
     public void Conectarse()
     {
        Socket servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, 5064);

        //Creamos el socket 

        try
         {
             servidor.Connect(ip);
             UnityMainThreadDispatcher.Instance().Enqueue(() =>
             {
                 Debug.Log("Conectado");

             });
        //    jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);

        }
        catch (SocketException ex)
         {
             UnityMainThreadDispatcher.Instance().Enqueue(() =>
             {
                 Debug.Log("no se ha podido conectar con el servidor:" + ex);
             });

             return;
         }
     }
    public void IniciarSesion()
    {
        try
        {
            string Name = NameInput.text;
            string Password = PasswordInput.text;
            string PasswordConfirm = ConfirmPasswordInput.text;
            string ID = IdInput.text;

            string iniciarSesion = "0" + "-" + Name + "-" + Password;
            byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(iniciarSesion);
            servidor.Send(mensaje1);

            ThreadStart t = delegate { AtenderServidor(); };
            atender = new Thread(t);
            atender.Start();
            // Crea una instancia del prefab del jugador en la posición inicial
            jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);
        }
        catch (SocketException e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                texto.text = "no se ha podido conectar con el servidor:";
                Debug.Log("no se ha podido conectar con el servidor:" + e);
            });
            return;
        }
    }
    public void Registrar()
    {
        Socket servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, 5064);

        //Creamos el socket 
        try
        {
            servidor.Connect(ip);

            Debug.Log("Conectado");

  
            string Name = NameInput.text;
            string Password = PasswordInput.text;
            string PasswordConfirm = ConfirmPasswordInput.text;
            string ID = IdInput.text;

            if (Password == PasswordConfirm)
            {
                string registrar = "1" + "-" + ID + "-" + Name + "-" + Password;
                byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(registrar);
                servidor.Send(mensaje1);

                    SceneManager.LoadScene("MenuJuego");

               // jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);
            }
            else if (PasswordInput.text != ConfirmPasswordInput.text)
            {
                texto.text = "Contraseña incorrecta.";
            }
        }
        catch (SocketException ex)
        {
            Debug.Log("no se ha podido conectar con el servidor:" + ex);


                texto.text = "No se ha podido conectar con el servidor: " + ex.Message;
                Debug.Log("No se ha podido conectar con el servidor: " + ex.Message);

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
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                string QUEry1 = "1";
                byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(QUEry1);
                servidor.Send(mensaje);
            });
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
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                string QUEry3 = "3";
                byte[] mensaje = System.Text.Encoding.ASCII.GetBytes(QUEry3);
                servidor.Send(mensaje);
            });
        }
        catch (SocketException ex)
        {
            Debug.Log("No se ha podido conectar con el servidor: " + ex);
            return;
        }
    }
    public void AbrirPanel()
    {
        PanelInvitacion.SetActive(true);

    }
    public void CerrarPanel()
    {
        PanelInvitacion.SetActive(false);
    }
    //invitar
    public void Invitar()
    {
        
    }

}