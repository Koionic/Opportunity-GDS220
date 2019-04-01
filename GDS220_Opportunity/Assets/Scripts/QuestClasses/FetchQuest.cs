using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FetchQuest : Quest
{
    [SerializeField]
    Vector3 targetArea;

    float targetThreshold;

    bool inTargetArea;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void QuestUpdate()
    {
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
}
