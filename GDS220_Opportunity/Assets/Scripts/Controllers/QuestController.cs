using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField]
    Quest[] quests;

    public Quest currentQuest;

    public QuestType previousQuestType;
    public QuestType currentQuestType;

    int currentQuestIndex;

    [SerializeField]
    float questObjectSpawnDistance;
    GameObject questObject;
    public GameObject spawnedObject;

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
        if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.Game))
        {

            if (currentQuest != null)
            {
                currentQuest.QuestUpdate();
            }


            playerLocation = GameController.instance.roverController.stats.currentPosition;

            playerLocationV2 = new Vector2(playerLocation.x, playerLocation.z);

            distanceFromQuest = (currentQuestLocationV2 - playerLocationV2).magnitude;

            if (distanceFromQuest <= questObjectSpawnDistance)
            {
                if (currentQuest != null)
                {
                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(currentQuest.questPrefab, currentQuest.questLocation, Quaternion.identity);
                        questObject = spawnedObject;
                    }
                    else if (!spawnedObject.activeSelf)
                    {
                        spawnedObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (currentQuest != null && spawnedObject != null)
                {
                    if (spawnedObject == questObject && spawnedObject.activeSelf)
                    {
                        spawnedObject.SetActive(false);
                    }
                }
                else
                {
                    Destroy(spawnedObject);
                    spawnedObject = null;
                    questObject = null;
                }
            }
        }
    }

    public void StartNewQuest()
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (!quests[i].questData.isCompleted)
            {
                currentQuestIndex = i;
                currentQuest = quests[currentQuestIndex];
                currentQuest.StartQuest();
                currentQuestType = currentQuest.questData.questType;
                currentQuestLocationV2 = new Vector2(currentQuest.questLocation.x, currentQuest.questLocation.z);


                UIController.instance.StartQuestUI(currentQuest);
                break;
            }
        }
    }

    void SendPhoto(Texture2D texture, bool correct)
    {
        print("questcontroller sending");
        currentQuest.CheckPhoto(texture, correct);
    }

    public void SendSample(SampleData sample)
    {
        UIController.instance.ChangeSampleText(sample.sampleType.ToString());

        if (currentQuestType == QuestType.Sample)
        {
            currentQuest.CheckSample(sample);
        }
    }

    public void CompleteQuest(QuestData completedQuestData)
    {
        UIController.instance.FinishQuestUI(currentQuest);

        currentQuest.questData.isCompleted = true;
        currentQuest.questData.isActive = false;
        currentQuest = null;
        currentQuestType = QuestType.Null;
    }

}
