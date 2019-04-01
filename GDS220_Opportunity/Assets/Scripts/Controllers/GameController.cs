using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    bool isPaused = false;
    bool gameOver = false;

    public static GameController instance = null;

    [SerializeField]
    GameObject roverPrefab;

    public RoverController roverController;

    [SerializeField]
    RoverStats savedRoverStats;

    bool quickSaveReady;
    float quickSaveTimer = 0f;

    [SerializeField]
    float quickSaveDelayInMinutes;

    bool newLevel = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
            
    }

    public void StartLevel()
    {
        roverController.FreezeRoverStates(RoverController.FreezeType.All, false);

        QuestController.instance.StartNewQuest();
    }

    void EndLevel()
    {
        gameOver = true;
        isPaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.Game))
        {
            quickSaveTimer += Time.deltaTime;
            quickSaveReady = quickSaveTimer >= quickSaveDelayInMinutes * 60f;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (quickSaveReady)
        {
            Save();
        }
    }

    public void Exit()
    {
        Save();
        gameOver = false;
        isPaused = false;
        SceneController.instance.MainMenu();
    }

    public void LoadSavedStats(int saveFileIndex)
    {
        savedRoverStats = SaveFiles.instance.GrabSaveFile(saveFileIndex);
    }

    public void Save()
    {
        savedRoverStats = roverController.stats;
        quickSaveTimer = 0f;
        print("saved!");
    }

    public void PrepareRover()
    {
        Vector3 spawnPoint = savedRoverStats.lastSavedPosition;
        if (newLevel)
        {
            spawnPoint = LevelDataHolder.instance.levelData[LevelDataHolder.instance.currentLevel].levelStats.lastSavedPosition;
            newLevel = false;
        }


        roverController = RoverController.instance;
        roverController.gameObject.transform.position = spawnPoint;

        roverController.FreezeRoverStates(RoverController.FreezeType.All, true);
        roverController.OutOfBattery.AddListener(EndLevel);

        savedRoverStats.currentPosition = spawnPoint;
        savedRoverStats.lastSavedPosition = spawnPoint;
        savedRoverStats.batteryLife = savedRoverStats.maxBattery;
        roverController.stats = savedRoverStats;
    }

    public bool GameOver()
    {
        return gameOver;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
