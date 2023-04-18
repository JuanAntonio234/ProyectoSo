/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using UnityEngine.UI;

public class Cliente : MonoBehaviour
{
    public Button ConectarServidor;
    public Button DesconectarseServidor;

    public InputField NameInput;
    public InputField PasswordInput;



    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public void Conectarse()
     {
        IPAddress direccion = IPAddress.Parse("127.0.0.1");
        IPEndPoint ip = new IPEndPoint(direccion, 9050);

        //Creamos el socket 
       
        try
        {
            server.Connect(ip);
        }
        catch(SocketException ex)
        {
            Console.WriteLine("No se ha podido conectar con el servudor" );

            return;
        }
        string name = NameInput.text;
        string password = PasswordInput.text;

        string mensaje= "1/"+name+"/"+ password;
        //enviamos al servidor el nombre
        byte[]msg=System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);

        //recibimos la respuesta
        byte []mg2 = new byte[80];
        server.Receive(mg2);
        mensaje=Encoding.ASCII.GetString(mg2).Split('\0')[0];

        if (mensaje == "SI")
        {
            //consultas
        }
        else
        {
            //consultas
        }

    }
    public void Desconectarse()
    { 
        //Mensaje de desconexión
        string mensaje = "0/";

        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);

        // Nos desconectamos

        server.Shutdown(SocketShutdown.Both);
        server.Close();
    }
    void Start()
    {
        ConectarServidor.onClick.AddListener(Conectarse);
        DesconectarseServidor.onClick.AddListener(Desconectarse);
    }
}
*/