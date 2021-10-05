using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public enum State { None, Idle, Running, Falling}
    public State currentState;

    [Space]
    public int sittingPointIndex = 0;
    public AnimController.DudeAnimation vehicleSittingAnim = AnimController.DudeAnimation.man_run_bring;
    public AnimController.DudeAnimation riderSittingAnim = AnimController.DudeAnimation.girl_sit;

    [Space]
    public bool isDead;

    [Space]
    public Collider hitTrigger;
    public List<Transform> sittingPoints;
    public ControllableSplineFollower controllableFollower;
    public AnimController animController;
    public StrengthController strengthController;
    public DirtController dirtController;
    public Transform headTransform;

    [Space]
    public Rider rider;

    private static int _IDLE_ANIM_INDEX;

    private void Start()
    {
        SetState(State.Idle);
        _IDLE_ANIM_INDEX++;
    }

    private void Update()
    {
        if (currentState == State.Running)
        {
            if (strengthController.strength > 0)
            {
                strengthController.strength -= strengthController.strengthDownSpeed * Time.deltaTime;

                if (strengthController.strength <= 0 && !isDead)
                {
                    Kill();
                }
            }
        }
    }

    public void Kill()
    {
        isDead = true;

        dirtController.enabled = false;
        hitTrigger.gameObject.SetActive(false);

        rider.ExitVehicle();
        //rider.Jump();
    }

    public void OnRiderEnter(Rider r)
    {
        rider = r;

        RunVehicle();
    }

    public void OnRiderExit()
    {
        rider = null;

        StopVehicle();
    }

    public void OnRiderExitWithJump()
    {
        rider = null;

        isDead = true;
        hitTrigger.enabled = false;
        dirtController.enabled = false;
        hitTrigger.gameObject.SetActive(false);

        //controllableFollower.isStopped = true;


        DOTween.Kill(GetInstanceID());
        DOVirtual.Float(1, 0, 2f, (p) => {
            controllableFollower.speed = controllableFollower.initialSpeed * p;
        }).SetEase(Ease.OutCubic);//.OnComplete(() => controllableFollower.splineFollower.follow = false).SetId(GetInstanceID());


        controllableFollower.Jump(new Vector3(0, 10f, 1f));

        controllableFollower.groundOffset = -30;
        //DOVirtual.DelayedCall(0.3f, () => controllableFollower.groundOffset = -30);



        SetState(State.Falling);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isDead) return;
        if (other.CompareTag("Untagged")) return;

        if (other.CompareTag("KnockObstacle"))
        {
            if (rider != null)
            {
                rider.ExitVehicle();
            }
            else
            {
                StopVehicle();
            }
        }
        else if (other.CompareTag("StopObstacle"))
        {
            controllableFollower.isStopped = true;
        }
        else if(other.CompareTag("LongitudinalWall"))
        {
            controllableFollower.HandleActualOffsetLimitation(other.transform, true);
        }
        else if (other.CompareTag("JumpObstacle"))
        {
            isDead = true;

            rider.Jump();
        }
        else if (other.CompareTag("Hatch"))
        {
            OnHatch(other.transform.position);
            DOVirtual.DelayedCall(0.1f, other.GetComponentInChildren<ParticleSystem>().Play);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StopObstacle"))
        {
            controllableFollower.isStopped = false;
        }
        else if (other.CompareTag("LongitudinalWall"))
        {
            controllableFollower.HandleActualOffsetLimitation(other.transform, false);
        }
    }

    private void OnHatch(Vector3 hatchPos)
    {
        isDead = true;
        currentState = State.None;

        //controllableFollower.enabled = false;
        controllableFollower.splineFollower.follow = false;
        controllableFollower.hitTrigger.enabled = false;

        animController.PlayAnimation(AnimController.DudeAnimation.girl_fall);


        transform.DOPath(new Vector3[] { hatchPos, hatchPos + Vector3.down * 5f }, 1.5f).SetEase(Ease.OutSine);

        controllableFollower.model.DOLocalMove(Vector3.zero, 0.35f).SetEase(Ease.OutSine);
        controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.35f).SetEase(Ease.OutSine);

        DOVirtual.DelayedCall(0.4f, () => dirtController.AddProgress(1f));


        rider.OnVehicleHatch();
    }

    private void RunVehicle()
    {
        controllableFollower.splineFollower.follow = true;
        controllableFollower.isStopped = false;

        DOTween.Kill(GetInstanceID());
        controllableFollower.speed = rider.controllableFollower.speed;
        DOVirtual.Float(0, 1, 0.15f, (p) => {
            controllableFollower.speed = Mathf.Lerp(rider.controllableFollower.speed, controllableFollower.initialSpeed, p);
        }).SetId(GetInstanceID());

        SetState(State.Running);
    }

    private void StopVehicle()
    {
        isDead = true;
        hitTrigger.enabled = false;
        dirtController.enabled = false;
        hitTrigger.gameObject.SetActive(false);

        DOTween.Kill(GetInstanceID());
        DOVirtual.Float(1, 0, 2f, (p) => {
            controllableFollower.speed = controllableFollower.initialSpeed * p;
        }).SetEase(Ease.OutCubic).OnComplete(()=> controllableFollower.splineFollower.follow = false).SetId(GetInstanceID());
        
        controllableFollower.Jump(new Vector3(0, 13.5f, 2f));

        SetState(State.Falling);
    }

    private void SetState(State s)
    {
        if (currentState == s) return;
        //Debug.Log("new state: " + s.ToString());
        currentState = s;

        switch (s)
        {
            case State.Idle:

                List<AnimController.DudeAnimation> anims = new List<AnimController.DudeAnimation>() {
                    AnimController.DudeAnimation.man_idle_1,
                    AnimController.DudeAnimation.man_idle_2,
                    AnimController.DudeAnimation.man_idle_3
                };

                animController.PlayAnimation(anims[_IDLE_ANIM_INDEX % anims.Count]);
                break;

            case State.Running:
                animController.PlayAnimation(vehicleSittingAnim);
                break;


            case State.Falling:
                animController.PlayAnimation(AnimController.DudeAnimation.man_fall);
                break;
        }
    }
}
