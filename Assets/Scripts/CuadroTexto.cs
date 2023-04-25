using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CuadroTexto : MonoBehaviour
{
    public GameObject cuadroTexto;
    public UnityEngine.UI.Text texto;
    private string nombreJugador;

    public void MostrarCuadroTexto()
    {
        cuadroTexto.SetActive(true);
    }
    public void CerrarCuadroTexto()
    {
        nombreJugador = cuadroTexto.GetComponent < InputField> ().text;
        cuadroTexto.SetActive(false);
    }

}
