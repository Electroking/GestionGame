using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour {
    [SerializeField] bool regenerateMap = true;
    [SerializeField] int terrainSize = 256;
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

    public void GenerateTerrain() {
        transform.position = new Vector3(-terrainSize * 0.5f, 0, -terrainSize * 0.5f);
        TerrainData tData = _terrain.terrainData;
        tData.heightmapResolution = terrainSize + 1;
        tData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
        tData.SetHeights(0, 0, GenerateHeights());
        _collider.terrainData = tData;
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