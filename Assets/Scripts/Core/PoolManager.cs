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
    [SerializeField] bool spawnNewVillagers = false;
    Queue<Villager> _pooledVillagers = new Queue<Villager>();
    Transform _poolVillager, _poolBuildings;

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

        InitPools();
    }
    void InitPools()
    {
        _poolVillager = new GameObject("Pool Villagers").transform; //set villager parent
        _poolVillager.SetParent(transform);
        _poolBuildings = new GameObject("Pool Buildings").transform; //set building parent
        _poolBuildings.SetParent(transform);
    }

    public void Pool(Villager villager)
    {
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
        Villager villager = UnpoolVillager(); //get the villager from the queue (or instantiate one if empty)
        villager.transform.position = GameManager.instance.GetTerrainPos(spawnPoint, villager.transform.GetChild(0).localScale); //set the new villager pos
        return villager;
    }
    public Vector3 NewVillagerPos() //Randomise the new villager pos but stay in the central square of the terrain
    {
        Bounds mapBounds = GameManager.instance.MapBounds; //get the mapBounds
        Vector3 minPos = new Vector3(mapBounds.center.x - mapBounds.size.x * 0.5f, 0, mapBounds.center.z - mapBounds.size.z * 0.5f);
        Vector3 maxPos = new Vector3(mapBounds.center.x + mapBounds.size.x * 0.5f, 0, mapBounds.center.z + mapBounds.size.z * 0.5f);
        float x = Random.Range(minPos.x, maxPos.x); //min = XmapCenter + 1/2 of size, max = XmapCenter + 1/2 of size
        float z = Random.Range(minPos.z, maxPos.z);  //min = ZmapCenter + 1/2 of size, max = ZmapCenter + 1/2 of size
        return new Vector3(x, 0, z); //return a new vector3 with x,z randomized
    }
    public Building SpawnBuilding(Building.Type buildingType)
    {
        Building prefabBuilding = null;
        switch (buildingType) //change the building type using a switch and prefabs
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
        return Instantiate(prefabBuilding, _poolBuildings); //instantiate a building with the changed building type
    }
    public House SpawnTownHall()
    {
        return Instantiate(prefabTownHall, _poolBuildings); //instantiate the TownHall, is equal to 5 houses
    }
}