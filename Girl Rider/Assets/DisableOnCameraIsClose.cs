using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnCameraIsClose : MonoBehaviour
{
    public float dist = 5;
    private Transform _cam;
    private void Start()
    {
        _cam = Camera.main.transform;
        InvokeRepeating("Check", 0.3f, 0.3f);
    }

    private void Check()
    {
        if (Vector3.Distance(_cam.position, transform.position) < dist)
        {
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }
}
