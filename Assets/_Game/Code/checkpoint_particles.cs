using UnityEngine;

public class checkpoint_particles : MonoBehaviour
{
    private ParticleSystem particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "player")
        {
            particles.Play();
        }
    }







}
