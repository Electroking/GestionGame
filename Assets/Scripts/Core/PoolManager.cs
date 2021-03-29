using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null;

    public Villager prefabVillager = null;
    public House prefabHouse = null;
    public School prefabSchool = null;
    public Farm prefabFarm = null;
    public Library prefabLibrary = null;
    public Museum prefabMuseum = null;
    public House prefabTownHall;

    NavMeshSurface navMeshSurface = null;
    [SerializeField] bool spawnNewVillagers = false;
    Queue<Villager> _pooledVillagers = new Queue<Villager>();
    Transform _poolVillager, _poolBuildings;
    float time=20, reload;

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
        //DontDestroyOnLoad(gameObject);

        InitPools();
    }
    private void Start()
    {
        navMeshSurface = FindObjectOfType<NavMeshSurface>();
        reload = GameManager.instance.dayLength / 10f;
    }
    private void Update()
    {
        /*if(!spawnNewVillagers) return;
        if( Time.time > time)
        {
            SpawnVillager(NewVillagerPos());
            time = Time.time + reload;
        }*/
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

    public Villager SpawnVillagerAtRandomPoint() => SpawnVillager(NewVillagerPos());
    public Villager SpawnVillager(Vector3 spawnPoint)
    {
        Villager villager = UnpoolVillager();
        villager.transform.position = GameManager.instance.GetTerrainPos(spawnPoint, villager.transform.GetChild(0).localScale);
        return villager;
    }
    public Vector3 NewVillagerPos()
    {
        Bounds mapBounds = GameManager.instance.MapBounds;
        //Vector3 pos = navMeshSurface.navMeshData.position, size = navMeshSurface.navMeshData.sourceBounds.size;
        Vector3 minPos = new Vector3(mapBounds.center.x - mapBounds.size.x * 0.5f, 0, mapBounds.center.z - mapBounds.size.z * 0.5f);
        Vector3 maxPos = new Vector3(mapBounds.center.x + mapBounds.size.x * 0.5f, 0, mapBounds.center.z + mapBounds.size.z * 0.5f);
        float x = Random.Range(minPos.x, maxPos.x); //min = x + un tier de la taille, max = x + (size - un tier de la size)
        float z = Random.Range(minPos.z, maxPos.z);  //min = z + un tier de la taille, max = z + (size - un tier de la size)
        //Vector3 zxPos = new Vector3(x,0,z);
        //Vector3 finalPos = new Vector3(x, GameManager.instance.GetTerrainHeight(zxPos), z);
        return new Vector3(x, 0, z);
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
    public House SpawnTownHall()
    {
        return Instantiate(prefabTownHall, _poolBuildings);
    }
}