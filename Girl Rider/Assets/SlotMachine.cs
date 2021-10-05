using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public Transform slotModel;
    public Transform cylinder;
    public Transform stick;
    public Transform riderTargetPoint;
    [Space]
    public Toothpaste dirtLiquid;
    public Toothpaste waterLiquid;

    [Space]
    public Toothpaste fakeLiquid;
    public FloatingText floatingTextPrefab;

    [Space]
    public string winText = "Lucky You!";
    public Color winColor;
    public string failText = "Try Again";
    public Color failColor;


  
    public void OnInteract(Rider rider)
    {
        bool isSuccess = Random.value > 0.5f;

        fakeLiquid = isSuccess ? waterLiquid : dirtLiquid;


        stick.DOLocalRotate(new Vector3(90, 0, 0), 0.1f, RotateMode.LocalAxisAdd);

        float deltaRot = isSuccess ? 32f + 1080f : 212f + 1080f;
        cylinder.DOLocalRotate(new Vector3(deltaRot, 0, 0), 0.1f, RotateMode.FastBeyond360).SetDelay(0.1f);


        fakeLiquid.transform.localRotation = Quaternion.identity;
        DOVirtual.DelayedCall(0.1f, () => {
            fakeLiquid.gameObject.SetActive(true);
            fakeLiquid.transform.DOLocalRotate(new Vector3(0, 0, 90), 0.2f).SetEase(Ease.OutBack);
            DOVirtual.DelayedCall(0.1f, fakeLiquid.Play);

            DOVirtual.DelayedCall(0.25f, () => rider.dirtController.AddProgress(isSuccess ? -1f : 1f));

            rider.animController.PlayAnimation(isSuccess ? AnimController.DudeAnimation.girl_finish_victory : AnimController.DudeAnimation.girl_finish_fail);

            FloatingText ft = Instantiate(floatingTextPrefab, transform);
            ft.transform.position = transform.position + Vector3.up * 2.6f;
            ft.label.color = isSuccess ? winColor : failColor;
            ft.label.text = isSuccess ? winText : failText;
            ft.transform.localScale *= 2f;
            ft.duration = 0.5f;

            DOVirtual.DelayedCall(0.7f, () => {
                slotModel.gameObject.SetActive(false);
                fakeLiquid.bucketModel.gameObject.SetActive(false);
                fakeLiquid.content.gameObject.SetActive(false);
                GetComponent<Collider>().enabled = false;

                rider.controllableFollower.AutoSetOffset(true, true);
                rider.controllableFollower.isStopped = false;
                rider.isDead = false;
                rider.controllableFollower.splineFollower.follow = true;

                rider.SetState(Rider.State.Running);

                if (Input.GetMouseButton(0))
                    rider._isInputStarted = true;
            });
        });
        
    }
}
