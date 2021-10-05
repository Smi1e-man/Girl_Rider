using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirrorer : MonoBehaviour
{
    void Start()
    {
        if (Random.value > 0.5f)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                Vector3 pos = t.localPosition;

                pos.z *= -1;

                t.localPosition = pos;
            }
        }
        
    }

}
