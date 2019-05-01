using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorMaths;

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

    List<GameObject> deleteQueue = new List<GameObject>();
    [SerializeField]
    float despawnDistance;

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

        QuestController.instance.StartNewMainQuest();
    }

    void RestartGame()
    {
        LoadingController.instance.StartFakeLoadingScreen();
        PrepareRover();
        DialogueController.instance.ResetStreams();
        QuestController.instance.ResetAllMainQuests();
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

            if (deleteQueue.Count > 0)
            {
                CheckObjectsToBeDeleted();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }

    public void AddObjectToDeleteList(GameObject obj)
    {
        deleteQueue.Add(obj);
    }

    void CheckObjectsToBeDeleted()
    {
        List<GameObject> objsToBeDeleted = new List<GameObject>();

        foreach (GameObject obj in deleteQueue)
        {
            float distance = Maths.GetDistance(roverController.transform.position, obj.transform.position);

            if (distance >= despawnDistance)
            {
                objsToBeDeleted.Add(obj);
            }
        }

        foreach (GameObject obj in objsToBeDeleted)
        {
            deleteQueue.Remove(obj);
            Destroy(obj);
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
            LevelData levelData = LevelDataHolder.instance.levelData[LevelDataHolder.instance.currentLevel];
            spawnPoint = levelData.levelStats.lastSavedPosition;
            newLevel = false;
        }

        roverController = RoverController.instance;
        roverController.gameObject.transform.position = spawnPoint;
        roverController.cameraMode = false;

        roverController.FreezeRoverStates(RoverController.FreezeType.All, true);

        roverController.OutOfBattery.RemoveListener(EndLevel);
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
