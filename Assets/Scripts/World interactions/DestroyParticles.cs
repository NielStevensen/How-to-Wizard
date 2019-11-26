using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    private ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        if (particles = null) Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(particles != null)
        {
            if (!particles.IsAlive())
            {
                Destroy(particles);
            }
        }
    }
}
