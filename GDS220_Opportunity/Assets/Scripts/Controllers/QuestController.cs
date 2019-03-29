using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField]
    Quest[] quests;

    Quest currentQuest;

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
                currentQuest.QuestCompleted.AddListener(OnQuestCompleted);
                currentQuestType = currentQuest.questData.questType;
                break;
            }
        }
    }

    public void OnQuestCompleted(QuestData completedQuestData)
    {
        
    }

}
