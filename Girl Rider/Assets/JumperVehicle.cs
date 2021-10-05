using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class JumperVehicle : MonoBehaviour
{
    public enum State { None, Idle, Running, Falling }
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
    public DirtController verticalProgressController;

    [Space]
    public JumperRider rider;

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

        rider.ExitVehicle();
        //rider.Jump();
    }

    public void OnRiderEnter(JumperRider r)
    {
        rider = r;

        RunVehicle();
    }

    public void OnRiderExit()
    {
        rider = null;

        StopVehicle();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (rider != null)
            {
                rider.OnVehicleHitsWall();
            }
            else
            {
                StopVehicle();
            }
        }
        else if (other.CompareTag("JumpObstacle"))
        {
            isDead = true;

            rider.Jump();
        }
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
        hitTrigger.enabled = false;
        verticalProgressController.enabled = false;
        //controllableFollower.splineFollower.follow = false;
        controllableFollower.isStopped = true;

        DOTween.Kill(GetInstanceID());
        DOVirtual.Float(1, 0, 0.3f, (p) => {
            controllableFollower.speed = controllableFollower.initialSpeed * p;
        }).OnComplete(() => controllableFollower.splineFollower.follow = false).SetId(GetInstanceID());

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
