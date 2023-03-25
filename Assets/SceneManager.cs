using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject m_registrarseUI = null;
    [SerializeField] private GameObject m_iniciarSesionUI = null;


    public void MostarRegistro()
    {
        m_registrarseUI.SetActive(true);
        m_iniciarSesionUI.SetActive(false);
    }
    public void MostrarInicioDeSesion()
    {
        m_registrarseUI.SetActive(false);
        m_iniciarSesionUI.SetActive(true);
    }
}