using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdater : MonoBehaviour
{
    public Transform target;
    public float speed = 20;

    private Vector3 _offset;

    private void Awake()
    {
        _offset = transform.position - target.position;
        _offset.x = _offset.z = 0;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + _offset, speed * Time.deltaTime);
    }
}
