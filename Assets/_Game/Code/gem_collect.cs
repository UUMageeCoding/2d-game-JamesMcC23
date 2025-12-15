using Unity.VisualScripting;
using UnityEngine;

public class gem_collect : MonoBehaviour
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
            Destroy(this,5f);
        }
    }

    
}
