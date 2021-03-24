using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour {
    public Vector3[] TreePositions { get; private set; }
    public Vector3[] RockPositions { get; private set; }
    public Vector3[] BushPositions { get; private set; }

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

    public Bounds GenerateTerrain(bool pseudoRandom = false) {
        // generate base terrain
        TerrainData tData = _terrain.terrainData;
        if (pseudoRandom) {
            tData.heightmapResolution = terrainSize + 1;
            tData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
            tData.SetHeights(0, 0, GenerateHeights());
        }
        _collider.terrainData = tData;
        Vector3 center = new Vector3(tData.size.x * 0.5f, 0, tData.size.z * 0.5f);

        // get trees, rocks, and bushes
        List<Vector3> trees = new List<Vector3>();
        List<Vector3> rocks = new List<Vector3>();
        List<Vector3> bushes = new List<Vector3>();
        for (int i=0; i<tData.treeInstances.Length; i++) {
            switch (tData.treeInstances[i].prototypeIndex) {
                case 0:
                    trees.Add(tData.treeInstances[i].position);
                    break;
                case 1:
                    rocks.Add(tData.treeInstances[i].position);
                    break;
                case 2:
                    bushes.Add(tData.treeInstances[i].position);
                    break;
                default:
                    break;
            }
        }
        TreePositions = trees.ToArray();
        RockPositions = rocks.ToArray();
        BushPositions = bushes.ToArray();

        UpdateNavMesh();

        return new Bounds(center, new Vector3(tData.size.x, 0, tData.size.z));
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