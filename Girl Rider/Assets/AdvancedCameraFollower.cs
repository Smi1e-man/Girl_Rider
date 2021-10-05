using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedCameraFollower : MonoBehaviour
{
    public static AdvancedCameraFollower Instance;

    public float speed = 10;
    public Transform target;

    [Space]
    public bool autoUpdate = true;

    private Vector3 _offset;
    private Quaternion _rotationOffset;
    private Quaternion _initialRot;
    void Start()
    {
        _offset = transform.position - target.position;
        _rotationOffset = target.rotation * Quaternion.Inverse(Quaternion.LookRotation(-_offset));
        _initialRot = transform.rotation;
        Instance = this;
    }

    private void LateUpdate()
    {
        if (autoUpdate)
            SetLerpedPos(target);
    }

    public void SetLerpedPos(Transform target)
    {
        Vector3 targetPos = target.position + target.rotation * _offset;
        
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
    }

}
