using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    private Rigidbody2D rigidbody2;
    float Horizontal;
    public float fuerzaDeSalto;
    public GameObject balaPrefab;
    private float ultimoDisparo;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        //codigo para saltar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody2.AddForce(Vector2.up*fuerzaDeSalto);
        }
        if(Horizontal <0.0f)transform.localScale= new Vector3 (-1.0f, 1.0f, 1.0f);
        else if(Horizontal>0.0f)transform.localScale=new Vector3(1.0f, 1.0f, 1.0f);
        if(Input.GetKey(KeyCode.Q)&& Time.time>ultimoDisparo+0.30f)
        {
            Disparar();
            ultimoDisparo = Time.time;
        }
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

        GameObject bullet = Instantiate(balaPrefab, transform.position+direccion*0.1f, Quaternion.identity);
        bullet.GetComponent<BalaScript>().EstablecerDireccion(direccion);
    }
}