using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestType { Photo, Collect, Repair, Null };

public class Quest : ScriptableObject
{
    public QuestData questData;

    public UnityEvent<QuestData> QuestCompleted;

    public virtual void StartQuest()
    {
        questData.isActive = true;
    }

    void Update()
    {

    }

    public virtual void QuestUpdate()
    {

    }

    public virtual void CheckPhoto(Texture2D newPhoto, bool correct)
    {

    }

    public virtual Transform GetPhotoTarget()
    {
        return null;
    }

    public void EndQuest()
    {
        QuestController.instance.CompleteQuest(questData);
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
