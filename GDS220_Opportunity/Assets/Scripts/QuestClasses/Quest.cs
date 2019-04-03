using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestType { Photo, Sample, Repair, Null };

public class Quest : ScriptableObject
{
    public QuestData questData;

    public UnityEvent<QuestData> QuestCompleted;

    public Vector3 questLocation;

    public string targetName;

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

    public virtual void CheckSample(SampleData sample)
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
