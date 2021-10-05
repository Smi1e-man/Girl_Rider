using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EmojiPanel : MonoBehaviour
{
    public Image image;
    public Transform imageRoot;

    public List<Sprite> levelUpEmojis;
    public List<Sprite> levelDownEmojis;

    [Space]
    public float visibleTime = 1.0f;

    private Vector3 _initialScale;

    private void Awake()
    {
        _initialScale = imageRoot.localScale;
        imageRoot.localScale = 0.001f * _initialScale;
    }
    public void OnNewState(bool isLevelUp)
    {
        List<Sprite> sprites = isLevelUp ? levelUpEmojis : levelDownEmojis;
        image.sprite = sprites[Random.Range(0, sprites.Count)];

        DOTween.Kill(GetInstanceID());
        Show();
        DOVirtual.DelayedCall(visibleTime, Hide).SetId(GetInstanceID());
    }

    public void Show()
    {
        DOTween.Kill(GetInstanceID());
        imageRoot.DOScale(_initialScale, 0.3f).SetId(GetInstanceID()).SetEase(Ease.OutCubic);
    }

    public void Hide()
    {
        DOTween.Kill(GetInstanceID());
        imageRoot.DOScale(0.001f * _initialScale, 0.1f).SetId(GetInstanceID()).SetEase(Ease.InCubic);

    }
}
