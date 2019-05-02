using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairData : QuestTarget
{
    public Mesh repairedMesh;
    public bool repaired = false;
    public float repairTime;

    public void Repair()
    {
        repaired = true;

        if (repairedMesh != null)
        {
            GetComponent<MeshFilter>().mesh = repairedMesh;
        }
    }
}
