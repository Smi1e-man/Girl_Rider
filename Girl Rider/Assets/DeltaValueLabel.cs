using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DeltaValueLabel : MonoBehaviour
{
    public Color waterColor;
    public Color dirtColor;

    [Space]
    public TextMeshProUGUI textLabel;
    public Transform scaleRoot;

    [Space]
    public float visibleTime = 1.5f;

    private float _accumulatedDelta;

    private Vector3 _initialScale;

    private void Awake()
    {
        _initialScale = scaleRoot.localScale;
        scaleRoot.localScale = 0.001f * _initialScale;
    }

    public void OnNewDeltaValue(float delta)
    {
        if (_accumulatedDelta * delta < 0) _accumulatedDelta = 0;
        delta *= 2;

        _accumulatedDelta += delta;
        bool isWater = _accumulatedDelta < 0;

        textLabel.color = isWater ? waterColor : dirtColor;
        textLabel.text = (isWater ? "+" : "-") + Mathf.Round(Mathf.Abs(_accumulatedDelta) * 100);

        DOTween.Kill(GetInstanceID());
        Show();
        DOVirtual.DelayedCall(visibleTime, Hide).SetId(GetInstanceID());
    }

    public void Show()
    {
        DOTween.Kill(GetInstanceID());
        scaleRoot.DOScale(_initialScale, 0.3f).SetId(GetInstanceID()).SetEase(Ease.OutCubic);
    }

    public void Hide()
    {
        DOTween.Kill(GetInstanceID());
        scaleRoot.DOScale(0.001f * _initialScale, 0.1f).SetId(GetInstanceID()).SetEase(Ease.InCubic);

        _accumulatedDelta = 0;
    }
}
