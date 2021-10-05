using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainJoystick : MonoBehaviour
{
    public static MainJoystick Instance;

    public Joystick joystick;

    private void Awake()
    {
        Instance = this;
    }
}
