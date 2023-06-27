using Client;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private ConexionServidor conexionServidor;

   private TextMeshProUGUI textoChat;
    TMP_InputField mensajeInputChat;
    public TMP_InputField nameChat;


    private void Start()
    {
        textoChat = GameObject.Find("Chat").GetComponent<TextMeshProUGUI>();
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
            }
        }
    }
    public void EnviarMensajeChat(string mensajeChat)
    {
        Debug.Log(mensajeChat);
        conexionServidor.EnviarMensajeServidor(mensajeChat);
    }
    public void ActualizarMensajeChat(string nombre, string mensaje)
    {
        textoChat.text += ">>" + nombre + "-" + mensaje + "\n";
    }
}
