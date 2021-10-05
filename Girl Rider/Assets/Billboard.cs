using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cameraT;

    public bool allowX = true;
    public bool allowY = true;
    public bool allowZ = true;


    private void Awake()
    {
        _cameraT = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 dir = transform.position - _cameraT.position;

        if (!allowX)
            dir.x = 0;

        if (!allowY)
            dir.y = 0;

        if (!allowZ)
            dir.z = 0;
        
        transform.forward = dir;
    }
}
