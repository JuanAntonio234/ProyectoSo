using UnityEngine;
using UnityEngine.SceneManagement;

public class Vida : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private AudioSource DEADSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("trampa"))
        {
            Die();
        }
    }

    private void Die()
    {
        DEADSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void Restart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
