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

    // Update is called once per frame
    public override void QuestUpdate()
    {
        base.QuestUpdate();
        CheckPlayerLocation();
    }

    void CheckPlayerLocation()
    {
        Vector3 distanceFromTargetArea = targetArea - RoverController.instance.stats.currentPosition;

        if (Mathf.Abs(distanceFromTargetArea.x) < targetThreshold && Mathf.Abs(distanceFromTargetArea.z) < targetThreshold)
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
            EndQuest();
        }
    }
}
