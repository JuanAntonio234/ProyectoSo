using UnityEngine;
using UnityEngine.SceneManagement;

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
}