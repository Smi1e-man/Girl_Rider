using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GreatFinishAsset : MonoBehaviour
{
    //public static GreatFinishAsset Instance;

    [System.Serializable]
    public class GreatFinishItem
    {
        public AnimController dudeAnimController;
        public Transform girlTargetPoint;

        public AnimController.DudeAnimation girlAnimation;
        public AnimController.DudeAnimation boyAnimation;
    }

    public float girlSpeed = 5;

    [Space]

    [Space]
    public Transform cameraTargetPoint;
    public Transform greatJobCameraTargetPoint;
    public Transform stairStartPoint;

    [Space]
    public List<GreatFinishItem> finishItems;

    [HideInInspector] public int itemIndex;
    [HideInInspector] public float onInteractTime;

    void Awake()
    {
        //Instance = this;
    }

    /*
    public void OnInteract(Rider rider)
    {
        onInteractTime = Time.time;
        itemIndex = -1;
        for (int i = 0; i < finishItems.Count; i++)
        {
            if (rider.dirtController.Progress <= finishItems[i].percent)
                itemIndex++;
            else
                break;
        }

        //Debug.Log("finish index: " + (itemIndex + 1));


        Vector3 upOffset = Vector3.up * 0.5f;
        float movementDur = Vector3.Distance(stairStartPoint.position, finishItems[itemIndex].girlTargetPoint.position) / girlSpeed;
        DOTween.Sequence()
            .Append(rider.transform.DOMove(stairStartPoint.position + upOffset, 0.5f).SetEase(Ease.Linear))
            //.AppendCallback(() => rider.animController.PlayAnimation(AnimController.DudeAnimation.girl_stairs_up))
            .Append(rider.transform.DOMove(finishItems[itemIndex].girlTargetPoint.position + upOffset, movementDur).SetEase(Ease.Linear))
            .AppendCallback(() => {
                rider.animController.PlayAnimation(AnimController.DudeAnimation.girl_idle);

                rider.transform.DOMove(-upOffset, 0.1f).SetEase(Ease.OutSine).SetRelative(true);

                rider.progressBarPositionUpdater.gameObject.SetActive(false);

                if (itemIndex == finishItems.Count - 1)
                {
                    CameraSplineFollower.Instance.transform.DOMove(greatJobCameraTargetPoint.position, 1.0f).SetEase(Ease.Linear);
                    CameraSplineFollower.Instance.transform.DORotateQuaternion(greatJobCameraTargetPoint.rotation, 1.0f).SetEase(Ease.Linear);
                }
            })
            .Append(rider.transform.DORotateQuaternion(finishItems[itemIndex].girlTargetPoint.rotation, 0.4f))
            .AppendCallback(() => { 
                rider.animController.PlayAnimation(finishItems[itemIndex].girlAnimation);
                finishItems[itemIndex].dudeAnimController.PlayAnimation(finishItems[itemIndex].boyAnimation);

                float resultScreenDelay = itemIndex == finishItems.Count - 1 ? 2f : 1.5f;
                GameController.Instance.OnLevelCompleted(true, resultScreenDelay);
            })
            ;


        rider.transform.DORotateQuaternion(Quaternion.LookRotation(transform.right), 0.3f);
        rider.controllableFollower.model.DOLocalRotateQuaternion(Quaternion.identity, 0.3f);


        CameraSplineFollower.Instance.enabled = false;
        Vector3 cameraEndPos = finishItems[itemIndex].girlTargetPoint.position + cameraTargetPoint.position - stairStartPoint.position;
        CameraSplineFollower.Instance.transform.DOMove(cameraTargetPoint.position, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => CameraSplineFollower.Instance.transform.DOMove(cameraEndPos, movementDur).SetEase(Ease.Linear));
        
        CameraSplineFollower.Instance.transform.DORotateQuaternion(cameraTargetPoint.rotation, 1.5f);
        
    }*/

    public void OnRiderStop(Rider rider, FinishStopPoint stopPoint)
    {
        onInteractTime = Time.time;
        itemIndex = stopPoint.index;

        GameController.Instance.OnFinishInteractTime = onInteractTime;
        GameController.Instance.OnFinishItemIndex = itemIndex;

        DOTween.Sequence()
            .Append(rider.transform.DOMove(stopPoint.targetTransform.position, 0.5f).SetEase(Ease.Linear))
            .Join(rider.transform.DORotateQuaternion(stopPoint.targetTransform.rotation, 0.5f))
            .AppendCallback(() =>
            {
                rider.animController.PlayAnimation(AnimController.DudeAnimation.girl_idle);
                rider.progressBarPositionUpdater.gameObject.SetActive(false);

                
                   
                rider.animController.PlayAnimation(finishItems[itemIndex].girlAnimation);
                finishItems[itemIndex].dudeAnimController.PlayAnimation(finishItems[itemIndex].boyAnimation);

                bool isWin = stopPoint.index > 0;
                GameController.Instance.OnLevelCompleted(isWin, 1.0f);

                if (isWin)
                    GameController.Instance.resultPanel.resultText.text = "You\n" + rider.progressBarPositionUpdater.GetComponent<ProgressBar>().labelText.text + "!";
            })
            ;

        CameraSplineFollower.Instance.enabled = false;
        CameraSplineFollower.Instance.transform.DOMove(stopPoint.cameraTransform.position, 0.8f);
        CameraSplineFollower.Instance.transform.DORotateQuaternion(stopPoint.cameraTransform.rotation, 0.8f);
       
    }
}
