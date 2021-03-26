﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //statics
    public static GameManager instance = null;

    //properties
    public float Prosperity
    {
        get { return _prosperity; }
        set { _prosperity = Mathf.Clamp(value, 0, maxProsperity);
        }
    }
    public float maxProsperity = 100f;
    public int Wood
    {
        get
        {
            return _wood;
        }
        set
        {
            _wood = value;
            UIManager.instance.UpdateUI();
        }
    }
    public int Stone
    {
        get
        {
            return _stone;
        }
        set
        {
            _stone = value;
            UIManager.instance.UpdateUI();
        }
    }
    public int Food
    {
        get
        {
            return _food;
        }
        set
        {
            _food = value;
            UIManager.instance.UpdateUI();
        }
    }
    public bool IsPaused {
        get { return _isPaused; }
        set {
            _isPaused = value;
            ChangeGameSpeed(value ? 0 : _gameSpeed);
        }
    }

    //privates
    public float timeOfDay, dayLength = 20, nightLength = 1;
    //[SerializeField] Vector3 terrainSize = Vector3.one;
    [SerializeField] int startVillagerCount = 5;
    [SerializeField] float spawnRadius = 1;
    int _food, _stone, _wood;
    bool _isDayEnding = false, _isPaused = false;
    float _prosperity, _gameSpeed = 1;

    //publics
    [HideInInspector] public Bounds mapBounds;
    [HideInInspector] public TerrainGenerator terrain;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // TESTING
    }

    void Start()
    {
        StartGame();
        Wood = 10;
        Stone = 10;
    }

    void Update()
    {
        //Debug.Log(GameManager.instance.maxProsperity);
        /*if (Input.GetMouseButtonDown(0))
        {
            Villager villager = PoolManager.instance.UnpoolVillager();
            villager.transform.position = Vector3.zero;
            villager.AssignJob((Job.Type)Random.Range(0, 4), true);
        }*/

        timeOfDay += Time.deltaTime;
        if (!_isDayEnding && timeOfDay >= dayLength)
        {
            _isDayEnding = true;
            StartCoroutine(EndDay());
        }
    }

    void StartGame()
    {
        timeOfDay = 0;
        // +++ GENERATE TERRAIN +++ //
        terrain = FindObjectOfType<TerrainGenerator>();
        mapBounds = terrain.GenerateTerrain();
        // bind environment objects to their respective job
        Lumberjack.treeArray = terrain.TreePositions;
        Miner.mineArray = terrain.RockPositions;
        Gatherer.bushArray = terrain.BushPositions;

        // +++ SPAWN VILLAGERS +++ //
        string villagerJobs = "";
        for (int i = 0; i < startVillagerCount; i++)
        {
            Vector3 position = mapBounds.center + Quaternion.Euler(0, i * 360 / startVillagerCount, 0) * new Vector3(spawnRadius, 0, 0);
            Villager villager = PoolManager.instance.SpawnVillager(position);
            //Debug.Log(villager.transform.position);
            villager.AssignJob((Job.Type)i, true);

            villagerJobs += $"villager {i}: " + villager.job?.ToString() + "; ";
        }
        //Debug.Log(villagerJobs);

        // +++ MISC +++ //
        PlayerCamera pCam = FindObjectOfType<PlayerCamera>();
        pCam.transform.position = mapBounds.center;
    }

    void StartDay()
    {
    }

    IEnumerator EndDay()
    {
        /*int foodDeficit = Villager.list.Count - Food;
        if (foodDeficit > 0)
        {
            for (int i = 0; i < foodDeficit; i++)
            {
                Villager.list[Random.Range(0, Villager.list.Count)].Die();
            }
        }*/

        /*/ All villagers who have worked during the day are exhausted
        for (int i = 0; i < Villager.listHasWorked.Count; i++)
        {
            Villager.listHasWorked[i].isExhausted = true;
        }
        // All exhausted villagers go to sleep
        Villager.nbSleeping = Villager.nbShouldSleep = 0;
        Villager.listHasSlept.Clear();
        for (int i = 0; i < House.list.Count && Villager.listHasWorked.Count > 0; i++)
        {
            Villager boba = Villager.listHasWorked[Random.Range(0, Villager.listHasWorked.Count)];
            StartCoroutine(boba.GoToSleep(House.list[i]));
            Villager.listHasWorked.Remove(boba);
            Villager.listHasSlept.Add(boba);
            Villager.nbShouldSleep++;
        }
        // wait for all exhausted villagers to reach a house (if possible)
        yield return new WaitUntil(() => Villager.nbSleeping >= Villager.nbShouldSleep);
        yield return new WaitForSeconds(nightLength);

        // All villagers who slept arent exhausted anymore
        for(int i = 0; i < Villager.listHasSlept.Count; i++) {
            Villager jango = Villager.listHasSlept[i];
            jango.isExhausted = false;
            //Villager.listHasSlept.Remove(jango);
        }
        // at the end
        timeOfDay = 0;
        isDayEnding = false;*/


        List<Villager> villagers = new List<Villager>(Villager.list);
        villagers.Sort((v1, v2) => Random.Range(-1, 2));
        for(int i = 0; i < villagers.Count; i++) {
            if(Food > 0) {
                Food--;
            } else {
                villagers[i].Die();
            }
        }
        // Get the list of all villagers who have worked during the day and exhaust them
        List<Villager> villagersWhoWorked = new List<Villager>();
        List<Villager> villagersExhausted = new List<Villager>();
        for(int i = 0; i < Villager.list.Count; i++) {
            if(Villager.list[i].hasWorked) {
                Villager.list[i].isExhausted = true;
                villagersWhoWorked.Add(Villager.list[i]);
            }
            if(Villager.list[i].isExhausted) {
                villagersExhausted.Add(Villager.list[i]);
            }
        }

        // Exhausted villagers try to go to sleep (if enough Houses available)
        villagersExhausted.Sort((v1, v2) => Random.Range(-1, 2)); // randomize villager order
        Villager.listHasSlept.Clear();
        List<Villager> villagersToBed = new List<Villager>();

        //Debug.Log("nbHouses = " + House.list.Count + "; nbExhausted = " + villagersExhausted.Count);

        for (int i = 0; i < villagersExhausted.Count && i < House.list.Count; i++) {
            Villager boba = villagersExhausted[i];
            villagersToBed.Add(boba);
            StartCoroutine(boba.GoToSleep(House.list[i]));
        }

        // wait for all exhausted villagers to reach a house (if possible)
        yield return new WaitUntil(() => Villager.listHasSlept.Count >= villagersToBed.Count);
        yield return new WaitForSeconds(nightLength);
        //Debug.Log("After the night !");
        // All villagers who slept arent exhausted anymore
        for(int i = 0; i < Villager.listHasSlept.Count; i++) {
            Villager.listHasSlept[i].Hide(false);
            Villager.listHasSlept[i].isExhausted = false;
        }
        // at the end
        timeOfDay = 0;
        _isDayEnding = false;
    }

    public void PauseGame(bool pause) {
        IsPaused = pause;
    }

    public void ChangeGameSpeed(float timeScale)
    {
        if(timeScale > 0) _gameSpeed = timeScale;
        Time.timeScale = timeScale;
    }

    public Vector3 GetTerrainPos(Vector3 position) => GetTerrainPos(position, Vector3.zero);
    public Vector3 GetTerrainPos(Vector3 position, Vector3 objectSize)
    {
        Vector3 terrainPos = GetBoundedPos(position, objectSize);
        terrainPos.y = GetTerrainHeight(terrainPos);
        return terrainPos;
    }

    public Vector3 GetBoundedPos(Vector3 position, Vector3 objectSize)
    {
        Bounds adaptedBounds = mapBounds;
        adaptedBounds.extents -= objectSize * 0.5f;
        return adaptedBounds.ClosestPoint(position);
        /*Vector3 relativePos = position - plane.transform.position;
        Vector3 basePlaneSize = plane.transform.localScale * 5;
        if (relativePos.x - objectSize.x < basePlaneSize.x && relativePos.z - objectSize.z < basePlaneSize.z
            && relativePos.x + objectSize.x > -basePlaneSize.x && relativePos.z + objectSize.z > -basePlaneSize.z) {
            return position;
        }
        Vector3 diffNeg = basePlaneSize + relativePos;
        Vector3 diffPos = basePlaneSize - relativePos;
        if (diffNeg.x >= diffPos)*/
    }

    public float GetTerrainHeight(Vector3 position)
    {
        Vector3 rayOrigin = position;
        rayOrigin.y = 100;
        if (Physics.Raycast(rayOrigin, -Vector3.up, out RaycastHit hit, 250, 1 << LayerMask.NameToLayer(Utils.LAYER_TERRAIN)))
        {
            return hit.point.y;
        }
        return 0;
    }
}
