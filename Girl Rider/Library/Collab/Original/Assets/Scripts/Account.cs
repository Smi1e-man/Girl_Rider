using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Account
{
    public static bool shuffleLevels = true;
    private static bool _isLoaded;

    public static int AB_TEST_GROUP_ID;

    private static int _level;
    public static int Level
    {
        get
        {
            return _level;
        }

        set
        {
            _level = value;
            PlayerPrefs.SetInt("level", _level);
        }
    }

    private static List<string> _sceneNames = new List<string>()
    {
        "level_01",
        "level_02",
        "level_03",
        "level_04",
        "level_04",

    };

    /*
    private static List<string> _defaultSceneNames = new List<string>()
    {
        "level_01",
        "level_02",
        "level_03",
        "level_04",
        "level_05",
        "level_06",
        "level_07",
        "level_08",
        "level_09",
        "level_10",
        "level_11",
        "level_12",
        "level_13",
        "level_14",
    };

    private static List<string> _hardcoreSceneNames = new List<string>()
    {
        "b_01",
        "b_02",
        "b_03",
        "b_04",
        "b_05",
        "b_06",
        "b_07",
        "b_08",
        "b_09",
        "b_10",

    };*/

    private static List<string> _shuffledSceneNames;

    public static string SceneName
    {
        get
        {
            return shuffleLevels && Level >= _sceneNames.Count ? _shuffledSceneNames[Level % _shuffledSceneNames.Count] : _sceneNames[Level % _sceneNames.Count];
        }
        
    }

    public static bool IsShuffled => Level >= _sceneNames.Count;

    public static void Load()
    {
        if (_isLoaded) return;
        _isLoaded = true;

        /*
        string ab_test_key = "abTest";
        if (!PlayerPrefs.HasKey(ab_test_key))
        {
            AB_TEST_GROUP_ID = Random.value > 0.5f ? 0 : 1;
            PlayerPrefs.SetInt(ab_test_key, AB_TEST_GROUP_ID);

            Debug.Log("***** AB_TEST_GROUP_ID is set to: " + AB_TEST_GROUP_ID);
        }
        else
            AB_TEST_GROUP_ID = PlayerPrefs.GetInt(ab_test_key);

        GameAnalyticsSDK.GameAnalytics.SetCustomDimension01(AB_TEST_GROUP_ID == 0 ? "releaseToKill" : "autoKill");
        */

        _level = PlayerPrefs.GetInt("level", 0);

        //_sceneNames = new List<string>(_ABTestGroup ? _defaultSceneNames : _hardcoreSceneNames);

        _shuffledSceneNames = new List<string>(_sceneNames);
        Shuffle(_shuffledSceneNames);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, (n + 1));
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void ClearSaves()
    {
        PlayerPrefs.DeleteAll();
    }
}
