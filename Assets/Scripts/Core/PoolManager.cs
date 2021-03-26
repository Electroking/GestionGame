using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null;

    [SerializeField] Villager prefabVillager = null;
    [SerializeField] House prefabHouse = null;
    [SerializeField] School prefabSchool = null;
    [SerializeField] Farm prefabFarm = null;
    [SerializeField] Library prefabLibrary = null;
    [SerializeField] Museum prefabMuseum = null;
    Queue<Villager> _pooledVillagers = new Queue<Villager>();
    Transform _poolVillager, _poolBuildings;
    [SerializeField] NavMeshSurface navMeshSurface = null;
    float time, reload;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        InitPools();
    }
    private void Start()
    {
        navMeshSurface = FindObjectOfType<NavMeshSurface>();
        reload = GameManager.instance.dayLength / 10f;
    }
    private void Update()
    {
        if( Time.time > time)
        {
            SpawnVillager(NewVillagerPos());
            time = Time.time + reload;
        }
    }

    void InitPools()
    {
        _poolVillager = new GameObject("Pool Villagers").transform;
        _poolVillager.SetParent(transform);
        _poolBuildings = new GameObject("Pool Buildings").transform;
        _poolBuildings.SetParent(transform);
    }

    public void Pool(Villager villager)
    {
        //villager.Reset();
        villager.transform.localPosition = Vector3.zero;
        villager.gameObject.SetActive(true);
        _pooledVillagers.Enqueue(villager);
    }

    public Villager UnpoolVillager()
    {
        if (_pooledVillagers.Count > 0)
        {
            return _pooledVillagers.Dequeue();
        }
        return Instantiate(prefabVillager, _poolVillager);
    }

    public Villager SpawnVillager(Vector3 spawnPoint)
    {
        Villager villager = UnpoolVillager();
        villager.transform.position = GameManager.instance.GetTerrainPos(spawnPoint);
        return villager;
    }
    public Vector3 NewVillagerPos()
    {
        Vector3 pos = navMeshSurface.navMeshData.position, size = navMeshSurface.navMeshData.sourceBounds.size;
        float x = Random.Range(pos.x + size.x / 3, pos.x + (size.x - size.x / 3)); //min = x + un tier de la taille, max = x + (size - un tier de la size)
        float z = Random.Range(pos.z + size.z / 3, pos.z + (size.z - size.z / 3));  //min = z + un tier de la taille, max = z + (size - un tier de la size)
        Vector3 zxPos = new Vector3(x,0,z);
        Vector3 finalPos = new Vector3(x, GameManager.instance.GetTerrainHeight(zxPos), z);
        return finalPos;
    }
    public Building SpawnBuilding(Building.Type buildingType)
    {
        Building prefabBuilding = null;
        switch (buildingType)
        {
            case Building.Type.House:
                prefabBuilding = prefabHouse;
                break;
            case Building.Type.School:
                prefabBuilding = prefabSchool;
                break;
            case Building.Type.Farm:
                prefabBuilding = prefabFarm;
                break;
            case Building.Type.Library:
                prefabBuilding = prefabLibrary;
                break;
            case Building.Type.Museum:
                prefabBuilding = prefabMuseum;
                break;
            default:
                break;
        }
        return Instantiate(prefabBuilding, _poolBuildings);
    }
}