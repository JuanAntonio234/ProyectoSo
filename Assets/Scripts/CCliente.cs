using JetBrains.Annotations;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class CCliente : MonoBehaviour

{
    private Socket clientSocket;

    private byte[] recibirbuffer = new byte[1024];
    private StringBuilder recibirData = new StringBuilder();

    // Start is called before the first frame update
    void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Connect("127.0.0.1", 8080);
    }
    private void Connect(string ipAddress, int port)
    {
        try
        {
            IPAddress serverIPAdress = IPAddress.Parse(ipAddress);
            IPEndPoint serverEP = new IPEndPoint(serverIPAdress, port);
            clientSocket.BeginConnect(serverEP, connectCallBa, null);
        }
        catch (Exception e)
        {
            Debug.Log("Socket exception" + e.Message);

        }
    }
    private void connectCallBa(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndConnect(ar);
            Debug.Log("conectado al servidor");
            StartReceivingData();
        }
        catch (Exception e)
        {
            Debug.Log("Socket Exception: " + e.Message);
        }
    }
    private void StartReceivingData()
    {
        clientSocket.BeginReceive(recibirbuffer, 0, recibirbuffer.Length, SocketFlags.None, Recieve, null);
    }
    private void SendCallba(IAsyncResult ar)
    {
        try
        {
            int enviarBytes = clientSocket.EndSend(ar);
            Debug.Log("Enviar " + enviarBytes + " bytes al servidor");
        }
        catch (Exception e)
        {
            Debug.Log("Socket exception: " + e.Message);
        }
    }
    private void SendData(string data)
    {
        byte[] dataBytes = Encoding.ASCII.GetBytes(data);
        clientSocket.BeginSend(dataBytes, 0, dataBytes.Length, SocketFlags.None, SendCallba, null);
    }
    private void Recieve(IAsyncResult ar)
    {
        try
        {
            int bytesRecibidos = clientSocket.EndReceive(ar);
            if (bytesRecibidos > 0)
            {
                byte[] data = new byte[bytesRecibidos];
                Array.Copy(recibirbuffer, data, bytesRecibidos);
                recibirData.Append(Encoding.ASCII.GetString(data));
                StartReceivingData();
            }
            else
            {
                Debug.Log("Conexion cerrada por el servidor");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Socket exception: " + e.Message);
        }
    }
    private void OnApplicationQuit()
    {
        clientSocket.Close();
    }
}
