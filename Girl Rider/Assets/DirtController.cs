using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    public delegate void DirtControllerEvent();
    public event DirtControllerEvent OnDeath;
    public delegate void DirtStateEvent(DirtState newState, DirtState oldState);
    public event DirtStateEvent OnStateChanged;

    public enum DirtState { PureBeauty, Messy, Dirty, Swampy, DirtQueen}
    public DirtState dirtState = DirtState.DirtQueen;

    [Range(0, 1)] [SerializeField] private float _progress;
    public float animatedProgress = 0;
    public float Progress => _progress;

    [Space]
    public float dirtUpSpeed = 1;
    public float dirtDownSpeed = 1;

    [Space]
    public bool isDead;

    [System.Serializable]
    public class DirtStateData
    {
        public string title;
        public float threshold;
        public Color progressBarColor;

        [Space]
        public AnimController.DudeAnimation movementAnim;
        public float movementSpeed;

        [Space]
        public Mesh mesh;
        public List<Material> materials;
    }

    [Space]
    public List<DirtStateData> dirtStateDataList;
    public DirtStateData CurrentDirtStateData;

    [Space]
    public ParticleSystem dirtFX;
    public ParticleSystem waterFX;

    [Space]
    public ParticleSystem dirtPoolParticles;
    public ParticleSystem waterPoolParticles;

    [Space]
    public ParticleIntensity cleanVFXIntensity;
    public ParticleIntensity dirtyVFXIntensity;

    private float _lastHapticTime;
    private float _hapticDelay = 0.15f;

    private float _lastVFXTime;
    private float _VFXDelay = 0.5f;

    [Space]
    public SkinnedMeshRenderer rend;
    public ProgressBar progressBar;
    public DeltaValueLabel deltaValueLabel;
    private List<Material> _materials;

    private int _dirtTriggerCount = 0;
    private int _waterTriggerCount = 0;

    private float _lastDirtStateUpdateTime;

    private void Awake()
    {
        _materials = new List<Material>();

        for (int i = 0; i < rend.sharedMaterials.Length; i++)
        {
            _materials.Add(new Material(rend.sharedMaterials[i]));
        }

        rend.sharedMaterials = _materials.ToArray();

        for (int i = 0; i < dirtStateDataList.Count; i++)
        {
            for (int j = 0; j < dirtStateDataList[i].materials.Count; j++)
            {
                dirtStateDataList[i].materials[j] = new Material(dirtStateDataList[i].materials[j]);
            }
        }


        UpdateDirtState(true);


        // for initial state
        animatedProgress = _progress;
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetFloat("_Progress", animatedProgress);
        }
        UpdateProgressBarProgress();
        
    }

    private void Start()
    {
        if (deltaValueLabel)
            GirlStateUI.Instance.UpdateProgress(_progress);
    }

    void Update()
    {
        animatedProgress = Mathf.Clamp01(Mathf.Lerp(animatedProgress, _progress, 5 * Time.deltaTime));

        UpdateProgressBarProgress();

        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetFloat("_Progress", animatedProgress);
        }

        /*
        foreach (var item in particleIntensities)
        {
            item.UpdateEmissionRate(animatedProgress);
        }*/

        if (Time.time - _lastDirtStateUpdateTime > 0.1f)
        {
            _lastDirtStateUpdateTime = Time.time;
            UpdateDirtState();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            /*
            if (animatedProgress >= 1f - dirtUpSpeed * Time.deltaTime && !isDead)
            {
                isDead = true;
                OnDeath?.Invoke();
            }*/

            _progress = Mathf.Clamp01(_progress + dirtUpSpeed * Time.deltaTime);
            if (deltaValueLabel)
            {
                deltaValueLabel.OnNewDeltaValue(dirtUpSpeed * Time.deltaTime);
                GirlStateUI.Instance.UpdateProgress(_progress);
            }

            SetDirtParticlesSpawnerPosition(other.bounds.max.y);

            if (Time.time - _lastHapticTime > _hapticDelay)
            {
                _lastHapticTime = Time.time;
                MMVibrationManager.Haptic(HapticTypes.LightImpact, true, true);
                //Debug.Log(name + " haptic");
            }

            if (Time.time - _lastVFXTime > _VFXDelay)
            {
                _lastVFXTime = Time.time;
                PlayVFX(true);

            }
        }
        else if (other.CompareTag("Water"))
        {
            _progress = Mathf.Clamp01(_progress - dirtDownSpeed * Time.deltaTime);
            if (deltaValueLabel)
            {
                deltaValueLabel.OnNewDeltaValue(-dirtDownSpeed * Time.deltaTime);
                GirlStateUI.Instance.UpdateProgress(_progress);
            }

            SetWaterParticlesSpawnerPosition(other.bounds.max.y);

            if (Time.time - _lastHapticTime > _hapticDelay)
            {
                _lastHapticTime = Time.time;
                MMVibrationManager.Haptic(HapticTypes.LightImpact, true, true);
                //Debug.Log(name + " haptic");
            }

            if (Time.time - _lastVFXTime > _VFXDelay)
            {
                _lastVFXTime = Time.time;
                PlayVFX(false);

            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            if (_dirtTriggerCount == 0)
            {
                dirtPoolParticles.Play();

                if (animatedProgress >= 1f - dirtUpSpeed * Time.deltaTime && !isDead)
                {
                    isDead = true;
                    OnDeath?.Invoke();
                }

            }
            _dirtTriggerCount++;

        }
        else if (other.CompareTag("Water"))
        {
            if (_waterTriggerCount == 0)
                waterPoolParticles.Play();
            _waterTriggerCount++;
        }
        else  if (other.CompareTag("Collectible"))
        {
            Collectible collectible = other.GetComponent<Collectible>();

            if (!collectible.isPermanent)
                collectible.OnCollect(transform.position + Vector3.up * 2f);

            AddProgress(collectible.value);

        }
        else if (other.CompareTag("Washer"))
        {
            AddProgress(-0.2f);
        }
        else if (other.CompareTag("FakeLiquid"))
        {
            AddProgress(0.2f);
        }
        else if (other.CompareTag("DirtTextureSwitcher"))
        {
            DirtTextureSwitcher dirtTextureSwitcher = other.GetComponent<DirtTextureSwitcher>();

            for (int i = 0; i < dirtStateDataList.Count; i++)
            {
                for (int j = 0; j < dirtStateDataList[i].materials.Count; j++)
                {
                    if (dirtTextureSwitcher.texture != null)
                        dirtStateDataList[i].materials[j].SetTexture("_BaseMap", dirtTextureSwitcher.texture);

                    if (dirtTextureSwitcher.normalMap != null)
                        dirtStateDataList[i].materials[j].SetTexture("Texture2D_1", dirtTextureSwitcher.normalMap);

                    dirtStateDataList[i].materials[j].SetColor("_Color", dirtTextureSwitcher.color);

                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            _dirtTriggerCount--;
            if (_dirtTriggerCount == 0)
                dirtPoolParticles.Stop();

            
        }
        else if (other.CompareTag("Water"))
        {
            _waterTriggerCount--;
            if (_waterTriggerCount == 0)
                waterPoolParticles.Stop();

            
        }
    }

    public void AddProgress(float delta, bool isDeathCheck = true)
    {
        if (delta == 0 || isDead) return;


        if (isDeathCheck && _progress >= 1f && delta > 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
        }

        _progress = Mathf.Clamp01(_progress + delta);
        if (deltaValueLabel)
        {
            deltaValueLabel.OnNewDeltaValue(delta);
            GirlStateUI.Instance.UpdateProgress(_progress);
        }

        UpdateDirtState();


        PlayVFX(delta > 0);
        MMVibrationManager.Haptic(HapticTypes.MediumImpact, true, true);

    }

    public void PlayVFX(bool isDirt)
    {
        if (isDirt)
            dirtFX.Play();
        else
            waterFX.Play();
    }

    public void SetDirtParticlesSpawnerPosition(float worldY)
    {
        Vector3 targetPos = transform.position - transform.forward * 0.2f;
        targetPos.y = worldY - 0.2f;
        dirtPoolParticles.transform.position = targetPos;

    }

    public void SetWaterParticlesSpawnerPosition(float worldY)
    {
        Vector3 targetPos = transform.position - transform.forward * 0.2f;
        targetPos.y = worldY - 0.2f;
        waterPoolParticles.transform.position = targetPos;

    }

    public void UpdateDirtState(bool ignoreEvent = false)
    {
        DirtState oldState = dirtState;

        for (int i = 0; i < dirtStateDataList.Count; i++)
        {
            if (Progress <= dirtStateDataList[i].threshold)
            {
                dirtState = (DirtState)i;
                break;
            }
        }

        CurrentDirtStateData = dirtStateDataList[(int)dirtState];

        if (progressBar)
            progressBar.SetTextAndColor(CurrentDirtStateData.title, CurrentDirtStateData.progressBarColor);

        if (oldState != dirtState && !ignoreEvent)
            OnStateChanged?.Invoke(dirtState, oldState);

        UpdateVisuals();
    }

    public void UpdateProgressBarProgress()
    {
        if (!progressBar) return;

        float p = animatedProgress;
        //progressBar.SetProgress(Mathf.Clamp01(1 - p));

        
        float maxP = 1 - ((int)dirtState == 0 ? 0 : dirtStateDataList[(int)dirtState - 1].threshold);
        float minP = 1 - dirtStateDataList[(int)dirtState].threshold;
        p = (1 - animatedProgress - minP) / (maxP - minP);


        progressBar.SetProgress(p);

        //Debug.Log((1f - animatedProgress) + " | " + minP + " |  " + maxP);
    }

    public void UpdateVisuals()
    {
        _materials = CurrentDirtStateData.materials;
        rend.sharedMaterials = _materials.ToArray();

        rend.sharedMesh = CurrentDirtStateData.mesh;
    }

}
