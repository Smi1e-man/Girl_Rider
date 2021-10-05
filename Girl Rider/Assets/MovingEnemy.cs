using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    public float speed = 3f;
    public float distanceForward = 3.5f;
    public float distanceBackward = 3.5f;

    [Space]
    public AnimController.DudeAnimation walkAnim = AnimController.DudeAnimation.girl_walk;
    public AnimController.DudeAnimation onHitAnim = AnimController.DudeAnimation.girl_idle;
    public float rotationDur = 0.4f;

    [Space]
    public float dirtDeltaValue = 0.3f;

    [Space]
    public bool isMovingForward = true;

    [Space]
    public AnimController animController;

    private Vector3 _forwardPos;
    private Vector3 _backPos;
    private Vector3 _forwardDir;

    private bool _isFirstLoop = true;


    void Start()
    {
        animController.PlayAnimation(walkAnim);

        _forwardDir = transform.forward;
        _forwardPos = transform.position + _forwardDir * distanceForward;
        _backPos = transform.position - _forwardDir * distanceBackward;


        PerformNextMove();
    }

    public void PerformNextMove()
    {
        Vector3 targetPos = isMovingForward ? _forwardPos : _backPos;
        Vector3 forwardDir = (isMovingForward ? 1 : -1) * _forwardDir;

        isMovingForward = !isMovingForward;


        if (_isFirstLoop)
        {
            _isFirstLoop = false;

            transform.DOMove(targetPos, speed).SetSpeedBased(true).OnComplete(PerformNextMove).SetEase(Ease.Linear);
        }
        else
        {
            transform.DORotateQuaternion(Quaternion.LookRotation(forwardDir), rotationDur).OnComplete(() =>
            {
                transform.DOMove(targetPos, speed).SetSpeedBased(true).OnComplete(PerformNextMove).SetEase(Ease.Linear);
            });

        }

    }

    public void OnHit(Transform rider)
    {
        DOTween.Kill(transform);

        transform.DORotateQuaternion(Quaternion.LookRotation(rider.transform.position - transform.position), rotationDur);

        animController.PlayAnimation(onHitAnim);
        //DOVirtual.DelayedCall(rotationDur * 0.5f, () => animController.PlayAnimation(onHitAnim));
    }
}
