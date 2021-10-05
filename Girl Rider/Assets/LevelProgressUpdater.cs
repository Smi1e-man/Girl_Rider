using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUpdater : MonoBehaviour
{
    public Rider rider;
    public List<FinishAquapark> finishList;

    public Slider slider;
    public float levelDist;

    private List<float> _initialDists;

    private float _prevValue;

    void Start()
    {
        rider = CameraSplineFollower.Instance.target;

        finishList = new List<FinishAquapark>(FindObjectsOfType<FinishAquapark>());

        slider.value = _prevValue = 0;

        _initialDists = new List<float>();
        for (int i = 0; i < finishList.Count; i++)
        {
            _initialDists.Add(Vector3.Distance(rider.transform.position, finishList[i].transform.position));
        }
    }

    void Update()
    {
        float minP = 9999;
        for (int i = 0; i < finishList.Count; i++)
        {
            float p = Vector3.Distance(rider.transform.position, finishList[i].transform.position) / _initialDists[i];
            if (p < minP)
                minP = p;
            
        }

        slider.value = Mathf.Max(_prevValue, 1 - minP);

        _prevValue = slider.value;
    }
}
