using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyLabel : MonoBehaviour
{
    public static MoneyLabel Instance;

    public GameObject root;
    public TextMeshProUGUI counterText;

    protected Vector3 _initialCounterScale;
    protected Vector3 _initialScale;

    public int money;

    private void Awake()
    {
        Instance = this;


        _initialCounterScale = counterText.transform.localScale;
        _initialScale = root.transform.localScale;

        money = PlayerPrefs.GetInt("money", 0);
        SetCounter(money);
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
        counterText.text = String.Format("{0:n0}", c);

    }

    public void AnimateCounterAdd(int c, float delay = 0)
    {
        int targetCount = money + c;
        DOTween.Kill(GetInstanceID());
        DOTween.To(() => money, x => money = x, targetCount, 0.5f).SetId(GetInstanceID()).OnUpdate(()=> SetCounter(money)).SetDelay(delay);
    }

    public void AddAndSave(int count)
    {
        int newValue = PlayerPrefs.GetInt("money", 0) + count;
        if (newValue < 0)
            newValue = 0;
        PlayerPrefs.SetInt("money", newValue);
    }
}
