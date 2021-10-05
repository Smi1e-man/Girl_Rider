using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ResultPanel : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI resultText;
    public ProgressUnlockable progressUnlockable;
    public MoneyLabel moneyLabel;
    public CoinIconSpawner coinIconSpawner;

    [Space]
    public Button continueButton;
    public Button retryButton;

    private Color _defaultBackgroundColor;
    private bool _isWin;

    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinueButtonPressed);
        retryButton.onClick.AddListener(OnRetryButtonPressed);

        gameObject.SetActive(false);

        _defaultBackgroundColor = background.color;
    }

    public void Show(bool isWin, float delay)
    {
        _isWin = isWin;

        if (isWin)
        {
            resultText.text = "You\nPure Beauty!";

        }
        else
        {
            resultText.text = "Fail!";
        }

        continueButton.gameObject.SetActive(isWin);
        retryButton.gameObject.SetActive(!isWin);

        PreShowAnimation();
        RunShowAnimation(delay);

    }

    private void PreShowAnimation()
    {
        var c = background.color;
        c.a = 0;
        background.color = c;

        resultText.transform.localScale = Vector3.zero;
        continueButton.transform.localScale = Vector3.zero;
        retryButton.transform.localScale = Vector3.zero;

        gameObject.SetActive(true);
        background.gameObject.SetActive(true);


        if (_isWin && GameController.Instance.unlockableController.isAnythingToUnlock)
        {
            progressUnlockable.SetData(
                GameController.Instance.unlockableController.currentUnlockableItem.sprite,
                GameController.Instance.unlockableController.currentUnlockableProgress
                );
        }
    }

    private void RunShowAnimation(float delay)
    {

        background.DOColor(_defaultBackgroundColor, 1).SetEase(Ease.InSine).SetId(GetInstanceID()).SetDelay(delay);
        resultText.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(delay + 0.5f);

        if (_isWin)
        {
            float rewardShowDelay = 0.6f;

            StartRewardAnim(delay + rewardShowDelay, 1.35f);

            if (GameController.Instance.unlockableController.isAnythingToUnlock)
                ShowUnlockablePanel(delay + rewardShowDelay + 3f);
            else
                continueButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(delay + rewardShowDelay + 2.2f);

        }
        else
        {
            retryButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(delay + 0.6f);
        }

    }

    private void OnContinueButtonPressed()
    {
        continueButton.interactable = false;
        Invoke("NextScene", 0.2f);
    }

    private void StartRewardAnim(float delay, float duration)
    {
        int baseMoney = GirlStateUI.Instance.value;
        int mult = GirlStateUI.Instance.mult;

        int moneyToAdd = baseMoney * mult;
        int iconsCount = Mathf.Clamp(moneyToAdd / 5, 25, 100);

        GirlStateUI.Instance.multText.text = "x" + mult;

        GirlStateUI.Instance.transform.DOLocalMove(new Vector3(-30, -170, 0), 0.25f).SetRelative(true);
        GirlStateUI.Instance.transform.DOScale(1.5f, 0.25f);
        Sequence s = DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() => {

                DOVirtual.Float(baseMoney, moneyToAdd, 0.5f, x => {
                    GirlStateUI.Instance.counterText.text = String.Format("{0:n0}", Math.Round(x));
                });
                
                
            })
            .AppendInterval(duration)
            .AppendCallback(() =>
            {
                coinIconSpawner.Spawn(GirlStateUI.Instance.root.transform.position, 250, iconsCount);

                moneyLabel.AnimateCounterAdd(moneyToAdd, 0.5f);
                moneyLabel.AddAndSave(moneyToAdd);


            })
            .AppendInterval(0.1f)
            .AppendCallback(() => {
                GirlStateUI.Instance.root.SetActive(false);
            })
            ;
    }

    private void ShowUnlockablePanel(float delay)
    {
        Sequence sequence = DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(()=> {
                    resultText.gameObject.SetActive(false);

                    progressUnlockable.imageRoot.localScale = Vector3.one * 0.001f;
                    progressUnlockable.titleLabel.transform.localScale = Vector3.one * 0.001f;

                    progressUnlockable.root.gameObject.SetActive(true);


                    progressUnlockable.imageRoot.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(0);
                    progressUnlockable.titleLabel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(0.1f);

                })
                .AppendInterval(0.8f)
                ;

        UnlockableController unlockableController = GameController.Instance.unlockableController;
        bool isUnlocked = unlockableController.AddProgressAndSave();

        float progressAnimDuration = 0.8f;
        sequence.AppendCallback(() =>
            progressUnlockable.AnimateProgress(unlockableController.currentUnlockableItem.deltaProgress, progressAnimDuration));

        sequence.AppendInterval(progressAnimDuration);


        if (isUnlocked)
        {
            sequence.AppendCallback(() => progressUnlockable.progressLabel.gameObject.SetActive(false));
            //sequence.AppendCallback(() => progressUnlockable.progressLabel.text = "UNLOCKED!");
            sequence.Append(progressUnlockable.blackImg.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));

        }


        sequence.Append(continueButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack));
        //sequence.AppendInterval(0.85f);
        //sequence.AppendCallback(() => Invoke("NextScene", 0.2f));
    }

    private void OnRetryButtonPressed()
    {
        retryButton.interactable = false;
        Invoke("Restart", 0.2f);

    }

    private void NextScene()
    {
        Account.Level++;


        DOTween.KillAll();
        SceneManager.LoadScene(Account.SceneName);
    }

    private void Restart()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
