using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public void IrAIniciarSesion()
    {
        SceneManager.LoadSceneAsync("IniciarSesion");
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
        SceneManager.LoadSceneAsync("Gameplay");
    }
    public void IrAJuegoCompletado()
    {
        SceneManager.LoadSceneAsync("JuegoCompletado");
    }
    public void IrADarseDeBaja()
    {
        SceneManager.LoadSceneAsync("DarDeBaja");
    }
    public void IrAInvitar()
    {
        SceneManager.LoadSceneAsync("Invitacion");
    }
}
