using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
    [SerializeField] bool regenerateMap = true;
    [SerializeField] int terrainSize = 256;
    [SerializeField] int terrainHeight = 10;
    [SerializeField] float scale = 1;
    [SerializeField] Vector2 offset = Vector2.zero;
    Terrain _terrain;

    private void Awake() {
        _terrain = GetComponent<Terrain>();
    }

    private void Update() {
        if (regenerateMap) {
            GenerateTerrain();
            regenerateMap = false;
        }
    }

    void GenerateTerrain() {
        TerrainData tData = _terrain.terrainData;
        tData.heightmapResolution = terrainSize + 1;
        tData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
        tData.SetHeights(0, 0, GenerateHeights());
        transform.position = new Vector3(-terrainSize * 0.5f, 0, -terrainSize * 0.5f);
    }

    float[,] GenerateHeights() {
        float[,] heights = new float[terrainSize, terrainSize];
        for (int y=0; y<heights.GetLength(1); y++) {
            for(int x = 0; x < heights.GetLength(0); x++) {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y) {
        float xCoord = x / (float)terrainSize * scale + offset.x;
        float yCoord = y / (float)terrainSize * scale + offset.y;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}