using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMeshOnStart : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

}
