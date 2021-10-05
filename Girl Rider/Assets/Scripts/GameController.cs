using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public ResultPanel resultPanel;
    public GamePanel gamePanel;
    public UnlockableController unlockableController;

    private float _startTime;
    private bool _isGameStarted;
    public bool IsGameStarted => _isGameStarted;

    public float OnFinishInteractTime { get; set; }
    public int OnFinishItemIndex { get; set; }

    private void Awake()
    {
        Instance = this;

        resultPanel.gameObject.SetActive(true);

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        gamePanel.levelText.text = "" + (Account.Level + 1);

    }

    private void Update()
    {
        if (!_isGameStarted && Input.GetMouseButtonDown(0) && RaycastMouse().Count <= 1)
            StartGame();
    }

    public void StartGame()
    {
        _isGameStarted = true;

        HandleStartEvents();
    }

    public void OnLevelCompleted(bool isWin = true, float delay = 0.5f)
    {
        HandleCompleteEvents(isWin);

        if (isWin)
        {

        }

        resultPanel.Show(isWin, delay);
    }

    private void HandleStartEvents()
    {
        string levelNumber = Utility.IntToString(Account.Level + 1, 5);
        _startTime = Time.time;

        //GameAnalyticsSDK.GameAnalytics.SetCustomDimension01(Account.AB_TEST_GROUP_ID == 0 ? "releaseToKill" : "autoKill");
        TinySauce.OnGameStarted(levelNumber);

        Debug.Log("HandleStartEvents | levelNumber: " + levelNumber);
    }

    private void HandleCompleteEvents(bool isWin)
    {
        string levelNumber = Utility.IntToString(Account.Level + 1, 5);
        string sceneName = SceneManager.GetActiveScene().name;// Account.SceneName;

        int levelDuration = Mathf.RoundToInt(OnFinishInteractTime - _startTime);

        int finishScore = isWin ? OnFinishItemIndex : -1;

        TinySauce.OnGameFinished(isWin, finishScore, levelNumber);

        if (!Account.IsShuffled)
        {
            if (isWin)
                TinySauce.TrackCustomEvent(levelNumber + "_win");
            else
                TinySauce.TrackCustomEvent(levelNumber + "_fail");
        }

        if (isWin)
            TinySauce.TrackCustomEvent(sceneName + "_win");
        else
            TinySauce.TrackCustomEvent(sceneName + "_fail");


        if (isWin)
        {
            TinySauce.TrackCustomEvent(sceneName + "_levelDuration_" + levelDuration);
            TinySauce.TrackCustomEvent(sceneName + "_finishScore_" + finishScore);

            TinySauce.TrackCustomEvent("level_duration", new Dictionary<string, object>() { { sceneName, levelDuration } });
            TinySauce.TrackCustomEvent("finish_score", new Dictionary<string, object>() { { sceneName, finishScore } });
        }

        Debug.Log("HandleCompleteEvents | levelNumber: " + levelNumber +  "   | finishScore = " + finishScore);
    }

    public List<RaycastResult> RaycastMouse()
    {

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

}
