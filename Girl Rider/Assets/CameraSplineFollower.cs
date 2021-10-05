using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class CameraSplineFollower : MonoBehaviour
{
    public static CameraSplineFollower Instance;

    public float forwardSpeed = 7;
    //public float sideSpeed = 0;
    public float upSpeed = 3;

    public float rotationSpeed = 7;

    [Space]
    public Rider target;

    private Vector3 _offset;
    private Quaternion _rotationOffset;
   
    void Awake()
    {
        Instance = this;

        _offset = Quaternion.Inverse(target.transform.rotation) * (transform.position - target.transform.position) ;

        Vector3 lookDir = target.transform.position - transform.position;
        lookDir.y = 0;
        _rotationOffset = Quaternion.Inverse(target.transform.rotation/*Quaternion.LookRotation(lookDir)*/) * transform.rotation;

        //_splineFollower = target.GetComponent<SplineFollower>();
    }

    private void LateUpdate()
    {
        SetLerpedPos();
    }

    public void SetLerpedPos()
    {
        ControllableSplineFollower follower = target.currentVehicle == null ? target.controllableFollower : target.currentVehicle.controllableFollower;
        SplineSample ss = follower.splineFollower.modifiedResult;// spline.Project(target.position);

        Vector3 forward = ss.forward;
        forward.y = 0;
        Quaternion forwardRotation = Quaternion.LookRotation(forward);

        Vector3 prePos = follower.transform.position;
        //prePos.y = Mathf.Clamp(prePos.y, -0.5f, 2f);
        //prePos.y = ss.position.y;
        Vector3 targetPos = prePos + forwardRotation/* target.transform.rotation*/ * _offset;

        Vector3 lerpedPos = Vector3.zero;// Vector3.Lerp(transform.position, targetPos, forwardSpeed * Time.deltaTime);
        lerpedPos.x = Mathf.Lerp(transform.position.x, targetPos.x, forwardSpeed * Time.deltaTime);
        lerpedPos.z = Mathf.Lerp(transform.position.z, targetPos.z, forwardSpeed * Time.deltaTime);
        lerpedPos.y = Mathf.Lerp(transform.position.y, targetPos.y, upSpeed * Time.deltaTime);


        Vector3 lookDir = target.transform.position - transform.position;
        lookDir.y = 0;
        Quaternion targetRot = forwardRotation/*Quaternion.LookRotation(lookDir)*/ * _rotationOffset;
        Quaternion lerpedRot = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        transform.SetPositionAndRotation(lerpedPos, lerpedRot);
    }

    public void GetStartPosition(out Vector3 pos, out Quaternion rot)
    {
        ControllableSplineFollower follower = target.currentVehicle == null ? target.controllableFollower : target.currentVehicle.controllableFollower;
        SplineSample ss = follower.splineFollower.modifiedResult;// spline.Project(target.position);

        Vector3 forward = ss.forward;
        forward.y = 0;
        Quaternion forwardRotation = Quaternion.LookRotation(forward);

        pos = follower.transform.position/*ss.position*/ + forwardRotation/* target.transform.rotation*/ * _offset;

        Vector3 lookDir = target.transform.position - transform.position;
        lookDir.y = 0;
        rot = forwardRotation/*Quaternion.LookRotation(lookDir)*/ * _rotationOffset;
    }
}
