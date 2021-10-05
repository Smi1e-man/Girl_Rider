using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class CoinIconSpawner : MonoBehaviour
{
    public static CoinIconSpawner Instance;

    public GameObject prefab;
    private Camera _camera;

    public Transform target;

    private void Awake()
    {
        Instance = this;
        _camera = Camera.main;


    }

    public void Spawn(Vector3 pos, float randomRadius, int count = 1)
    {
        Vector3 spawnPos = pos;

        for (int i = 0; i < count; i++)
        {
            GameObject icon = Instantiate(prefab, transform);

            Vector3 offset = Random.insideUnitCircle * randomRadius;

            icon.transform.position = spawnPos + offset;

            Vector3 scale = icon.transform.localScale;
            icon.transform.localScale = Vector3.zero;

            float delay = Random.Range(0, 0.3f);
            float duration = 0.65f;
            DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() => MMVibrationManager.Haptic(HapticTypes.SoftImpact))
                .AppendInterval(0.2f * duration)
                .Append(icon.transform.DOMove(target.position, 0.8f * duration).SetEase(Ease.InOutCubic))
                .OnComplete(() => {
                    MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    icon.gameObject.SetActive(false);
                })
                ;

            DOTween.Sequence()
              .AppendInterval(delay)
              .Append(icon.transform.DOScale(scale, 0.2f * duration).SetEase(Ease.OutBack))
              .AppendInterval(0.6f * duration)
              .Append(icon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InSine))
              ;
        }


    }
}
