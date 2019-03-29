using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestType { Photo, Collect, Repair };

public class Quest : ScriptableObject
{
    public QuestData questData;

    public UnityEvent<QuestData> QuestCompleted;

    protected RoverController roverController;

    public virtual void StartQuest()
    {
        questData.isActive = true;
        roverController = FindObjectOfType<RoverController>();
    }

    void Update()
    {
        if (questData.isActive)
        {
            QuestUpdate();
        }
    }

    protected virtual void QuestUpdate()
    {

    }

    public void CompleteQuest()
    {
        questData.isCompleted = true;
        QuestCompleted.Invoke(questData);
    }
}

[System.Serializable]
public struct QuestData
{
    public bool isActive;
    public bool isCompleted;

    public float gcBias;
    public float scBias;

    public QuestType questType;
}
