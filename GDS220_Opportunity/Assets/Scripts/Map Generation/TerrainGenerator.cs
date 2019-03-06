using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    const float viewerMoveThreshholdForChunkUpdate = 25f;
    const float sqrViewerMoveThreshholdForChunkUpdate = viewerMoveThreshholdForChunkUpdate * viewerMoveThreshholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public int startingRenderDistance;
    bool hasRendered;
    int chunksRenderedAtStart;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    [SerializeField] Transform viewer;
    [SerializeField] Material mapMaterial;

     Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunksVisibleInViewDist;

    int currentChunkCoordX;
    int currentChunkCoordY;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    void Start()
    {
        if (LevelDataHolder.instance != null)
        {
            heightMapSettings = LevelDataHolder.instance.levelData[LevelDataHolder.instance.currentLevel].levelMap;
            textureSettings = LevelDataHolder.instance.levelData[LevelDataHolder.instance.currentLevel].levelTexture;
        }

        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDist = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / meshWorldSize);
        chunksRenderedAtStart = Mathf.RoundToInt(startingRenderDistance / meshWorldSize);

        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach(TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThreshholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

        //loops through each chunk that was visible last update and turns it off
        for (int i = visibleTerrainChunks.Count - 1; i >= 0 ; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        //Rounds the players position to the center of current chunk
        currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        int renderDistance = hasRendered ? chunksVisibleInViewDist : chunksRenderedAtStart;

        for (int yOffset = -renderDistance; yOffset < renderDistance + 1; yOffset++)
        {
            for (int xOffset = -renderDistance; xOffset < renderDistance + 1; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);

                        newChunk.onVisiblityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }

        if (!hasRendered)
        {
            hasRendered = true;
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }

    public bool LevelIsLoaded()
    {
        return hasRendered;
    }
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshold;

    public float sqrVisibleDistanceThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }


}
