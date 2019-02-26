using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //Creates a 2D float array of perlin noise dictated by the inputted values
    //Octaves refer to the amount of layers of noise used to add finer detail
    //Lacunarity increases the frequency of the noise layer each octave
    //Persistance decreases the amplitude of the noise layer each octave
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];


        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY); 
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        //iterating from bottom to top
        for (int y = 0; y < mapHeight; y++)
        {
            //iterating from left to right
            for (int x = 0; x < mapWidth; x++)
            {
                //amplitude dictates the effect of the noise layer on the whole map
                float amplitude = 1;
                //frequency dictates the amount of detail within the noise layer
                float frequency = 1;
                //the value accumulating all the layers into the final height value
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    //creates sampling coordinates to read perlin values
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    //creates a perlin noise value between -1 and 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    //alters the height based on its amplitude (less alteration in higher octaves)
                    noiseHeight += perlinValue * amplitude;

                    //decreases the amplitude of the next octave
                    amplitude *= persistance;
                    //increases the frequency of the next octave
                    frequency *= lacunarity;
                }

                //tracks the lowest and highest values for inverse lerp
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //normalises the values between 0 and 1 to send to MapGenerator
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
