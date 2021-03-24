using System.Collections;
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
        set { _prosperity = value; }
    }
    public float _prosperity, maxProsperity;
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
    int _wood;
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
    int _stone;
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
    int _food;

    //privates
    public float timeOfDay, dayLength = 20, nightLength = 1;
    //[SerializeField] Vector3 terrainSize = Vector3.one;
    [SerializeField] int startVillagerCount = 5;
    [SerializeField] float spawnRadius = 1;
    bool isDayEnding = false;

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
        /*if (Input.GetMouseButtonDown(0))
        {
            Villager villager = PoolManager.instance.UnpoolVillager();
            villager.transform.position = Vector3.zero;
            villager.AssignJob((Job.Type)Random.Range(0, 4), true);
        }*/

        timeOfDay += Time.deltaTime;
        if (!isDayEnding && timeOfDay >= dayLength)
        {
            isDayEnding = true;
            StartCoroutine(EndDay());
        }
    }

    void StartGame()
    {
        timeOfDay = 0;
        // +++ GENERATE TERRAIN +++ //
        terrain = FindObjectOfType<TerrainGenerator>();
        mapBounds = terrain.GenerateTerrain();

        // +++ SPAWN VILLAGERS +++ //
        string villagerJobs = "";
        for (int i = 0; i < startVillagerCount; i++)
        {
            Vector3 position = mapBounds.center + Quaternion.Euler(0, i * 360 / startVillagerCount, 0) * new Vector3(spawnRadius, 0, 0);
            Villager villager = PoolManager.instance.SpawnVillager(position);
            villager.AssignJob((Job.Type)i, true);

            villagerJobs += $"villager {i}: " + villager.job?.ToString() + "; ";
        }
        Debug.Log(villagerJobs);

        GetAllMines();
        GetAllTrees();
        GetAllBushes();
    }
    void GetAllMines()
    {
        Miner.mineArray = GameObject.FindGameObjectsWithTag("Mine");
        for (int i = 0; i < Miner.mineArray.Length; i++)
        {
            Miner.mineDic.Add(Miner.mineArray[i], null);
        }
    }
    void GetAllTrees()
    {
        Lumberjack.treeArray = GameObject.FindGameObjectsWithTag("Tree");
        for (int i = 0; i < Lumberjack.treeArray.Length; i++)
        {
            Lumberjack.treeDic.Add(Lumberjack.treeArray[i], null);
        }
    }
    void GetAllBushes()
    {
        Gatherer.bushArray = GameObject.FindGameObjectsWithTag("Bush");
        for (int i = 0; i < Gatherer.bushArray.Length; i++)
        {
            Gatherer.bushDic.Add(Gatherer.bushArray[i], null);
        }
    }

    void StartDay()
    {
        // start day code
        int listCount = Villager.listHasWorked.Count;
        for (int i = 0; i < listCount; i++)
        {
            Villager jango = Villager.listHasSleep[i];
            jango.isExhausted = false;
            Villager.listHasSleep.Remove(jango);
        }
        // at the end
        timeOfDay = 0;
        isDayEnding = false;
    }

    IEnumerator EndDay()
    {
        int foodDeficit = Villager.list.Count - Food;
        if (foodDeficit > 0)
        {
            for (int i = 0; i < foodDeficit; i++)
            {
                Villager.list[Random.Range(0, Villager.list.Count)].Die();
            }
        }

        int listCount = Villager.listHasWorked.Count;
        for (int i = 0; i < listCount; i++)
        {
            Villager.listHasWorked[i].isExhausted = true;
        }

        Villager.nbSleeping = Villager.nbShouldSleep = 0;
        for (int i = 0; i < House.list.Count && Villager.listHasWorked.Count > 0; i++)
        {
            Villager boba = Villager.listHasWorked[Random.Range(0, Villager.listHasWorked.Count)];
            StartCoroutine(boba.GoToSleep(House.list[i]));
            Villager.listHasWorked.Remove(boba);
            Villager.listHasSleep.Add(boba);
            Villager.nbShouldSleep++;
        }
        yield return new WaitUntil(() => Villager.nbSleeping >= Villager.nbShouldSleep);
        yield return new WaitForSeconds(nightLength);
        StartDay();
    }

    public void ChangeGameSpeed(int timeScale)
    {
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
