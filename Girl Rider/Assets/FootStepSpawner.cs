using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSpawner : MonoBehaviour
{
    public float delay = 0.1f;

    [Space]
    public GameObject leftFootPrefab;
    public GameObject rightFootPrefab;
    [Space]
    public Transform leftFootTransform;
    public Transform rightFootTransform;


    private float _lastSpawnTime;
    private bool _lastSide;


    private void Update()
    {
        if (Time.time - _lastSpawnTime > delay)
        {
            _lastSpawnTime = Time.time;

            GameObject footStep = Instantiate(_lastSide ? leftFootPrefab : rightFootPrefab);

            footStep.transform.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);

            Vector3 pos = _lastSide ? leftFootTransform.position : rightFootTransform.position;
            pos.y = transform.position.y + 0.03f;
            footStep.transform.position = pos + transform.forward * 0.2f;

            _lastSide = !_lastSide;

            Destroy(footStep, 5);
        }
    }
}
