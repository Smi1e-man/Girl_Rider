using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAsset : MonoBehaviour
{
    public static StartAsset Instance;

    public Transform startCameraPoint;
    public Transform finishCameraPoint;

    private void Awake()
    {
        Instance = this;
    }
}
