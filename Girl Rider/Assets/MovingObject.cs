using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 3f;
    public float distanceForward = 3.5f;
    public float distanceBackward = 3.5f;
    public float dirRotationOffset = 0;

   
    [Space]
    public bool isMovingForward = true;


    private Vector3 _forwardPos;
    private Vector3 _backPos;
    private Vector3 _forwardDir;

    void Start()
    {
        _forwardDir = Quaternion.Euler(0, dirRotationOffset, 0) * transform.forward;
        _forwardPos = transform.position + _forwardDir * distanceForward;
        _backPos = transform.position - _forwardDir * distanceBackward;

        PerformNextMove();
    }

    public void PerformNextMove()
    {
        Vector3 targetPos = isMovingForward ? _forwardPos : _backPos;
        Vector3 forwardDir = (isMovingForward ? 1 : -1) * _forwardDir;

        isMovingForward = !isMovingForward;

        transform.DOMove(targetPos, speed).SetSpeedBased(true).OnComplete(PerformNextMove).SetEase(Ease.Linear);

    }

}
