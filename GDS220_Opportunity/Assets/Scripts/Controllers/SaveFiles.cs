using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFiles : MonoBehaviour
{
    RoverStats[] saveFiles = new RoverStats[3];

    public static SaveFiles instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }

    public RoverStats GrabSaveFile(int saveIndex)
    {
        return saveFiles[saveIndex];
    }

    public void SaveFile(RoverStats statsToSave)
    {
        saveFiles[statsToSave.saveFileIndex] = statsToSave;
    }
}
