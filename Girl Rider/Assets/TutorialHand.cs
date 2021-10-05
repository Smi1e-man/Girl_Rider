using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    public float dx = 100;
    public float duration = 0.5f;

    void Start()
    {
        transform.DOLocalMoveX(dx, duration).SetRelative(true).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

}
