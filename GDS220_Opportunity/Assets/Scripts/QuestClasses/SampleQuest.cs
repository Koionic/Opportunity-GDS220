using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SampleQuest : Quest
{
    [SerializeField]
    Vector3 targetArea;

    [SerializeField]
    float targetThreshold;

    bool inTargetArea;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void StartQuest()
    {
        base.StartQuest();
    }

    // Update is called once per frame
    public override void QuestUpdate()
    {
        base.QuestUpdate();
        CheckPlayerLocation();
    }

    void CheckPlayerLocation()
    {
        if (distanceFromQuest < targetThreshold)
        {
            inTargetArea = true;
        }
        else
        {
            inTargetArea = false;
        }
    }

    public override void CheckSample(SampleData sample)
    {
        if (inTargetArea)
        {
            questData.success = true;
        }

        EndQuest();
    }
}
