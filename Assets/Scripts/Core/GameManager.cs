using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //properties
    public float Prosperity
    {
        get { return _prosperity; }
        set { _prosperity = value; }
    }
    float _prosperity;
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
            Debug.Log("Wood" + Wood);
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
            Debug.Log("Stone" + Stone);
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
            Debug.Log("Food" + Food);
        }
    }
    int _food;

    //privates
    float timeOfDay;
    Queue<Building> buildQueue;
    [SerializeField] Vector3 terrainSize = Vector3.one;
    [SerializeField] int startVillagerCount = 5;
    [SerializeField] float spawnRadius = 1;
    public Bounds mapBounds;
    public TerrainGenerator terrain;

    //publics
    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // TESTING
        mapBounds = new Bounds(Vector3.zero, terrainSize * 2);
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
    }

    void StartGame()
    {
        // +++ GENERATE TERRAIN +++ //
        terrain = FindObjectOfType<TerrainGenerator>();
        terrain.GenerateTerrain();

        // +++ SPAWN VILLAGERS +++ //
        string villagerJobs = "";
        for (int i = 0; i < startVillagerCount; i++)
        {
            Vector3 position = Quaternion.Euler(0, i * 360 / startVillagerCount, 0) * new Vector3(spawnRadius, 0, 0);
            Villager villager = PoolManager.instance.SpawnVillager(position);
            villager.AssignJob((Job.Type)i, true);

            villagerJobs += $"villager {i}: " + villager.job?.ToString() + "; ";
        }
        Debug.Log(villagerJobs);

        GetAllMines();
        GetAllTrees();
        GetAllBushes();

        //StartDay();
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
        int nblist = Villager.list.Count;
        for (int i = 0; i < nblist; i++)
        {
            if (!Villager.list[i].isExhausted)
            {
                StartCoroutine(Villager.list[i].GoToWork());
            }
        }
    }

    void EndDay()
    {
        int foodDeficit = Villager.list.Count - Food;
        if (foodDeficit > 0)
        {
            for (int i = 0; i < foodDeficit; i++)
            {
                Villager.list[Random.Range(0, Villager.list.Count - 1 - i)].Die();
            }
        }

        int listCount = Villager.listHasWorked.Count;
        for (int i = 0; i < listCount; i++)
        {
            Villager.listHasWorked[i].isExhausted = true;
        }

        bool gotEnoughHouses = House.nbHouses >= Villager.listHasWorked.Count;
        if (gotEnoughHouses)
        {
            for (int i = 0; i < listCount; i++)
            {
                Villager.listHasWorked[i].StopAllCoroutines();
                Villager.listHasWorked[i].GoToSleep();
            }
        }
        else
        {
            for (int i = 0; i < House.nbHouses; i++)
            {
                Villager.listHasWorked[Random.Range(0, Villager.listHasWorked.Count - 1 - i)].GoToSleep();
            }
        }
    }

    void ChangeGameSpeed(int timeScale)
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
