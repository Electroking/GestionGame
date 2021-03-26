using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour {
    public Vector3[] TreePositions { get; private set; }
    public Vector3[] RockPositions { get; private set; }
    public Vector3[] BushPositions { get; private set; }

    /*[SerializeField] bool regenerateMap = true;
    [SerializeField] int terrainSize = 256;
    [SerializeField] int terrainHeight = 10;
    [SerializeField] float scale = 1;
    [SerializeField] Vector2 offset = Vector2.zero;*/
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

    public void UpdateNavMesh() {
        _surface.BuildNavMesh();
    }

    public Bounds GenerateTerrain(bool pseudoRandom = false) {
        // generate base terrain
        TerrainData tData = _terrain.terrainData;
        /*if (pseudoRandom) {
            tData.heightmapResolution = terrainSize + 1;
            tData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
            tData.SetHeights(0, 0, GenerateHeights());
        }*/
        _collider.terrainData = tData;

        // ++ Replace Trees on Map ++ //
        GameObject[] trees = GameObject.FindGameObjectsWithTag(Utils.TAG_TREE);
        TreePositions = new Vector3[trees.Length];
        for(int i = 0; i < trees.Length; i++) {
            trees[i].transform.position = AdjustHeight(trees[i].transform.position);
            TreePositions[i] = trees[i].transform.position;
            //Debug.Log($"TreePositions[{i}]: {TreePositions[i]}");
        }
        // ++ Replace Rocks on Map ++ //
        GameObject[] rocks = GameObject.FindGameObjectsWithTag(Utils.TAG_MINE);
        RockPositions = new Vector3[rocks.Length];
        for(int i = 0; i < rocks.Length; i++) {
            rocks[i].transform.position = AdjustHeight(rocks[i].transform.position);
            RockPositions[i] = rocks[i].transform.position;
            //Debug.Log($"RockPositions[{i}]: {RockPositions[i]}");
        }
        // ++ Replace Bushes on Map ++ //
        GameObject[] bushes = GameObject.FindGameObjectsWithTag(Utils.TAG_BUSH);
        BushPositions = new Vector3[bushes.Length];
        for(int i = 0; i < bushes.Length; i++) {
            bushes[i].transform.position = AdjustHeight(bushes[i].transform.position);
            BushPositions[i] = bushes[i].transform.position;
            //Debug.Log($"BushPositions[{i}]: {BushPositions[i]}");
        }
        /*for (int i=0; i<tData.treeInstances.Length; i++) {
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
        }*/
        //Debug.Log($"Trees: {TreePositions.Length}; Rocks: {RockPositions.Length}; Bushes: {BushPositions.Length}");


        UpdateNavMesh();

        Vector3 center = new Vector3(tData.size.x, 0, tData.size.z) * 0.5f;
        return new Bounds(center, new Vector3(tData.size.x, 0, tData.size.z));
    }

    /*float[,] GenerateHeights() {
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
    }*/

    Vector3 AdjustHeight(Vector3 position) {
        Vector3 newPosition = position;
        newPosition.y = GameManager.instance.GetTerrainHeight(newPosition);
        return newPosition;
    }
}