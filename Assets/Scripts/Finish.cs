using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private AudioSource ENDSoundEffect;

    private bool final = false;

    private void Start()
    {
        ENDSoundEffect = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !final)
        {
            ENDSoundEffect.Play();
            final = true;
            Invoke("Final", 2f);


        }
    }

    private void Final()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
