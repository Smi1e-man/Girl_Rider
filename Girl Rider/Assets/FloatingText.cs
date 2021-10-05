using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float duration = 1;
    public TextMeshProUGUI label;

    private void Start()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.one * 0.001f;

        transform.DOScale(scale, 0.3f);

        DOVirtual.DelayedCall(duration, () => transform.DOScale(scale, 0.3f).OnComplete(() => Destroy(gameObject)));
    }
}
