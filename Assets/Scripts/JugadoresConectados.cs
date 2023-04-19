using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerListUI : MonoBehaviour
{
    public Text TextListaJugadores;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            string ListaJugadores = "";
            foreach (var jugador in NetworkManager.singleton.client.connection.playerControllers)
            {
                ListaJugadores =+ jugador.gameObject.name + "\n";
            }
            TextListaJugadores.text = ListaJugadores;
        }
    }
}
