using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour {
    [SerializeField] bool regenerateMap = true;
    public  int terrainSize = 256;
    [SerializeField] int terrainHeight = 10;
    [SerializeField] float scale = 1;
    [SerializeField] Vector2 offset = Vector2.zero;
    Terrain _terrain;
    TerrainCollider _collider;
    NavMeshSurface _surface;

    private void Awake() {
        _terrain = GetComponent<Terrain>();
        _collider = GetComponent<TerrainCollider>();
        _surface = GetComponentInParent<NavMeshSurface>();
    }

    private void Update() {
        /*if (regenerateMap) {
            GenerateTerrain();
            regenerateMap = false;
        }*/
    }

    public void GenerateTerrain(bool pseudoRandom = false) {
        // generate base terrain
        TerrainData tData = _terrain.terrainData;
        if (pseudoRandom) {
            tData.heightmapResolution = terrainSize + 1;
            tData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
            tData.SetHeights(0, 0, GenerateHeights());
        } else {
            terrainSize = (int)tData.size.x;
        }
        _collider.terrainData = tData;
        transform.position = new Vector3(-tData.size.x * 0.5f, 0, -tData.size.z * 0.5f);
        // generate trees ?

        UpdateNavMesh();
    }

    float[,] GenerateHeights() {
        float[,] heights = new float[_terrain.terrainData.heightmapResolution, _terrain.terrainData.heightmapResolution];
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

    public void UpdateNavMesh() {
        _surface.BuildNavMesh();
    }
}