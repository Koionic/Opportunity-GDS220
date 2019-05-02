using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RepairQuest : Quest
{
    public override void StartQuest()
    {
        base.StartQuest();
    }

    public override void QuestUpdate()
    {
        base.QuestUpdate();

    }

    public override void CheckRepair(RepairData repairData)
    {
        if (repairData.repaired)
        {
            EndQuest();
        }
    }
}
