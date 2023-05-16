using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField messageInput;
    public TMP_Text chatText;
    public string IP = "192.168.56.102";
    public int puerto= 5052;

    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[1024];
    private List<string> chatMessages = new List<string>();

    private void Start()
    {
        ConectarServidor();
    }

    private void ConectarServidor()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            clientSocket.Connect(IP, puerto);
            Debug.Log("conectado al server");
            RecibirMensaje();
        }
        catch (SocketException ex)
        {
            Debug.Log("fallo al conectarse al server: " + ex.Message);
        }
    }

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

    public void EnviarMensajeChat()
    {
        string message = messageInput.text;

        if (!string.IsNullOrEmpty(message))
        {
            EnviarMensajeServidor(message);
            messageInput.text = string.Empty;
        }
    }

    private void AñadirMensajeCHat(string message)
    {
        chatMessages.Add(message);
        ActualizarMensajeChat();
    }

    private void ActualizarMensajeChat()
    {
        chatText.text = string.Join("\n", chatMessages);
    }

    private void OnDestroy()
    {
        if (clientSocket != null && clientSocket.Connected)
            clientSocket.Close();
    }
}
