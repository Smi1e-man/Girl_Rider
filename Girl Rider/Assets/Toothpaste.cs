using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Dreamteck.Splines;
using DG.Tweening;

public class Toothpaste : MonoBehaviour
{
    [OnValueChanged("UpdateProgress")] [Range(0, 1f)] public float progress;

    [Space]
    public SplineComputer splineComputer;
    public TubeGenerator tubeGenerator;
    public MeshRenderer mr;
    public ParticleSystem particles;
    public Transform content;
    public Transform blob;
    public Transform bucketModel;

   
    [Space]
    public AnimationCurve clipFrom;
    public AnimationCurve clipTo;

    [Space]
    public Vector2 uvScale;
    public Vector2 uvOffsetSpeed;


    [Space]
    public AnimationCurve contentCurve;

    public Vector2 contentScale;
    public Vector2 contentPosY;

    [Space]
    public AnimationCurve blobCurve;
    public Vector2 blobScale;


    [Space]
    public float animationDuration = 0.7f;
    public float startPartilcesDelay = 0.4f;



    public void SetProgress(float p)
    {
        progress = p;
        p = Mathf.Clamp01(p);


        tubeGenerator.clipTo = clipTo.Evaluate(p);
        tubeGenerator.clipFrom = clipFrom.Evaluate(p);

        tubeGenerator.uvScale = tubeGenerator.CalculateLength() / 10f * uvScale;
        tubeGenerator.uvOffset += uvOffsetSpeed * Time.deltaTime;


        float contentP = contentCurve.Evaluate(p);
        float cs = Mathf.Lerp(contentScale.x, contentScale.y, contentP);
        content.localScale = new Vector3(cs, content.localScale.y, cs);
        Vector3 cp = content.localPosition;
        cp.y = Mathf.Lerp(contentPosY.x, contentPosY.y, contentP);
        content.localPosition = cp;


        float blobP = blobCurve.Evaluate(p);
        blob.localScale = Vector3.one * Mathf.Lerp(blobScale.x, blobScale.y, blobP);

        mr.enabled = p > 0.001f && p < 0.999f;

    }

    [Button]
    public void Play()
    {
        progress = 0;

        DOVirtual.Float(0, 1, animationDuration, (p) => {
            SetProgress(p);
        }).SetEase(Ease.Linear);

        DOVirtual.DelayedCall(startPartilcesDelay, particles.Play);
        DOVirtual.DelayedCall(animationDuration + 0.1f, particles.Stop);

    }

    private void UpdateProgress()
    {
        SetProgress(progress);

    }
}
