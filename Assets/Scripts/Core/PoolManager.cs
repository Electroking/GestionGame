using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {
    public static PoolManager instance = null;
    
    [SerializeField] Villager prefabVillager = null;
    [SerializeField] House prefabHouse = null;
    [SerializeField] School prefabSchool = null;
    [SerializeField] Farm prefabFarm = null;
    [SerializeField] Library prefabLibrary = null;
    [SerializeField] Museum prefabMuseum = null;
    Queue<Villager> _pooledVillagers = new Queue<Villager>();
    Transform _poolVillager, _poolBuildings;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        InitPools();
    }

    void InitPools() {
        _poolVillager = new GameObject("Pool Villagers").transform;
        _poolVillager.SetParent(transform);
        _poolBuildings = new GameObject("Pool Buildings").transform;
        _poolBuildings.SetParent(transform);
    }

    public void Pool(Villager villager) {
        villager.Reset();
        villager.transform.localPosition = Vector3.zero;
        villager.SetActive(true);
        _pooledVillagers.Enqueue(villager);
    }

    public Villager UnPoolVillager() {
        if (_pooledVillagers.Count > 0) {
            return _pooledVillagers.Dequeue();
        }
        return SpawnVillager();
    }

    Villager SpawnVillager() {
        return Instantiate(prefabVillager, _poolVillager);
    }

    public Building SpawnBuilding(Building.Type buildingType) {
        Building prefabBuilding = null;
        switch (buildingType) {
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