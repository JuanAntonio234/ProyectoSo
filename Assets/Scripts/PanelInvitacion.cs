using UnityEngine;

public class PanelInvitacion : MonoBehaviour
{
    public GameObject invitacion;
    // Start is called before the first frame update
    void Start()
    {
        CerrarPanel();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AbrirPanel()
    {
        invitacion.SetActive(true);

    }
    public void CerrarPanel()
    {
        invitacion.SetActive(false);
    }
}
