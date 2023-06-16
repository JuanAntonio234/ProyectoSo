using UnityEngine.SceneManagement;
using UnityEngine;

public class CambiarEscena : MonoBehaviour
{
    public void IrAIniciarSesion()
    {
        SceneManager.LoadSceneAsync("IniciarSesión");
    }
    public void IrARegistrarse()
    {
        SceneManager.LoadSceneAsync("Registrarse");
    }
    public void IrAOpciones()
    {
        SceneManager.LoadSceneAsync("Opciones");
    }
    public void IrAMenu()
    {
        SceneManager.LoadSceneAsync("MenuPrincipal");
    }
    public void IrAConsulta()
    {
        SceneManager.LoadSceneAsync("Consulta");
    }
    public void IrAMenuJuego()
    {
        SceneManager.LoadSceneAsync("MenuJuego");
    }
    public void IrAJuego()
    {
        SceneManager.LoadSceneAsync("Juego");
    }
}
