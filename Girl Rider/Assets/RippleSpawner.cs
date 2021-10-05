using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Water2DTool;

public class RippleSpawner : MonoBehaviour
{
    public float radius = 0.5f;
    public float strength = -0.1f;
    //public float delay = 0.05f;
    //public LayerMask waterMask;

    [Space]
    //public Water2D_Ripple water2D_Ripple;

    private float _lastSpawnTime;

    /*
    private void Update()
    {
        if (Time.time - _lastSpawnTime > delay)
        {
            _lastSpawnTime = Time.time;
            Spawn();
        }
    }*/

    public void Spawn()
    {
        /*
        if (water2D_Ripple == null)
        {
            if (Physics.Raycast(new Ray(transform.position + Vector3.up * 10f, Vector3.down), out RaycastHit hit, 30f, waterMask))
            {
                water2D_Ripple = hit.collider.GetComponent<Water2D_Ripple>();
            }
        }*/

        //if (water2D_Ripple != null)
        //{
        //    water2D_Ripple.AddRippleAtPosition(transform.position, radius, strength);
        //}
    }
}
