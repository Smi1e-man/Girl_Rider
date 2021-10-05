using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtParticlesSpawner : MonoBehaviour
{
    //public float delay = 0.05f;
    //public LayerMask dirtMask;
    public ParticleSystem particles;

    //private float _lastSpawnTime;

    /*
    private void Update()
    {
        if (Time.time - _lastSpawnTime > delay)
        {
            _lastSpawnTime = Time.time;
            Spawn();
        }
    }

    public void Spawn()
    {
        particles.Stop();
        if (Physics.Raycast(new Ray(transform.position + Vector3.up * 10f, Vector3.down), out RaycastHit hit, 30f, dirtMask))
        {
            particles.transform.position = hit.point;
            particles.transform.localPosition = new Vector3(0, particles.transform.localPosition.y, 0);
            particles.Play();
        }

    }*/
}
