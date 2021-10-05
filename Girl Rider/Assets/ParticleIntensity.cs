using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleIntensity : MonoBehaviour
{
    [System.Serializable]
    public class ParamPair
    {
        public ParticleSystem particleSystem;
        public float emissionMult = 1;
    }

    public List<ParamPair> paramPairs;

    public void SetEmissionRate(float p)
    {
        foreach (var pair in paramPairs)
        {
            ParticleSystem.EmissionModule emission = pair.particleSystem.emission;
            emission.rateOverTime = p * pair.emissionMult;

        }
    }
  
}
