using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinishCoin : MonoBehaviour
{
    public int mult = 1;

    [Space]
    public Transform rotationRoot;
    public Transform centerTransform;
    public Transform scaleRoot;

    public void OnInteract(Rider rider)
    {
        rotationRoot.DOLocalRotate(new Vector3(-90, 0, 0), 0.3f).SetEase(Ease.OutBack);

        rider.animController.PlayAnimation(AnimController.DudeAnimation.girl_finish_victory);

        rider.transform.DOMove(transform.position + Vector3.up * 0.2f - transform.forward * Vector3.Distance(rotationRoot.position, centerTransform.position), 0.5f).SetEase(Ease.OutCubic);

        DOVirtual.DelayedCall(0.5f, () => {
            rider.transform.SetParent(transform, true);
            transform.DOMove(transform.up * 4, 2f).SetRelative(true);

            scaleRoot.localScale = new Vector3(1, 1, 10);


            //rider.transform.DORotate(new Vector3(0, 180, 0), 2f).SetRelative(true);
            //scaleRoot.DORotate(new Vector3(0, 180, 0), 1f).SetRelative(true);

        });
    }
}
