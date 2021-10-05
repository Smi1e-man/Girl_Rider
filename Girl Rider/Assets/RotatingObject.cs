using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float duration = 0.5f;
    public float angleForward = 90f;
    public float angleBackward = 90f;

    [Space]
    public bool isMovingForward = true;


    private Quaternion _forwardAnlge;
    private Quaternion _backAngle;

    void Start()
    {
        _forwardAnlge = transform.rotation * Quaternion.Euler(0, angleForward, 0);
        _backAngle = transform.rotation * Quaternion.Euler(0, -angleBackward, 0);

        PerformNextMove();
    }

    public void PerformNextMove()
    {
        Quaternion targetRot = isMovingForward ? _forwardAnlge : _backAngle;

        isMovingForward = !isMovingForward;

        transform.DORotateQuaternion(targetRot, duration).OnComplete(PerformNextMove).SetEase(Ease.Linear);

    }

}
