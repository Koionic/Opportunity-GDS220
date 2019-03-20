using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataHolder : MonoBehaviour
{
    public int currentLevel;

    public LevelData[] levelData;

    public static LevelDataHolder instance = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }
}

[System.Serializable]
public struct LevelData
{
    public string levelName;
    public TextureData levelTexture;
    public HeightMapSettings levelMap;
    public RoverStats levelStats;
}
