using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public Image filler;
    public TextMeshProUGUI labelText;

    public void SetProgress(float p)
    {
        slider.value = p;
    }

    public void SetTextAndColor(string t, Color c)
    {
        labelText.text = t;
        filler.color = labelText.color = c;

    }

    public void ShakeText()
    {
        DOTween.Kill(GetInstanceID());
        DOTween.Sequence().SetId(GetInstanceID())
            .Append(labelText.transform.DOScale(1.7f, 0.12f).SetId(GetInstanceID()))
            .Append(labelText.transform.DOScale(1f, 0.3f).SetId(GetInstanceID()))
            ;

    }
}
