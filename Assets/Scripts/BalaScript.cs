using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaScript : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    public float velocidad;
    private Vector2 Direccion;

    public float tiempoVida = 2f; // Duración de vida de la bala en segundos

    private float tiempoInicio;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        tiempoInicio = Time.time;
    }
    
    // Update is called once per frame
    void Update()
    {
        float tiempoTranscurrido = Time.time - tiempoInicio;

        if (tiempoTranscurrido >= tiempoVida)
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        Rigidbody2D.velocity = Direccion * velocidad;
    }
    public void EstablecerDireccion(Vector2 direccion)
    {
        Direccion = direccion;
    }
}
