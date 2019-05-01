using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VectorMaths;

public enum QuestType { Photo, Sample, Repair, Null };

public class Quest : ScriptableObject
{
    public QuestData questData;

    public UnityEvent<QuestData> QuestCompleted;

    public Vector3 questLocation;
    public GameObject questPrefab;

    public GameObject spawnedObject;

    [SerializeField]
    float questObjectSpawnDistance;

    public string targetName;

    public string startQuestText, failQuestText, succeedQuestText;

    public virtual void StartQuest()
    {
        questData.isActive = true;

        if (startQuestText != null)
        {
            Debug.Log("queuing start text");
            DialogueController.instance.QueueGCDialogue(startQuestText);
        }
    }

    public virtual void QuestUpdate()
    {
        float distanceFromQuest = Maths.GetDistance(RoverController.instance.stats.currentPosition, questLocation);

        if (distanceFromQuest <= questObjectSpawnDistance)
        {
            if (spawnedObject != null)
            {
                spawnedObject.SetActive(true);
            }
            else
            {
                spawnedObject = Instantiate(questPrefab, questLocation, Quaternion.identity, QuestController.instance.transform);
            }
        }
        else
        {
            if (spawnedObject != null)
            {
                if (spawnedObject.activeSelf)
                {
                    spawnedObject.SetActive(false);
                }
            }
        }
    }

    public virtual void CheckPhoto(Texture2D newPhoto, bool correct)
    {

    }

    public virtual void CheckSample(SampleData sample)
    {

    }

    public void EndQuest()
    {
        QuestController.instance.CompleteQuest(this);
    }
}

[System.Serializable]
public struct QuestData
{
    public bool isActive, isCompleted;
    public bool success;

    public float gcBias;
    public float scBias;
}
