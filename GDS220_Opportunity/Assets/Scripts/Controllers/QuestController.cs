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
        if (currentQuest != null)
        {
            currentQuest.QuestUpdate();


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
        currentQuest.questData.isCompleted = true;
        currentQuest.questData.isActive = false;
        currentQuest = null;
        currentQuestType = QuestType.Null;
    }

}
