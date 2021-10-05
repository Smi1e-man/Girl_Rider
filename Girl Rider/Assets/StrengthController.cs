using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthController : MonoBehaviour
{
    [Range(0, 1f)] public float strength = 1;
    public float strengthDownSpeed = 0.3f;

    public Vector2 blendShapeWeightRange = new Vector2(-150, 150);
    public SkinnedMeshRenderer smr;
    
    void LateUpdate()
    {
        smr.SetBlendShapeWeight(0, Mathf.Lerp(blendShapeWeightRange.x, blendShapeWeightRange.y, strength));

    }
}
