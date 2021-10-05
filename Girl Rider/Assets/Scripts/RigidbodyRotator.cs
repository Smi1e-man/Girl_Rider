using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyRotator : MonoBehaviour
{
    public float startDelay;
    [Space]
    public float duration = 0.5f;
    public float delay = 0.5f;

    [Space]
    public Vector3 angle;
    public bool direction = true;


    public bool isOneWay;

    public bool isComeBack;
    Quaternion _startRotation;


    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();


        if (!direction)
            angle *= -1;

        DOTween.Sequence().InsertCallback(startDelay, StartAnimation);

    }

    private void StartAnimation()
    {
        if (isOneWay)
        {
            DOTween.Sequence().SetLoops(-1, LoopType.Restart).SetUpdate(UpdateType.Fixed)
               .Append(_rigidbody.DORotate(angle, duration).SetEase(Ease.InOutSine).SetRelative(true))
               .AppendInterval(delay)
               ;
        }
        else if (isComeBack)
        {
            DOTween.Sequence().SetLoops(-1, LoopType.Yoyo).SetUpdate(UpdateType.Fixed)
                .AppendInterval(delay)
                .Append(_rigidbody.DORotate(angle, duration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine).SetRelative(true))
                .AppendInterval(delay)
                ;
        }
        else
        {
            DOTween.Sequence().SetLoops(-1, LoopType.Restart).SetUpdate(UpdateType.Fixed)
                .Append(_rigidbody.DORotate(angle, duration).SetEase(Ease.InOutSine).SetRelative(true))
                .AppendInterval(delay)
                .Append(_rigidbody.DORotate(-angle, duration).SetEase(Ease.InOutSine).SetRelative(true))
                .AppendInterval(delay)
                ;
        }
    }
}
