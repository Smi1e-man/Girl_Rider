using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUnlockable : MonoBehaviour
{
    public Image colorImg;
    public Image blackImg;
    public Image shadowImg;
    public Image colorImgBack;

    public TextMeshProUGUI progressLabel;
    public TextMeshProUGUI titleLabel;

    public AnimationCurve progressCurve;
    public Transform root;
    public Transform imageRoot;

    private float _p;

    public void SetData(Sprite s, float p)
    {
        _p = p;
        colorImg.sprite = blackImg.sprite = shadowImg.sprite = colorImgBack.sprite = s;

        colorImg.fillAmount = colorImgBack.fillAmount = progressCurve.Evaluate(p);
        progressLabel.text = Mathf.FloorToInt(Mathf.Clamp01(p) * 100) + "%";

    }

    public void AnimateProgress(float deltaP, float duration, TweenCallback callback = null)
    {
        DOVirtual.Float(_p, _p + deltaP, duration, x => {
            colorImg.fillAmount = colorImgBack.fillAmount = progressCurve.Evaluate(x);
            progressLabel.text = Mathf.FloorToInt(Mathf.Clamp01(x) * 100) + "%";
        }).SetEase(Ease.InOutSine).OnComplete(callback);

       
    }
}
