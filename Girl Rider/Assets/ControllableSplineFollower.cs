using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class ControllableSplineFollower : MonoBehaviour
{
    public float speed = 5f;

    //public float maxOffset = 4;
    public float gravity = -10f;

    [Space]
    public float jumpForceDurationUp = 0.3f;
    public float jumpForceDurationDown = 1.5f;

    [Space]
    public Vector3 extraVelocity;
    public Vector3 extraVelocityFadingOut;

    [Space]
    public bool isStopped = true;
    public bool isOnGround;
    public bool previusIsOnGround;

    [Space]
    public SplineFollower splineFollower;
    public LevelPath levelPath;

    [Space]
    public float modelRotationSpeed = 8f;
    [Range(0, 1)] public float modelRotationPercent = 1f;

    [Space]
    public Transform model;
    public Collider hitTrigger;

    [Space]
    public float inputDeltaX;
    public float groundOffset;
    public float initialSpeed;
    public Vector3 actualSpeed;

    [Space]
    public float actualOffsetLeft;
    public float actualOffsetRight;

    private Vector3 _previousPos;
    private Quaternion _targetModelRotation;

    public virtual void Awake()
    {

        initialSpeed = speed;

        AutoSetOffset();
        groundOffset = splineFollower.motion.offset.y;

        splineFollower.follow = true;

        _targetModelRotation = model.rotation;
        _previousPos = transform.position;

        previusIsOnGround = isOnGround = true;


        actualOffsetLeft = -levelPath.halfWidth;
        actualOffsetRight = levelPath.halfWidth;
    }

    public virtual void Update()
    {
        Vector2 offset = splineFollower.motion.offset;
        
        offset.x = Mathf.Clamp(offset.x + inputDeltaX + extraVelocity.x * Time.deltaTime, actualOffsetLeft, actualOffsetRight);
        offset.y = Mathf.Clamp(offset.y + (extraVelocity.y + gravity) * Time.deltaTime, groundOffset, 1000f);

        splineFollower.motion.offset = offset;
        splineFollower.followSpeed = GetSpeed();

        previusIsOnGround = isOnGround;
        isOnGround = offset.y - groundOffset <= 0.01f;




        // animate model
        actualSpeed = transform.position - _previousPos;
        actualSpeed.y = 0;

        Vector3 forward = transform.forward;
        forward.y = 0;

        if (actualSpeed.sqrMagnitude > 0.01f)
            _targetModelRotation = Quaternion.LookRotation(Vector3.Lerp(forward, actualSpeed, modelRotationPercent), Vector3.up);

        model.rotation = Quaternion.Slerp(model.rotation, _targetModelRotation, modelRotationSpeed * Time.deltaTime);

        _previousPos = transform.position;
    }

    public virtual float GetSpeed()
    {
        return isStopped ? 0 : speed + extraVelocity.z;
    }

    public virtual void Jump(Vector3 jumpForce)
    {
        //Debug.Log(name + " Jump");
        DOTween.Kill(GetInstanceID());

        DOTween.Sequence()
            .Append(DOTween.To(() => extraVelocity, x => extraVelocity = x, jumpForce, jumpForceDurationUp).SetEase(Ease.InSine).SetId(GetInstanceID()))
            .Append(DOTween.To(() => extraVelocity, x => extraVelocity = x, Vector3.zero, jumpForceDurationDown).SetEase(Ease.InSine).SetId(GetInstanceID()))
            .InsertCallback(0.3f, ()=> hitTrigger.enabled = true)
            .SetId(GetInstanceID())
            ;
        
    }

    public void AutoSetOffset(bool setPercent = false, bool setOffetY = false)
    {
        Vector2 offset = splineFollower.motion.offset;

        SplineSample ss = new SplineSample();
        splineFollower.Project(transform.position, ss);

        offset.x = Vector3.Dot(transform.position - ss.position, ss.right);
        offset.y = 0;

        if (setOffetY)
            offset.y = Vector3.Dot(transform.position - ss.position, ss.up);

        if (setPercent)
            splineFollower.SetPercent(ss.percent);

        splineFollower.motion.offset = offset;
    }

    public void HandleActualOffsetLimitation(Transform obstacle, bool isEnter)
    {
        bool isLeft = Vector3.Dot(transform.right, obstacle.position - transform.position) < 0;
        //Debug.Log(isEnter + " | " + isLeft);
        if (isEnter)
        {
            if (isLeft)
                actualOffsetLeft = splineFollower.motion.offset.x;
            else
                actualOffsetRight = splineFollower.motion.offset.x;
        }
        else
        {
            if (isLeft)
                actualOffsetLeft = -levelPath.halfWidth;
            else
                actualOffsetRight = levelPath.halfWidth;
        }
    }

    public void SetLevelPath(LevelPath path)
    {
        levelPath = path;

        splineFollower.follow = false;
        splineFollower.spline = path.spline;
        splineFollower.RebuildImmediate();
        AutoSetOffset(true, true);
        splineFollower.follow = true;

        actualOffsetLeft = -levelPath.halfWidth;
        actualOffsetRight = levelPath.halfWidth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathSwitcher"))
        {
            LevelPathSwitcher pathSwitcher = other.GetComponent<LevelPathSwitcher>();

            LevelPath path = pathSwitcher.path;
            SetLevelPath(path);
        }
    }

    public void AnimateSpeed(float newSpeed, float duration)
    {
        DOTween.Kill(GetInstanceID() + " speed");
        DOTween.To(()=> speed, x=> speed = x, newSpeed, duration).SetId(GetInstanceID() + " speed");
    }
}
