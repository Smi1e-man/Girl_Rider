using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public static CameraFollower Instance;

    public Vector3 speed;
    public Transform target;

    [Space]
    public bool autoUpdate = true;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - target.position;

        Instance = this;
    }

    private void LateUpdate()
    {
        if (autoUpdate)
            SetLerpedPos(target);
    }

    public void SetLerpedPos(Transform target)
    {
        Vector3 targetPos = target.position + _offset;
        Vector3 lerpedPos = Vector3.zero;

        lerpedPos.x = Mathf.Lerp(transform.position.x, targetPos.x, speed.x * Time.deltaTime);
        lerpedPos.y = Mathf.Lerp(transform.position.y, targetPos.y, speed.y * Time.deltaTime);
        lerpedPos.z = Mathf.Lerp(transform.position.z, targetPos.z, speed.z * Time.deltaTime);

        transform.position = lerpedPos;
    }

}
