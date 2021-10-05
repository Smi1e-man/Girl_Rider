using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinishAsset : MonoBehaviour
{
    public static FinishAsset Instance;

    public float winPercent = 0.5f;
    public float moveGirlToBoyDuration = 1;
    public float winDelay = 1f;

    [Space]
    public float cameraAnimationDuration = 1;

    [Space]
    public Transform girlTargetPoint;
    public AnimController boyAnimController;
    public Transform cameraTargetPoint;

    void Awake()
    {
        Instance = this;
    }

    public void AnimateCamera()
    {
        CameraSplineFollower.Instance.enabled = false;
        CameraSplineFollower.Instance.transform.DOMove(cameraTargetPoint.position, cameraAnimationDuration);
        CameraSplineFollower.Instance.transform.DORotateQuaternion(cameraTargetPoint.rotation, cameraAnimationDuration);

    }

}
