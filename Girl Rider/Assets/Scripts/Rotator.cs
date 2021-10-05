using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private Rigidbody _rb;
    public Vector3 speed;

    private float _speedMult = 1;
    private float _targetSpeedMult = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _speedMult = Mathf.Lerp(_speedMult, _targetSpeedMult, 2 * Time.deltaTime);
        transform.Rotate(speed * _speedMult * Time.deltaTime * 60);
    }
}
