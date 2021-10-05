using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class StaticLiquid : MonoBehaviour
{
    public SplineComputer splineComputer;
    public TubeGenerator tubeGenerator;
    public ParticleSystem particles;
    public Transform blob;

    [Space]
    public Vector2 uvOffsetSpeed;

    public void Start()
    {

    }

    public void Update()
    {
        tubeGenerator.uvOffset += uvOffsetSpeed * Time.deltaTime;
    }
}
