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
    public TMP_Text texto;

    Socket servidor;
    Thread atender;

    int puerto=5062;

    private byte[] recibirbuffer = new byte[1024];
    private StringBuilder recibirData = new StringBuilder();

    public TMP_InputField NameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPasswordInput;
    public TMP_InputField IdInput;

    public Button Consulta1;
    public Button Consulta2;
    public Button Consulta3;

    private GameObject prefabJugador; // Asigna el prefab del jugador 
    private GameObject jugadorLocal;

    private string nombreJugador;

    private void Start()
    {
        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        servidor.Connect(ip);
    }
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
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "Se ha accedido correctamente a la cuenta";
                            Debug.Log("Se ha accedido correctamente a la cuenta");
                        });
                    }
                    else if (mensaje == "Error")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "Error";
                            Debug.Log("Usuario o contrase�a incorrecta");
                        });
                    }
                    break;
                case 1:
                    if (mensaje == "1-0")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            texto.text = "Registrado correctamente";
                            Debug.Log("Registrado Correctamente");
                        });
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
               case 2:
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
                case 3:
                    if (mensaje == "3")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            string jugador = mensaje.Split("-")[1];
                            texto.text = "El nombre del jugador con una duraci�n mayor que tres es " + jugador;
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
                case 4:
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
                    break;
                case 5:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        int numeroConectados = int.Parse(trozos[1]);
                        List<string> jugadoresConectados = GetConnectedPlayersList(mensaje);

                        for (int i = 0; i < numeroConectados; i++)
                        {
                            foreach (string jugador in jugadoresConectados)
                            {
                                texto.text = jugador;
                            }
                        }
                    });
                    ///creo que mejor poner text y asignar uno a uno//////////////////////////////////////////
                    ////////////////////////////////////////////
                    ///////////////////////////////////////////
                    ///////////////////////////////////////////
                    //////////////////////////////////////////
                    
                    break;
                case 6:
                    if (mensaje != "6-ERROR")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Convert.ToString(mensaje);
                            string Partida = mensaje.Split('-')[1];
                            string invitacion = mensaje.Split('-')[2];
                            texto.text = "El jugador te ha invitado a jugar";
                        });
                    }
                    break;
                case -1:
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
   /*  public void Conectarse()
    {
        servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress direccion = IPAddress.Parse("192.168.56.102");
        IPEndPoint ip = new IPEndPoint(direccion, puerto);

        //Creamos el socket 

        try
        {
            servidor.Connect(ip);
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                texto.text = "Conectado correctamente";
                Debug.Log("Conectado");
            });
        }
        catch (SocketException ex)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                texto.text = "no se ha podido conectar con el servidor:";
                Debug.Log("no se ha podido conectar con el servidor:" + ex);
            });

            return;
        }
    }*/
    public void IniciarSesion()
    {
        try
        { 
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
            // Crea una instancia del prefab del jugador en la posici�n inicial
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
        try
        {
            string Name = NameInput.text;
            string Password = PasswordInput.text;
            string PasswordConfirm = ConfirmPasswordInput.text;
            string ID = IdInput.text;
            
            if (Password == PasswordConfirm)
            {
                string registrar = "1" + "-" +ID + "-" + Name + "-" + Password;
                byte[] mensaje1 = System.Text.Encoding.ASCII.GetBytes(registrar);
                servidor.Send(mensaje1);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    SceneManager.LoadScene("MenuJuego");
                });
                jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);
            }
            else if (PasswordInput.text != ConfirmPasswordInput.text)
            {
                texto.text = "Contrase�a incorrecta.";
            }
        }
        catch (SocketException e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                texto.text = "No se ha podido conectar con el servidor: " + e.Message;
                Debug.Log("No se ha podido conectar con el servidor: " + e.Message);
            });
            return;
        }
    }
    ///////////////////////////////////////////////////////////////

    // Realizar consulta 1
    public void Query1()
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

    //la lista la obtengo del servidor
    public List<string> GetConnectedPlayersList(string stringJugador)
    {
        List<string> jugadoresConectados = new List<string>();
        string[] ArrayJugadores = stringJugador.Split('-');
        int numeroConectados = int.Parse(ArrayJugadores[1]);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            for (int i = 0; i < numeroConectados; i++)
            {
                string Jugador = ArrayJugadores[i + 2];
                string[] informacionJugador = Jugador.Split('|');
                string nombreJugador = informacionJugador[0];
                string estadoJugador = informacionJugador[1];
                jugadoresConectados.Add(nombreJugador);
            }
        });
        return jugadoresConectados;
    }
    void ConectarJugador(string nombre)
    {
        // Asignar el nombre del jugador al prefab
        nombreJugador = nombre;
        ActualizarNombreJugador();
    }
    void ActualizarNombreJugador()
    {
        // Asignar el nombre del jugador al nombre del prefab
        prefabJugador.name = nombreJugador;
    }
    void ActualizarPosicionJugador(GameObject jugador, Vector3 nuevaPosicion)
    {
        byte[] msg = new byte[1024];
        servidor.Receive(msg);

        string mensaje = Encoding.ASCII.GetString(msg).Split('\0')[0];

        //borrar posteriormente      // El mensaje debe contener la informaci�n de posici�n en un formato espec�fico (por ejemplo: "POSICION:2.5,1.3")
        string[] partes = mensaje.Split(':');
        if (partes.Length == 2 && partes[0] == "POSICION")
        {
            string[] valores = partes[1].Split(',');
            if (valores.Length == 2)
            {
                float posX = float.Parse(valores[0]);
                float posY = float.Parse(valores[1]);
                // Actualiza la posici�n del jugador local
                jugadorLocal.transform.position = new Vector3(posX, posY, 0f);
            }
        }
    }
}