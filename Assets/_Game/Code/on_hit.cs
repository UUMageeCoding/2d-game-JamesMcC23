using UnityEngine;

public class on_hit : MonoBehaviour, attackable_object
{
    private ParticleSystem particle_system;
    public void is_hit()
    {
        particle_system.Play();
    }
}
