using UnityEngine.SceneManagement;
using UnityEngine;

public class CambiarEscena : MonoBehaviour
{
    public void IrAIniciarSesion()
    {
        SceneManager.LoadScene("IniciarSesion");
    }
    public void IrARegistrarse()
    {
        SceneManager.LoadScene("Registrarse");
    }
    public void IrAOpciones()
    {
        SceneManager.LoadScene("Opciones");
    }
    public void IrAMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    public void IrAConsulta()
    {
        SceneManager.LoadScene("Consulta");
    }
    public void IrAMenuJuego()
    {
        SceneManager.LoadScene("MenuJuego");
    }
    public void IrAJuego()
    {
        SceneManager.LoadScene("Juego");
    }
}
