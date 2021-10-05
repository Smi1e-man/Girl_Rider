using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMover : MonoBehaviour
{
    public float startDelay;
    [Space]
    public float duration = 0.5f;
    public float delay = 0.5f;

    [Space]
    public Vector3 from;
    public Vector3 to;
    public bool direction = true;


    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        transform.position = direction ? from : to;
        _rigidbody.MovePosition(direction ? from : to);

        DOTween.Sequence().InsertCallback(startDelay, StartAnimation);
    }

    private void StartAnimation()
    {
        
        DOTween.Sequence().SetLoops(-1, LoopType.Restart).SetUpdate(UpdateType.Fixed)
            .Append(_rigidbody.DOMove(direction ? to : from, duration).SetEase(Ease.InOutSine))
            .AppendInterval(delay)
            .Append(_rigidbody.DOMove(direction ? from : to, duration).SetEase(Ease.InOutSine))
            .AppendInterval(delay)
            ;
    }
}
