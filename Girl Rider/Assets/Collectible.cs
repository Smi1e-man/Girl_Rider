using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float value = 0.2f;
    [Space]
    public Transform model;
    public Vector3 rotationSpeed;
    public float floatingSpeed;
    public float floatingHeight;
    public bool isCollected;
    public bool isPermanent;

    private Vector3 _initialPos;
    private Collider _trigger;

    private void Awake()
    {
        _initialPos = model.localPosition;
        _trigger = GetComponent<Collider>();
    }

    void Update()
    {
        if (!isCollected && !isPermanent)
        {
            model.localPosition = _initialPos + Vector3.up * Mathf.Sin(floatingSpeed * Time.time) * floatingHeight;
            model.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    public void OnCollect(Vector3 pos)
    {
        isCollected = true;
        _trigger.enabled = false;

        transform.DOMove(pos, 0.3f);
        model.DOScale(0, 0.3f).OnComplete(() => Destroy(gameObject));
    }
}
