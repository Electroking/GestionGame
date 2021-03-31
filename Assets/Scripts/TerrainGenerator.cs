using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour {
    public Vector3[] TreePositions { get; private set; } //a board with all the trees pos
    public Vector3[] RockPositions { get; private set; } //a board with all the rocks pos
    public Vector3[] BushPositions { get; private set; } //a board with all the bushes pos

    [SerializeField] LineRenderer lr = null;
    Terrain _terrain;
    TerrainCollider _collider;
    NavMeshSurface _surface;

    private void Awake() {
        _terrain = GetComponent<Terrain>(); //get the terrain
        _collider = GetComponent<TerrainCollider>();//get the terrain collider
        _surface = GetComponentInParent<NavMeshSurface>(); //get the navmesh surface
    }

    private void Update() {
        /*if (regenerateMap) {
            GenerateTerrain();
            regenerateMap = false;
        }*/
    }

    public void UpdateNavMesh() {
        _surface.BuildNavMesh(); //build surface
    }

    public Bounds GenerateTerrain(bool pseudoRandom = false) {
        // generate base terrain
        TerrainData tData = _terrain.terrainData;
        _collider.terrainData = tData;

        // ++ Replace Trees on Map ++ //
        GameObject[] trees = GameObject.FindGameObjectsWithTag(Utils.TAG_TREE);
        TreePositions = new Vector3[trees.Length];
        for(int i = 0; i < trees.Length; i++) {
            trees[i].transform.position = AdjustHeight(trees[i].transform.position);
            TreePositions[i] = trees[i].transform.position;
        }
        // ++ Replace Rocks on Map ++ //
        GameObject[] rocks = GameObject.FindGameObjectsWithTag(Utils.TAG_MINE);
        RockPositions = new Vector3[rocks.Length];
        for(int i = 0; i < rocks.Length; i++) {
            rocks[i].transform.position = AdjustHeight(rocks[i].transform.position);
            RockPositions[i] = rocks[i].transform.position;
        }
        // ++ Replace Bushes on Map ++ //
        GameObject[] bushes = GameObject.FindGameObjectsWithTag(Utils.TAG_BUSH);
        BushPositions = new Vector3[bushes.Length];
        for(int i = 0; i < bushes.Length; i++) {
            bushes[i].transform.position = AdjustHeight(bushes[i].transform.position);
            BushPositions[i] = bushes[i].transform.position;
        }
        Bounds mapBounds = new Bounds(new Vector3(tData.size.x, 0, tData.size.z) * 0.5f, new Vector3(tData.size.x, 0, tData.size.z) / 3f); ; //set the bounds of the map
        DrawSquareLine(mapBounds);
        UpdateNavMesh();

        return mapBounds;
    }

    Vector3 AdjustHeight(Vector3 position) { 
        Vector3 newPosition = position;
        newPosition.y = GameManager.instance.GetTerrainHeight(newPosition); //change the y pos of the vector3 getting the terrain height
        return newPosition;
    }

    public void DrawSquareLine(Bounds mapBounds)
    {
        float y = 2f;
        lr.transform.position = mapBounds.center; //set the line renderer pos to the center of the map
        Vector3 a = mapBounds.center + new Vector3(-mapBounds.size.x * 0.5f, y, -mapBounds.size.z * 0.5f); //define all vector3 for each point of the line 
        Vector3 b = mapBounds.center + new Vector3(-mapBounds.size.x * 0.5f, y, mapBounds.size.z * 0.5f);
        Vector3 c = mapBounds.center + new Vector3(mapBounds.size.x * 0.5f, y, mapBounds.size.z * 0.5f);
        Vector3 d = mapBounds.center + new Vector3(mapBounds.size.x * 0.5f, y, -mapBounds.size.z * 0.5f);
        lr.loop = true;
        lr.SetPositions(new Vector3[] {a, b, c, d}); //set all vector3 that we defined before in the line renderer point list
    }
}