using Client;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{/*
    public TMP_InputField messageInput;
    public TMP_Text chatText;
    public string IP = "192.168.56.102";
    public int puerto = 5052;

    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[1024];
    private List<string> chatMessages = new List<string>();


    private void RecibirMensaje()
    {
        if (clientSocket == null || !clientSocket.Connected)
            return;

        clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, OnReceiveCallback, null);
    }

    private void OnReceiveCallback(IAsyncResult ar)
    {
        int bytesRead = clientSocket.EndReceive(ar);

        if (bytesRead > 0)
        {
            string message = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            AñadirMensajeCHat(message);
        }

        RecibirMensaje();
    }

    private void EnviarMensajeServidor(string message)
    {
        if (clientSocket == null || !clientSocket.Connected)
            return;

        byte[] sendBuffer = Encoding.ASCII.GetBytes(message);
        clientSocket.Send(sendBuffer);
    }



    private void AñadirMensajeCHat(string message)
    {
        chatMessages.Add(message);
        ActualizarMensajeChat();
    }

    private void ActualizarMensajeChat()
    {
        textoChat.text = string.Join("\n", chatMessages);
    }

    private void OnDestroy()
    {
        if (clientSocket != null && clientSocket.Connected)
            clientSocket.Close();
    }*/


    private ConexionServidor conexionServidor;


    TMP_Text textoChat;
    TMP_InputField mensajeInputChat;
    public TMP_InputField nameChat;


    private void Start()
    {
        textoChat = GameObject.Find("Chat").GetComponent<TMP_Text>();
        mensajeInputChat = GameObject.Find("MensajeInputChat").GetComponent<TMP_InputField>();

        // Inicializar la instancia de ConexionServidor
        conexionServidor = ConexionServidor.GetInstance();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (mensajeInputChat.text != "")
            {
                string mensajeChat = mensajeInputChat.text;
                Debug.Log(mensajeChat);
                string name = nameChat.text;
                Debug.Log(name);

                mensajeInputChat.text = "";
                nameChat.text = "";
                string mensaje = "9-" + mensajeChat + "-" + name;
                EnviarMensajeChat(mensaje);
                ActualizarMensajeChat(name,mensajeChat);
            }
        }
    }
    public void EnviarMensajeChat(string mensajeChat)
    {
        Debug.Log(mensajeChat);
        conexionServidor.EnviarMensajeServidor(mensajeChat);
    }
    public void ActualizarMensajeChat(string nombre,string mensaje)
    {
        textoChat.text += ">>" + nombre +"-"+mensaje+ "\n";
    }
}
