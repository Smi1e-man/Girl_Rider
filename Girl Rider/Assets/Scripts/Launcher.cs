using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        Account.Load();

        SceneManager.LoadScene(Account.SceneName);
    }
}
