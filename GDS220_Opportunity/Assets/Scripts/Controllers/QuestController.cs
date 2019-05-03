using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField]
    Quest[] mainQuests;

    public Quest mainQuest;
    public Quest socialQuest1, socialQuest2;

    int currentQuestIndex;

    Vector3 playerLocation;
    Vector2 playerLocationV2;


    float distanceFromQuest;
    
    Vector2 currentQuestLocationV2;


    public static QuestController instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneController.instance != null)
        {
            if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.Game))
            {
                if (mainQuest != null)
                {
                    mainQuest.QuestUpdate();
                }
            }
        }
    }

    public void ResetAllMainQuests()
    {
        mainQuest = null;

        for (int i = 0; i < mainQuests.Length; i++)
        {
            mainQuests[i].questData.isActive = false;
            mainQuests[i].questData.isCompleted = false;
            mainQuests[i].questData.success = false;
        }

        RepairData[] repairDatas = FindObjectsOfType<RepairData>();
        foreach (RepairData data in repairDatas)
        {
            data.repaired = false;
        }
        SampleData[] sampleDatas = FindObjectsOfType<SampleData>();
        foreach (SampleData data in sampleDatas)
        {
            data.sampled = false;
        }
    }

    public void StartNewMainQuest()
    {
        Debug.Log("starting quest");
        for (int i = 0; i < mainQuests.Length; i++)
        {
            if (!mainQuests[i].questData.isCompleted && QuestTypeIsAvailable(mainQuests[i].GetType()))
            {
                currentQuestIndex = i;
                mainQuest = mainQuests[currentQuestIndex];
                mainQuest.StartQuest();

                UIController.instance.StartQuestUI(mainQuest);
                break;
            }
        }
    }

    public void SendPhoto(Texture2D texture, bool correct)
    {
        Quest cameraQuest = ActiveQuestOfType(typeof(CameraQuest));

        if (cameraQuest != null)
        {
            cameraQuest.CheckPhoto(texture, correct);
        }
    }

    public void SendSample(SampleData sample)
    {
        Quest sampleQuest = ActiveQuestOfType(typeof(SampleQuest));

        if (sampleQuest != null)
        {
            sampleQuest.CheckSample(sample);
        }
    }

    bool QuestTypeIsAvailable(System.Type type)
    {
        bool isAvailable = true;

        if (mainQuest != null && mainQuest.GetType() == type)
        {
            isAvailable = false;
        }
        if (socialQuest1 != null && socialQuest1.GetType() == type)
        {
            isAvailable = false;
        }
        if (socialQuest2 != null && socialQuest2.GetType() == type)
        {
            isAvailable = false;
        }

        return isAvailable;
    }

    public void CompleteQuest(Quest completedQuest)
    {
        UIController.instance.FinishQuestUI(completedQuest);

        if (completedQuest.tutorialQuest)
        {
            if (completedQuest.GetType() == typeof(CameraQuest))
            {
                UIController.instance.EnableCompassUI();
            }
            if (completedQuest.GetType() == typeof(RepairQuest))
            {
                UIController.instance.EnableGroundControl();
            }
            if (completedQuest.GetType() == typeof(SampleQuest))
            {
                UIController.instance.EnableHomeWaypoint();
            }

            UIController.instance.HideTutorialText();
            if (UIController.instance.groundControlEnabled)
            {
                DialogueController.instance.QueueGCDialogue(completedQuest.succeedQuestText);
            }
            else
            {
                UIController.instance.ShowRoverLog(completedQuest.succeedQuestText);
            }

            Invoke("HideRoverLog", 2f);
        }
        else if (completedQuest.spawnedObject != null)
        {
            GameController.instance.AddObjectToDeleteList(completedQuest.spawnedObject.gameObject);
        }

        completedQuest.questData.isCompleted = true;
        completedQuest.questData.isActive = false;

        if (mainQuest != null && mainQuest == completedQuest)
        {
            mainQuest = null;

            Invoke("StartNewMainQuest", 2f);
        }
        else if (socialQuest1 != null && socialQuest1 == completedQuest)
        {
            socialQuest1 = null;
        }
        else if (socialQuest2 != null && socialQuest2 == completedQuest)
        {
            socialQuest2 = null;
        }

        completedQuest = null;
    }

    void HideRoverLog()
    {
        UIController.instance.HideRoverLog();
    }

    public Quest ActiveQuestOfType(System.Type type)
    {
        if (mainQuest != null && mainQuest.GetType() == type)
            return mainQuest;

        if (socialQuest1 != null && socialQuest1.GetType() == type)
            return socialQuest1;

        if (socialQuest2 != null && socialQuest2.GetType() == type)
            return socialQuest2;

        return null;
    }

}
