using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class JumperRider : MonoBehaviour
{
    public enum State { None, Running, Flying, Riding }

    public JumperVehicle currentVehicle;
    public State currentState;

    [Space]
    public ControllableSplineFollower controllableFollower;
    public AnimController animController;
    public DirtController verticalProgressController;
    public PositionUpdater progressBarPositionUpdater;
    public Transform jumpTarget;

    [Space]
    public bool isInputAllowed = true;
    public bool jumpOnRelease = true;


    public float horizontalSensivity = 25f;
    public float inputLerpSpeed = 10f;

    [Space]
    public bool isDead;
    public bool isAiming;

    private bool _isInputStarted;

    private Vector3 _previousMousePosition;
    private AnimController.DudeAnimation _rideAnim = AnimController.DudeAnimation.girl_sit;

    [Space]
    public float inputDeltaX;
    public float targetInputDeltaX;

    private float _startTime;

    private void Awake()
    {

    }

    private void Start()
    {
        SetState(State.Running);

        _startTime = Time.time;

        jumpTarget.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        if (Input.GetMouseButtonDown(0) && isInputAllowed)
            _isInputStarted = true;

        if (Input.GetMouseButtonUp(0))
        {
            _isInputStarted = false;

            if (!isAiming && jumpOnRelease)
                Jump();
            else if (isAiming)
                EndJump();
            
        }

        if (_isInputStarted)
            targetInputDeltaX = Mathf.Clamp((Input.mousePosition - _previousMousePosition).x / Screen.width * horizontalSensivity, -1, 1);
        else
            targetInputDeltaX = 0;

        inputDeltaX = Mathf.Lerp(inputDeltaX, targetInputDeltaX, inputLerpSpeed * Time.deltaTime);



        if (isAiming)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 deltaPos = (Input.mousePosition - _previousMousePosition) / Screen.width * horizontalSensivity;
                Vector3 moveDir = Vector3.zero;
                moveDir.x = deltaPos.y;
                moveDir.z = -deltaPos.x;
                jumpTarget.position += moveDir * 60 * Time.deltaTime;
            }
            
        }
        else
        {

            if (currentVehicle != null)
                currentVehicle.controllableFollower.inputDeltaX = inputDeltaX;
            else
                controllableFollower.inputDeltaX = inputDeltaX;

            //if (currentVehicle == null && Time.time - _startTime > 1 )
            //    SetState(controllableFollower.isOnGround ? State.Running : State.Running);

            if (currentVehicle == null && Time.time - _startTime > 1 && controllableFollower.isOnGround && !controllableFollower.previusIsOnGround)
                SetState(State.Running);
        }


        _previousMousePosition = Input.mousePosition;

    }

    public void Jump()
    {
        if (currentVehicle == null && !controllableFollower.isOnGround) return;

        Vector3 pos = transform.position;
        if (currentVehicle != null)
        {
            pos = currentVehicle.transform.position;
            ExitVehicle();

        }

        isAiming = true;
        SetState(State.Flying);

        controllableFollower.enabled = false;
        controllableFollower.splineFollower.follow = false;
        controllableFollower.hitTrigger.enabled = false;
        controllableFollower.isOnGround = false;

        controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.35f);
        transform.DORotateQuaternion(Quaternion.LookRotation(Vector3.right), 0.35f);

        transform.position = pos;
        pos.y = 5;
        pos.x += 1.5f;
        transform.DOMove(pos, 1f).SetEase(Ease.OutSine);


        Vector3 targetPos = transform.position;
        targetPos.y = 0.03f;
        targetPos.x += 5f;

        jumpTarget.position = targetPos;
        jumpTarget.gameObject.SetActive(true);
    }

    public void EndJump()
    {
        isAiming = false;
        jumpTarget.gameObject.SetActive(false);
        controllableFollower.hitTrigger.enabled = true;

        transform.DOMove(jumpTarget.position, 0.5f).SetEase(Ease.InSine).OnComplete(()=> {

            if (currentVehicle == null)
            {
                controllableFollower.enabled = true;
                controllableFollower.AutoSetOffset(true, true);
                controllableFollower.splineFollower.follow = true;
            }
            
        });

    }

    public void EnterVehicle(JumperVehicle vehicle)
    {
        currentVehicle = vehicle;
        vehicle.OnRiderEnter(this);

        controllableFollower.enabled = false;
        controllableFollower.splineFollower.follow = false;
        controllableFollower.hitTrigger.enabled = false;
        transform.SetParent(vehicle.transform, true);

        DOTween.Kill(GetInstanceID());
        Transform sittingPoint = vehicle.sittingPoints[vehicle.sittingPointIndex];

        Vector3[] path = new Vector3[] { (transform.localPosition + sittingPoint.localPosition) * 0.5f + Vector3.up * 1.2f, sittingPoint.localPosition };
        transform.DOLocalPath(path, 0.35f).SetEase(Ease.OutSine).SetId(GetInstanceID());

        transform.DOLocalRotateQuaternion(sittingPoint.localRotation, 0.35f).SetEase(Ease.OutSine);
        controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.35f);

        _rideAnim = vehicle.riderSittingAnim;
        SetState(State.Riding);


        progressBarPositionUpdater.target = vehicle.transform;
    }

    public void ExitVehicle()
    {
        DOTween.Kill(GetInstanceID());


        currentVehicle.OnRiderExit();

        Vector3 pos = transform.position;
        pos.x = currentVehicle.transform.position.x;
        pos.z = currentVehicle.transform.position.z;
        transform.position = pos;

        currentVehicle = null;


        transform.SetParent(transform.parent.parent, true);

        controllableFollower.enabled = true;
        controllableFollower.AutoSetOffset(true, true);
        controllableFollower.splineFollower.follow = true;
        controllableFollower.hitTrigger.enabled = false;
        DOVirtual.DelayedCall(0.3f, () => controllableFollower.hitTrigger.enabled = true).SetId(GetInstanceID());

        progressBarPositionUpdater.target = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle") && currentVehicle == null)
        {
            JumperVehicle vehicle = other.GetComponentInParent<JumperVehicle>();
            EnterVehicle(vehicle);
        }
        else if (other.CompareTag("Wall"))
        {
            controllableFollower.isStopped = true;
        }
        else if (other.CompareTag("JumpObstacle"))
        {
            Jump();
        }
        else if (other.CompareTag("FinishTrigger"))
        {
            /*
            isDead = true;
            if (currentVehicle) ExitVehicle();

            controllableFollower.enabled = false;
            controllableFollower.splineFollower.enabled = false;
            controllableFollower.hitTrigger.enabled = false;

            animController.PlayAnimation(AnimController.DudeAnimation.girl_walk);

            FinishAsset.Instance.AnimateCamera();
            //verticalProgressController.progressBar.gameObject.SetActive(false);
            verticalProgressController.progressBar.GetComponent<Billboard>().allowZ = true;

            verticalProgressController.enabled = false;

            Vector3 lookDir = FinishAsset.Instance.girlTargetPoint.position - transform.position;
            lookDir.y = 0;

            DOTween.Sequence()
                .Append(transform.DOMove(FinishAsset.Instance.girlTargetPoint.position, FinishAsset.Instance.moveGirlToBoyDuration))
                .Join(transform.DORotateQuaternion(Quaternion.LookRotation(lookDir), 0.3f))
                .Join(controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.3f))
                .AppendCallback(() =>
                {
                    transform.DORotateQuaternion(FinishAsset.Instance.girlTargetPoint.rotation, 0.4f);
                    animController.PlayAnimation(AnimController.DudeAnimation.girl_finish);
                })
                .AppendInterval(FinishAsset.Instance.winDelay)
                .AppendCallback(() =>
                {
                    bool isWin = verticalProgressController.progress < FinishAsset.Instance.winPercent;

                    if (isWin)
                    {
                        animController.PlayAnimation(AnimController.DudeAnimation.girl_finish_victory);
                        FinishAsset.Instance.boyAnimController.PlayAnimManFinishVictory();

                    }
                    else
                    {
                        animController.PlayAnimation(AnimController.DudeAnimation.girl_finish_fail);
                        FinishAsset.Instance.boyAnimController.PlayAnimManFinishFail();

                    }

                    GameController.Instance.OnLevelCompleted(isWin, 1.5f);
                })
                ;
            */
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            controllableFollower.isStopped = false;
        }
    }

    public void OnVehicleHitsWall()
    {
        Jump();
        //ExitVehicle();
    }

    private void SetState(State s)
    {
        if (currentState == s) return;
        //Debug.Log("new state: " + s.ToString());
        currentState = s;

        switch (s)
        {
            case State.Running:
                animController.PlayAnimation(AnimController.DudeAnimation.girl_walk);
                break;

            case State.Flying:
                animController.PlayAnimation(AnimController.DudeAnimation.girl_fall);
                break;


            case State.Riding:
                animController.PlayAnimation(_rideAnim);
                break;
        }
    }
}
