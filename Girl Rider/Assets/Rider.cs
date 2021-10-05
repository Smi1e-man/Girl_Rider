using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class Rider : MonoBehaviour
{
    public enum State { None, Running, Flying, Riding, RotationTube, SlidingTube, SplineFlying}

    public Vehicle currentVehicle;
    public State currentState;

    [Space]
    public ControllableSplineFollower controllableFollower;
    public AnimController animController;
    public DirtController dirtController;
    public PositionUpdater progressBarPositionUpdater;
    public Transform headTransform;
    public EmojiPanel emojiPanel;

    [Space]
    public AnimController.DudeAnimation onEnemyHitAnim = AnimController.DudeAnimation.girl_idle;

    [Space]
    public bool isInputAllowed = true;
    public bool jumpOnRelease = true;
    public bool isDirtDeathEnabled = true;

    [Space]
    public Vector3 flyJumpForce = new Vector3(0, 20f, 8.5f);
    public Vector3 fallJumpForce = new Vector3(0, 13.5f, 2f);

    [Space]
    public float horizontalSensivity = 25f;
    public float inputLerpSpeed = 10f;

    [Space]
    public bool isDead;

    public bool _isInputStarted;

    private Vector3 _previousMousePosition;
    private AnimController.DudeAnimation _rideAnim = AnimController.DudeAnimation.girl_sit;

    private AnimController.DudeAnimation _walkAnim = AnimController.DudeAnimation.girl_walk;

    [Space]
    public float inputDeltaX;
    public float targetInputDeltaX;

    private float _startTime;
    private bool _isGameStarted;
    private bool _gameCanBeStarted;

    private float _rotationTubeAngle;


    private void OnEnable()
    {
        if (isDirtDeathEnabled)
            dirtController.OnDeath += Death;

        dirtController.OnStateChanged += OnDirtStateChanged;
    }

    private void OnDisable()
    {
        if (isDirtDeathEnabled)
            dirtController.OnDeath -= Death;

        dirtController.OnStateChanged -= OnDirtStateChanged;
    }

    private void Start()
    {
        _isGameStarted = false;
        _gameCanBeStarted = true; /// !! for start animation
        isDead = true;
        controllableFollower.enabled = false;
        controllableFollower.splineFollower.follow = false;

        animController.PlayAnimation(AnimController.DudeAnimation.girl_idle);

        /*
        animController.PlayAnimation(AnimController.DudeAnimation.girl_walk);

        float fallDelay = 0.3f;
        transform.DOMove(transform.forward * 3, fallDelay + 0.5f).SetEase(Ease.OutSine).SetRelative(true);
        DOVirtual.DelayedCall(fallDelay, () => animController.PlayAnimation(AnimController.DudeAnimation.man_fall));

        DOVirtual.DelayedCall(fallDelay + 0.4f, () => dirtController.AddProgress(0.67f));

        DOVirtual.DelayedCall(1.5f, () => _gameCanBeStarted = true);

        StartAsset.Instance.transform.SetParent(transform.parent, true);

        CameraSplineFollower.Instance.enabled = false;
        CameraSplineFollower.Instance.transform.SetPositionAndRotation(StartAsset.Instance.startCameraPoint.position, StartAsset.Instance.startCameraPoint.rotation);


        DOTween.Kill(CameraSplineFollower.Instance.GetInstanceID());
        DOVirtual.DelayedCall(1, () => {
            CameraSplineFollower.Instance.transform.DOMove(StartAsset.Instance.finishCameraPoint.position, 1f).SetEase(Ease.InOutCubic).SetId(CameraSplineFollower.Instance.GetInstanceID());
            CameraSplineFollower.Instance.transform.DORotateQuaternion(StartAsset.Instance.finishCameraPoint.rotation, 1f).SetEase(Ease.InOutCubic).SetId(CameraSplineFollower.Instance.GetInstanceID());
        });

        DOVirtual.DelayedCall(1.5f, GameController.Instance.gamePanel.ShowTutorial);
        */

        CameraSplineFollower.Instance.enabled = false;
        GameController.Instance.gamePanel.ShowTutorial();


        dirtController.cleanVFXIntensity.SetEmissionRate(0);
        dirtController.dirtyVFXIntensity.SetEmissionRate(0);

    }

    public void OnGameStarted()
    {
        _isGameStarted = true;
        _startTime = Time.time;

        isDead = false;
        controllableFollower.enabled = true;
        controllableFollower.splineFollower.follow = true;
        controllableFollower.speed = 0;

        DOVirtual.Float(0, controllableFollower.initialSpeed, 1f, x => controllableFollower.speed = x);

        SetState(State.Running);

        _isInputStarted = true;

        CameraSplineFollower.Instance.enabled = true;
        DOTween.Kill(CameraSplineFollower.Instance.GetInstanceID());

        GameController.Instance.gamePanel.HideTutorial();
    }

    private void Update()
    {
        if (!_isGameStarted && _gameCanBeStarted &&  Input.GetMouseButton(0))
            OnGameStarted();

        if (isDead) return;


        switch (currentState)
        {
            case State.SlidingTube:
                HandleSlidingTubeUpdate();
                break;

            case State.RotationTube:
                HandleRotationTubeUpdate();
                break;

            default:
                HandleMovementUpdate();
                break;
        }





        /*
        if (Input.GetKeyDown(KeyCode.Space))
            Fall();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isDead = true;
            controllableFollower.splineFollower.follow = false;

            animController.PlayAnimation(AnimController.DudeAnimation.girl_finish_fail);
            GameController.Instance.OnLevelCompleted(false, 0.5f);
        }
        */

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isDead = true;
            controllableFollower.splineFollower.follow = false;

            animController.PlayAnimation(AnimController.DudeAnimation.girl_finish_victory);
            progressBarPositionUpdater.gameObject.SetActive(false);

            GameController.Instance.OnLevelCompleted(true, 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.X))
            PlayerPrefs.DeleteAll();
    }

    private void HandleMovementUpdate()
    {

        if (Input.GetMouseButtonDown(0) && isInputAllowed)
        {
            _isInputStarted = true;
            _previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isInputStarted = false;

            if (jumpOnRelease && currentVehicle != null)
                Jump();

        }

        if (_isInputStarted)
            targetInputDeltaX = Mathf.Clamp((Input.mousePosition - _previousMousePosition).x / Screen.width * horizontalSensivity, -1, 1);
        else
            targetInputDeltaX = 0;


        inputDeltaX = Mathf.Lerp(inputDeltaX, targetInputDeltaX, inputLerpSpeed * Time.deltaTime);

        _previousMousePosition = Input.mousePosition;

        if (currentVehicle != null)
            currentVehicle.controllableFollower.inputDeltaX = inputDeltaX;
        else
            controllableFollower.inputDeltaX = inputDeltaX;


        
        if (currentVehicle == null && Time.time - _startTime > 1 && controllableFollower.isOnGround && !controllableFollower.previusIsOnGround)
            SetState(State.Running);
        

    }

    private void HandleSlidingTubeUpdate()
    {

        if (Input.GetMouseButtonDown(0) && isInputAllowed)
        {
            _isInputStarted = true;
            _previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isInputStarted = false;

            if (jumpOnRelease && currentVehicle != null)
                Jump();

        }

        if (_isInputStarted)
            targetInputDeltaX = Mathf.Clamp((Input.mousePosition - _previousMousePosition).x / Screen.width * horizontalSensivity, -1, 1);
        else
            targetInputDeltaX = 0;


        inputDeltaX = Mathf.Lerp(inputDeltaX, targetInputDeltaX, inputLerpSpeed * Time.deltaTime);
        controllableFollower.inputDeltaX = inputDeltaX;

        _previousMousePosition = Input.mousePosition;

        controllableFollower.model.rotation = Quaternion.Lerp(controllableFollower.model.rotation, Quaternion.LookRotation(controllableFollower.splineFollower.result.forward), 10 * Time.deltaTime);

    }

    private void HandleRotationTubeUpdate()
    {

        if (Input.GetMouseButtonDown(0) && isInputAllowed)
        {
            _isInputStarted = true;
            _previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isInputStarted = false;

            if (jumpOnRelease && currentVehicle != null)
                Jump();

        }

        
        if (_isInputStarted)
            targetInputDeltaX = Mathf.Clamp((Input.mousePosition - _previousMousePosition).x / Screen.width * horizontalSensivity, -1, 1);
        else
            targetInputDeltaX = 0;


        inputDeltaX = Mathf.Lerp(inputDeltaX, targetInputDeltaX, inputLerpSpeed * Time.deltaTime);
        //controllableFollower.inputDeltaX = inputDeltaX;

        _rotationTubeAngle -= 150 * inputDeltaX * Time.deltaTime * 60;
        _rotationTubeAngle = Mathf.Clamp(_rotationTubeAngle, -150, 150);

        _previousMousePosition = Input.mousePosition;

        controllableFollower.model.rotation = Quaternion.Lerp(controllableFollower.model.rotation, Quaternion.LookRotation(controllableFollower.splineFollower.result.forward) * Quaternion.Euler(0, 0, _rotationTubeAngle), 10 * Time.deltaTime);
        //controllableFollower.hitTrigger.transform.rotation = controllableFollower.model.transform.rotation;

    }

    public void Jump()
    {
        if (currentVehicle != null)
        {
            ExitVehicleWithJump();

        }
        else if (controllableFollower.isOnGround)
        {
            controllableFollower.Jump(flyJumpForce);
        }

        SetState(State.Flying);
    }

    public void Death()
    {
        Debug.Log("DEATH!");

        DOTween.Kill(GetInstanceID());

        isDead = true;

        controllableFollower.splineFollower.follow = false;
        controllableFollower.inputDeltaX = inputDeltaX = targetInputDeltaX = 0;
        controllableFollower.enabled = false;

        animController.PlayAnimation(AnimController.DudeAnimation.man_fall);
        animController.isDead = true;

        //progressBarPositionUpdater.gameObject.SetActive(false);

        GameController.Instance.OnLevelCompleted(false, 0.5f);
    }

    public void Fall()
    {
        isDead = true;
        controllableFollower.inputDeltaX = inputDeltaX = targetInputDeltaX = 0;

        animController.PlayAnimation(AnimController.DudeAnimation.man_fall);
        controllableFollower.Jump(fallJumpForce);

        //controllableFollower.hitTrigger.enabled = false;

        //DOVirtual.DelayedCall(0.5f, () => controllableFollower.Jump(new Vector3(0, 0, -5)));

        DOVirtual.DelayedCall(0.5f, () => controllableFollower.speed = 1);

        DOVirtual.DelayedCall(1.2f, () => {
            isDead = false;


            //controllableFollower.hitTrigger.enabled = true;

            currentState = State.None;
            SetState(State.Running);

            animController.animator.speed = 0.6f;

            controllableFollower.speed = controllableFollower.initialSpeed;

        });

        DOVirtual.DelayedCall(2, () => {
            animController.animator.speed = 1f;
        });
    }

    public void FailFall()
    {
        isDead = true;

        animController.PlayAnimation(AnimController.DudeAnimation.man_fall);
        controllableFollower.Jump(new Vector3(0, 10f, 1.5f));

        controllableFollower.hitTrigger.enabled = false;

        DOTween.Kill(GetInstanceID());
        DOVirtual.Float(1, 0.33f, 4f, (p) => {
            controllableFollower.speed = controllableFollower.initialSpeed * p;
        }).SetEase(Ease.OutCubic);


        controllableFollower.groundOffset = -30;

        dirtController.progressBar.gameObject.SetActive(false);
        dirtController.enabled = false;

        DOVirtual.DelayedCall(0.2f, () => CameraSplineFollower.Instance.enabled = false);
        GameController.Instance.OnLevelCompleted(false, 0.8f);
    }

    public void EnterHatch(Vector3 hatchPos)
    {
        isDead = true;
        currentState = State.None;

        //controllableFollower.enabled = false;
        controllableFollower.splineFollower.follow = false;
        controllableFollower.hitTrigger.enabled = false;

        animController.PlayAnimation(AnimController.DudeAnimation.girl_idle);


        //transform.DOPath(new Vector3[] {hatchPos, hatchPos + Vector3.down * 3.1f}, 0.5f).SetEase(Ease.InOutSine);
        transform.DOMove(hatchPos + Vector3.down * 2.0f, 0.6f).SetEase(Ease.OutBack);

        controllableFollower.model.DOLocalMove(Vector3.zero, 0.35f).SetEase(Ease.OutSine);
        controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.35f).SetEase(Ease.OutSine);

        DOVirtual.DelayedCall(0.2f, () => dirtController.AddProgress(0.5f));

        DOVirtual.DelayedCall(0.7f, ExitHatch).SetId(GetInstanceID());
    }

    public void ExitHatch()
    {
        animController.PlayAnimation(AnimController.DudeAnimation.girl_fall);

        transform.DOMove(transform.position + Vector3.up * 2.0f, 0.35f).SetEase(Ease.OutSine).OnComplete(() => {
            isDead = false;

            controllableFollower.enabled = true;
            controllableFollower.AutoSetOffset(true, true);
            controllableFollower.splineFollower.follow = true;
            controllableFollower.hitTrigger.enabled = false;
            DOVirtual.DelayedCall(0.3f, () => controllableFollower.hitTrigger.enabled = true).SetId(GetInstanceID());

            SetState(State.Running);
        }).SetId(GetInstanceID());

    }

    public void OnVehicleHatch()
    {
        DOTween.Kill(GetInstanceID());

        controllableFollower.model.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
        controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.5f).SetEase(Ease.InOutSine);

        SetState(State.Running);


        DOVirtual.DelayedCall(0.2f, () =>
        {
            currentVehicle = null;
            transform.SetParent(transform.parent.parent, true);

            controllableFollower.enabled = true;
            controllableFollower.AutoSetOffset(true, true);
            controllableFollower.splineFollower.follow = true;
            controllableFollower.hitTrigger.enabled = false;
            DOVirtual.DelayedCall(0.3f, () => controllableFollower.hitTrigger.enabled = true).SetId(GetInstanceID());


            progressBarPositionUpdater.target = headTransform;
        });

    }

    public void EnterVehicle(Vehicle vehicle)
    {
        currentVehicle = vehicle;
        vehicle.OnRiderEnter(this);

        controllableFollower.enabled = false;
        controllableFollower.splineFollower.follow = false;
        controllableFollower.hitTrigger.enabled = false;
        transform.SetParent(vehicle.transform, true);

        DOTween.Kill(GetInstanceID());
        Transform sittingPoint = vehicle.sittingPoints[vehicle.sittingPointIndex];

       
        transform.DOLocalMove(Vector3.zero, 0.35f).SetEase(Ease.OutSine).SetId(GetInstanceID());
        transform.DOLocalRotateQuaternion(Quaternion.identity, 0.35f);

        controllableFollower.model.DOLocalMove(sittingPoint.localPosition, 0.35f).SetEase(Ease.OutSine);
        controllableFollower.model.DOLocalRotateQuaternion(sittingPoint.localRotation, 0.35f).SetEase(Ease.OutSine);

        _rideAnim = vehicle.riderSittingAnim;
        SetState(State.Riding);


        progressBarPositionUpdater.target = vehicle.headTransform;
    }

    public void ExitVehicle()
    {
        DOTween.Kill(GetInstanceID());


        currentVehicle.OnRiderExit();

        transform.DOLocalMove(Vector3.forward * 1.5f, 0.35f).SetEase(Ease.InOutSine); ;


        DOVirtual.DelayedCall(0.15f, () => {
            controllableFollower.model.DOLocalMove(Vector3.zero, 0.35f).SetEase(Ease.InOutSine);
            controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.35f).SetEase(Ease.InOutSine);
            SetState(State.Flying);
        });


        DOVirtual.DelayedCall(0.5f, () => {
            controllableFollower.SetLevelPath(currentVehicle.controllableFollower.levelPath);
            currentVehicle = null;
            transform.SetParent(transform.parent.parent, true);

            controllableFollower.enabled = true;
            //controllableFollower.AutoSetOffset(true, true);
            //controllableFollower.splineFollower.follow = true;
            controllableFollower.hitTrigger.enabled = false;
            DOVirtual.DelayedCall(0.3f, () => controllableFollower.hitTrigger.enabled = true).SetId(GetInstanceID());

            progressBarPositionUpdater.target = headTransform;
        });
        
    }

    public void ExitVehicleWithJump()
    {
        DOTween.Kill(GetInstanceID());


        currentVehicle.OnRiderExitWithJump();

        transform.DOLocalMove(Vector3.forward * 0.5f + Vector3.up * 1f, 0.15f).SetEase(Ease.OutSine); ;
        controllableFollower.model.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.OutSine);
        controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.15f).SetEase(Ease.OutSine);

        DOVirtual.DelayedCall(0.15f, () => {
            currentState = State.None;
            SetState(State.Flying);

            controllableFollower.SetLevelPath(currentVehicle.controllableFollower.levelPath);
            currentVehicle = null;
            transform.SetParent(transform.parent.parent, true);

            controllableFollower.enabled = true;
            //controllableFollower.AutoSetOffset(true, true);
            //controllableFollower.splineFollower.follow = true;

            progressBarPositionUpdater.target = headTransform;

            controllableFollower.Jump(flyJumpForce);

        });


        DOVirtual.DelayedCall(0.5f, () => {
            
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isDead) return;
        if (other.CompareTag("Untagged")) return;

        if (other.CompareTag("Vehicle") && currentVehicle == null)
        {
            Vehicle vehicle = other.GetComponentInParent<Vehicle>();

            if (!vehicle.isDead)
                EnterVehicle(vehicle);
        }
        else if (other.CompareTag("KnockObstacle"))
        {
            Fall();
        }
        else if (other.CompareTag("StopObstacle"))
        {
            controllableFollower.isStopped = true;
        }
        else if (other.CompareTag("LongitudinalWall"))
        {
            controllableFollower.HandleActualOffsetLimitation(other.transform, true);
        }
        else if (other.CompareTag("JumpObstacle"))
        {
            FailFall();
        }
        else if (other.CompareTag("Hatch"))
        {
            EnterHatch(other.transform.position);
            DOVirtual.DelayedCall(0.1f, other.GetComponentInChildren<ParticleSystem>().Play);

        }
        else if (other.CompareTag("SlotMachine"))
        {
            controllableFollower.isStopped = true;
            isDead = true;
            _isInputStarted = false;
            controllableFollower.inputDeltaX = inputDeltaX = targetInputDeltaX = 0;
            controllableFollower.splineFollower.follow = false;

            currentState = State.None;
            animController.PlayAnimation(AnimController.DudeAnimation.girl_idle);


            SlotMachine slotMachine = other.GetComponent<SlotMachine>();

            transform.DOMove(slotMachine.riderTargetPoint.position, 0.2f);
            slotMachine.OnInteract(this);
        }
        else if (other.CompareTag("Enemy"))
        {
            MovingEnemy movingEnemy = other.GetComponent<MovingEnemy>();

            movingEnemy.OnHit(transform);


            dirtController.AddProgress(movingEnemy.dirtDeltaValue);
            other.enabled = false;


            controllableFollower.isStopped = true;
            isDead = true;
            _isInputStarted = false;
            controllableFollower.inputDeltaX = inputDeltaX = targetInputDeltaX = 0;

            currentState = State.None;
            animController.PlayAnimation(onEnemyHitAnim);

            DOVirtual.DelayedCall(0.7f, () =>
            {
                controllableFollower.isStopped = false;
                isDead = false;

                SetState(State.Running);

                if (Input.GetMouseButton(0))
                    _isInputStarted = true;

            }).SetId(GetInstanceID());
        }
        else if (other.CompareTag("StateSwitcher"))
        {
            StateSwitcher stateSwitcher = other.GetComponent<StateSwitcher>();

            SetState(stateSwitcher.state);
        }
        else if (other.CompareTag("FinishCoin"))
        {
            isDead = true;
            controllableFollower.enabled = false;
            controllableFollower.splineFollower.enabled = false;
            controllableFollower.hitTrigger.enabled = false;

            progressBarPositionUpdater.gameObject.SetActive(false);


            FinishCoin coin = other.GetComponent<FinishCoin>();
            GirlStateUI.Instance.mult = coin.mult;
            coin.OnInteract(this);


            GameController.Instance.OnFinishInteractTime = Time.time;
            GameController.Instance.OnFinishItemIndex = coin.mult;

            GameController.Instance.OnLevelCompleted(true, 1f);
            GameController.Instance.resultPanel.resultText.text = "You\n" + progressBarPositionUpdater.GetComponent<ProgressBar>().labelText.text + "!";
        }
        else if (other.CompareTag("FinishStopPoint"))
        {
            FinishStopPoint stopPoint = other.GetComponent<FinishStopPoint>();

            if (stopPoint.index >= 4 - (int)dirtController.dirtState)
            {
                isDead = true;
                if (currentVehicle) ExitVehicle();

                controllableFollower.enabled = false;
                controllableFollower.splineFollower.enabled = false;
                controllableFollower.hitTrigger.enabled = false;

                stopPoint.finishAsset.OnRiderStop(this, stopPoint);
            }
           
        }
        else if (other.CompareTag("CentrifyTrigger"))
        {
            //Debug.Log("CentrifyTrigger");
            isDead = true;
            inputDeltaX = targetInputDeltaX = controllableFollower.inputDeltaX = 0;

            Vector2 initialOffset = controllableFollower.splineFollower.motion.offset;
            DOVirtual.Float(0, 1, 0.5f, p =>
            {
                Vector2 offset = controllableFollower.splineFollower.motion.offset;
                offset.x = Mathf.Lerp(initialOffset.x, 0, p);
                offset.y = Mathf.Lerp(initialOffset.y, 0, p);

                controllableFollower.splineFollower.motion.offset = offset;

            });
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

    private void OnDirtStateChanged(DirtController.DirtState newDirtState, DirtController.DirtState oldDirtState)
    {
        //Debug.Log(" --- new dirt state: " + dirtState.ToString());

        if (currentState == State.Running && !isDead)
        {
            animController.PlayAnimation(AnimController.DudeAnimation.girl_spin);

            DOTween.Kill(GetInstanceID() + " spin");
            DOVirtual.DelayedCall(0.3f, () => SetState(State.Running)).SetId(GetInstanceID() + " spin");
        }

        dirtController.progressBar.ShakeText();
        emojiPanel.OnNewState(newDirtState < oldDirtState);

        switch (newDirtState)
        {
            case DirtController.DirtState.PureBeauty:
                dirtController.cleanVFXIntensity.SetEmissionRate(1);
                dirtController.dirtyVFXIntensity.SetEmissionRate(0);
                break;

            case DirtController.DirtState.Messy:
                dirtController.cleanVFXIntensity.SetEmissionRate(0);
                dirtController.dirtyVFXIntensity.SetEmissionRate(0);
                break;

            case DirtController.DirtState.Dirty:
                dirtController.cleanVFXIntensity.SetEmissionRate(0);
                dirtController.dirtyVFXIntensity.SetEmissionRate(0);
                break;

            case DirtController.DirtState.Swampy:
                dirtController.cleanVFXIntensity.SetEmissionRate(0);
                dirtController.dirtyVFXIntensity.SetEmissionRate(0.2f);
                break;

            case DirtController.DirtState.DirtQueen:
                dirtController.cleanVFXIntensity.SetEmissionRate(0);
                dirtController.dirtyVFXIntensity.SetEmissionRate(1);
                break;
        }
    }

    public void SetState(State s)
    {
        if (isDead) return;
        //if (currentState == s) return;
        //Debug.Log("new state: " + s.ToString());
        DOTween.Kill(GetInstanceID() + " spin");

        switch (s)
        {
            case State.Running:

                AnimController.DudeAnimation anim = dirtController.CurrentDirtStateData.movementAnim;
                float speed = dirtController.CurrentDirtStateData.movementSpeed;

                if (currentState != s || _walkAnim != anim)
                {
                    _walkAnim = anim;

                    animController.PlayAnimation(_walkAnim);

                    controllableFollower.AnimateSpeed(speed, 0.35f);
                }


                break;

            case State.Flying:

                if (currentState != s)
                    animController.PlayAnimation(AnimController.DudeAnimation.girl_fall);

                break;

            case State.SplineFlying:

                animController.PlayAnimation(AnimController.DudeAnimation.girl_jump);
                controllableFollower.AnimateSpeed(14, 0.2f);

                break;

           
            case State.Riding:

                if (currentState != s)
                    animController.PlayAnimation(_rideAnim);
                break;

            case State.SlidingTube:
                animController.PlayAnimation(AnimController.DudeAnimation.man_fall);
                controllableFollower.AnimateSpeed(14, 1f);

                break;

            case State.RotationTube:
                animController.PlayAnimation(AnimController.DudeAnimation.girl_pipe);

                controllableFollower.inputDeltaX = inputDeltaX = targetInputDeltaX = 0;
                controllableFollower.AnimateSpeed(8, 1f);

                Vector2 initialOffset = controllableFollower.splineFollower.motion.offset;
                DOVirtual.Float(0, 1, 0.9f, p => {
                    Vector2 offset = controllableFollower.splineFollower.motion.offset;
                    offset.x = Mathf.Lerp(initialOffset.x, 0, p);
                    offset.y = Mathf.Lerp(initialOffset.y, 0, p);

                    controllableFollower.splineFollower.motion.offset = offset;

                });

                _rotationTubeAngle = 0;

                break;
        }

        currentState = s;
    }
}
