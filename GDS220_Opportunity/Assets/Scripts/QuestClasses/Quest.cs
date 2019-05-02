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

    public QuestTarget spawnedObject;

    [SerializeField]
    float questObjectSpawnDistance;

    public string targetName;

    public string startQuestText, failQuestText, succeedQuestText;

    public bool tutorialQuest;

    protected float distanceFromQuest;

    public virtual void StartQuest()
    {
        questData.isActive = true;

        if (tutorialQuest)
        {
            UIController.instance.ShowRoverLog(startQuestText);
            UIController.instance.ShowTutorialText("Press Space to Enter Camera Mode");
        }
        else
        {
            if (startQuestText != null)
            {
                Debug.Log("queuing start text");
                DialogueController.instance.QueueGCDialogue(startQuestText);
            }
        }
    }

    public virtual void QuestUpdate()
    {
        distanceFromQuest = Maths.GetDistance(RoverController.instance.stats.currentPosition, questLocation);

        if (!tutorialQuest)
        {
            if (distanceFromQuest <= questObjectSpawnDistance)
            {
                if (spawnedObject != null)
                {
                    spawnedObject.gameObject.SetActive(true);
                }
                else
                {
                    spawnedObject = Instantiate(questPrefab, questLocation, Quaternion.identity, GameObject.FindWithTag("ObjectContainer").transform).GetComponent<QuestTarget>();
                }
            }
            else
            {
                if (spawnedObject != null)
                {
                    if (spawnedObject.gameObject.activeSelf)
                    {
                        spawnedObject.gameObject.SetActive(false);
                    }
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
