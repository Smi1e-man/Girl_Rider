using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GirlStateUI : MonoBehaviour
{
    public static GirlStateUI Instance;

    public GameObject root;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI multText;

    protected Vector3 _initialCounterScale;
    protected Vector3 _initialScale;


    [HideInInspector] public int value;
    [HideInInspector] public int mult;

    private void Awake()
    {
        Instance = this;

        _initialCounterScale = counterText.transform.localScale;
        _initialScale = root.transform.localScale;

        multText.text = "";
    }

    public static void Show(float delay = 0)
    {
        Instance.root.SetActive(true);
        Instance.root.transform.DOScale(Instance._initialScale, 0.3f).SetEase(Ease.OutSine).SetDelay(delay);
    }

    public static void Hide(float delay = 0)
    {
        Instance.root.transform.DOScale(0, 0.2f).SetEase(Ease.InBack).SetDelay(delay);
    }

    public void SetCounter(int c)
    {
        value = c;
        counterText.text = c.ToString();

    }

    public void AnimateCounterAdd(int c)
    {
        int targetCount = value + c;
        DOTween.Kill(GetInstanceID());
        DOTween.To(() => value, x => value = x, targetCount, 0.5f).SetId(GetInstanceID()).OnUpdate(() => SetCounter(value));
    }

    public void UpdateProgress(float p)
    {
        SetCounter(Mathf.RoundToInt((1 - p) * 100 * 2));
    }
}
