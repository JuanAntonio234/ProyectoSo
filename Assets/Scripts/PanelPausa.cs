using UnityEngine;

public class PanelPausa : MonoBehaviour
{
    public GameObject pausa;

    // Update is called once per frame
    void Update()
    {

    }
    public void AbrirPausa()
    {
        pausa.SetActive(true);

    }
    public void CerrarPausa()
    {
        pausa.SetActive(false);
    }
}
