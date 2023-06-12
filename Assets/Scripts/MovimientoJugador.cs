using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    private Rigidbody2D rigidbody2;
    float Horizontal;
    public float fuerzaDeSalto;
    public GameObject balaPrefab;
    private float ultimoDisparo;

    private Vector3 posicionActual;
    private Socket clienteSocket;
     private GameObject prefabJugador; // Asigna el prefab del jugador 
    private GameObject jugadorLocal;

    // Start is called before the first frame update
    void Start()
    {
        jugadorLocal = Instantiate(prefabJugador, Vector3.zero, Quaternion.identity);

        rigidbody2 = GetComponent<Rigidbody2D>();
        posicionActual = transform.position;

        // Obtengo la referencia al socket cliente  desde CCliente
        clienteSocket = CCliente.clienteSocket;
    }

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        //codigo para saltar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody2.AddForce(Vector2.up * fuerzaDeSalto);
        }
        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (Input.GetKey(KeyCode.Q) && Time.time > ultimoDisparo + 0.30f)
        {
            Disparar();
            ultimoDisparo = Time.time;
        }
        // Llamar a la función para actualizar la posición del jugador
        ActualizarPosicionJugador();
    }
    private void FixedUpdate()
    {
        rigidbody2.velocity = new Vector2(Horizontal, rigidbody2.velocity.y);
    }
    private void Disparar()
    {
        Vector3 direccion;
        if (transform.localScale.x == 1.0f) direccion = Vector2.right;
        else direccion = Vector2.left;

        GameObject bullet = Instantiate(balaPrefab, transform.position + direccion * 0.1f, Quaternion.identity);
        bullet.GetComponent<BalaScript>().EstablecerDireccion(direccion);
    }
    // Función para actualizar la posición del jugador y enviarla al servidor
    void ActualizarPosicionJugador()
    {
        Vector3 nuevaPosicion = transform.position;

        if (nuevaPosicion != posicionActual)
        {
            posicionActual = nuevaPosicion;
            EnviarPosicion((int)nuevaPosicion.x, (int)nuevaPosicion.y);
        }
    }
    // Función para enviar la posición al servidor
    void EnviarPosicion(int posX, int posY)
    {
        try
        {
            string mensaje = "6-" + posX.ToString() + "-" + posY.ToString();
            byte[] buffer = Encoding.ASCII.GetBytes(mensaje);
            clienteSocket.Send(buffer);
        }
        catch (SocketException ex)
        {
            Debug.Log("Error de socket: " + ex.Message);
        }
    }
}