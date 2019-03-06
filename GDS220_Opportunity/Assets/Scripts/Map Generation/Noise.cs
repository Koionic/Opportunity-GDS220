using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public enum NormalizeMode {Local, Global};

    //Creates a 2D float array of perlin noise dictated by the inputted values
    //Octaves refer to the amount of layers of noise used to add finer detail
    //Lacunarity increases the frequency of the noise layer each octave
    //Persistance decreases the amplitude of the noise layer each octave
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
     {
        float[,] noiseMap = new float[mapWidth, mapHeight];


        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0f;

        //amplitude dictates the effect of the noise layer on the whole map
        float amplitude = 1;
        //frequency dictates the amount of detail within the noise layer
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        //iterating from bottom to top
        for (int y = 0; y < mapHeight; y++)
        {
            //iterating from left to right
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                //the value accumulating all the layers into the final height value
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    //creates sampling coordinates to read perlin values
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    //creates a perlin noise value between -1 and 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    //alters the height based on its amplitude (less alteration in higher octaves)
                    noiseHeight += perlinValue * amplitude;

                    //decreases the amplitude of the next octave
                    amplitude *= settings.persistance;
                    //increases the frequency of the next octave
                    frequency *= settings.lacunarity;
                }

                //tracks the lowest and highest values for inverse lerp
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                        //normalises the values between 0 and 1 to send to MapGenerator
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50;

    [Tooltip("The amount of layers of noise used to add finer detail")]
    public int octaves = 6;
    [Range(0, 1)]
    [Tooltip("The rate of which the the amplitude of each octave is decreased")]
    public float persistance = .6f;
    [Tooltip("The rate of which the the frequency of each octave is increased")]
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}