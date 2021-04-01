using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public float Prosperity
    {
        get { return _prosperity; }
        set
        {
            _prosperity = Mathf.Clamp(value, 0, maxProsperity);
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
            UIManager.instance.UpdateResources();
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
            UIManager.instance.UpdateResources();
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
            UIManager.instance.UpdateResources();
        }
    }
    public bool IsPaused
    {
        get { return _isPaused; }
        set
        {
            _isPaused = value;
            ChangeGameSpeed(value ? 0 : _gameSpeed);
            UIManager.instance.UpdatePlayPause();
        }
    }
    public Bounds MapBounds
    {
        get; private set;
    }

    [HideInInspector] public TerrainGenerator terrain;
    public float timeOfDay, dayLength = 20, nightLength = 1;
    public bool isDayEnding = false;

    [SerializeField] int startVillagerCount = 5;
    [SerializeField] float spawnRadius = 1;
    int _food, _stone, _wood;
    bool _isPaused = false;
    float _prosperity, _gameSpeed = 1;


    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (GameOver()) return;
        if (Victory()) return;

        timeOfDay += Time.deltaTime;
        if (!isDayEnding && timeOfDay >= dayLength)
        {
            isDayEnding = true;
            StartCoroutine(EndDay());
        }
    }

    void ResetAllStatics()
    {
        Building.unbuiltList = new List<Building>();
        Farm.list = new List<Farm>();
        House.list = new List<House>();
        School.list = new List<School>();
        Builder.idleList = new List<Villager>();
        Villager.list = new List<Villager>();
        Villager.listHasSlept = new List<Villager>();
        Villager.usedNames = new List<string>();
    }
    void StartGame()
    {
        IsPaused = false; // because Timescale is set to 0 when winning or losing a game.
        ResetAllStatics();
        timeOfDay = 0;

        // +++ GENERATE TERRAIN +++ //
        terrain = FindObjectOfType<TerrainGenerator>();
        MapBounds = terrain.GenerateTerrain();

        // bind environment objects to their respective job
        Lumberjack.treeArray = terrain.TreePositions;
        Miner.mineArray = terrain.RockPositions;
        Gatherer.bushArray = terrain.BushPositions;

        // +++ SPAWN BUILDINGS +++ //
        House townHall = PoolManager.instance.SpawnTownHall();
        townHall.transform.position = GetTerrainPos(MapBounds.center);
        townHall.Place(true);
        townHall.BuildInstant();

        // +++ SPAWN VILLAGERS +++ //
        for (int i = 0; i < startVillagerCount; i++)
        {
            Vector3 position = MapBounds.center + Quaternion.Euler(0, i * 360 / startVillagerCount, 0) * new Vector3(spawnRadius, 0, 0);
            Villager villager = PoolManager.instance.SpawnVillager(position);
            villager.AssignJob((Job.Type)i, true);
        }

        // +++ MISC +++ //
        PlayerCamera pCam = FindObjectOfType<PlayerCamera>();
        pCam.transform.position = MapBounds.center;
        Wood = 10;
        Stone = 10;
        Food = 10;
    }

    IEnumerator EndDay()
    {
        OnDayEnds();

        // Get the list of villagers, update their age and kill them if not enough food
        List<Villager> villagers = new List<Villager>(Villager.list);
        villagers.Sort((v1, v2) => Random.Range(-1, 2)); // Randomize the villager list
        for (int i = 0; i < villagers.Count; i++)
        {
            villagers[i].Age++;
            if (villagers[i] == null) continue; // if died of old age, don't feed them
            if (Food > 0)
            {
                Food--;
            }
            else
            {
                villagers[i].Die();
            }
        }

        // Get the list of all villagers who have worked during the day and exhaust them
        List<Villager> villagersExhausted = new List<Villager>();
        for (int i = 0; i < Villager.list.Count; i++)
        {
            if (Villager.list[i].hasWorked)
            {
                Villager.list[i].isExhausted = true;
                Villager.list[i].hasWorked = false;
            }
            if (Villager.list[i].isExhausted)
            {
                villagersExhausted.Add(Villager.list[i]);
            }
        }

        // Exhausted villagers try to go to sleep (if enough Houses available)
        villagersExhausted.Sort((v1, v2) => Random.Range(-1, 2)); // randomize villager order
        Villager.listHasSlept.Clear();
        List<Villager> villagersToBed = new List<Villager>();
        Villager boba;
        for (int i = 0; i < House.list.Count; i++)
        {
            House.list[i].inhabitants.Clear();
        }

        for (int i = 0; i < villagersExhausted.Count; i++)
        {
            boba = villagersExhausted[i];
            for (int j = 0; j < House.list.Count; j++)
            {
                if (House.list[j].inhabitants.Count < House.list[j].maxInhabitant)
                {
                    villagersToBed.Add(boba);
                    House.list[j].inhabitants.Add(boba);
                    StartCoroutine(boba.GoToSleep(House.list[j]));
                    break;
                }
            }
        }

        // wait for all exhausted villagers to reach a house (if possible)
        yield return new WaitUntil(() =>
        {
            return Villager.listHasSlept.Count >= villagersToBed.Count;
        });
        yield return new WaitForSeconds(nightLength);

        // All villagers who slept arent exhausted anymore
        Villager jango;
        for (int i = 0; i < Villager.listHasSlept.Count; i++)
        {
            jango = Villager.listHasSlept[i];
            jango.Hide(false);
            jango.isExhausted = false;
        }

        for (int i = 0; i < villagersExhausted.Count; i++)
        {
            if (villagersExhausted[i].isExhausted)
            {
                Prosperity -= 0.005f * maxProsperity;
            }
        }

        OnDayStarts();

        // at the end
        timeOfDay = 0;
        isDayEnding = false;
    }

    void OnDayEnds()
    {
        UIManager.instance.uiVillager.LockJobChange(true); // disable jobChange
    }

    void OnDayStarts()
    {
        PoolManager.instance.SpawnVillagerAtRandomPoint(); // spawn new Villager
        UIManager.instance.uiVillager.LockJobChange(false); // enable jobChange
    }

    public bool Victory()
    {
        if (Prosperity >= maxProsperity)
        {
            UIManager.instance.ShowVictoryPanel();
            IsPaused = true;
            return true;
        }
        return false;
    }

    public bool GameOver()
    {
        if (Villager.list.Count == 0)
        {
            UIManager.instance.ShowGameOverPanel();
            IsPaused = true;
            return true;
        }
        return false;
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }

    /// <summary>
    /// Set the game speed and change the timescale if the game is unpaused.
    /// </summary>
    /// <param name="gameSpeed"></param>
    public void ChangeGameSpeed(float gameSpeed)
    {
        if (gameSpeed > 0) _gameSpeed = gameSpeed;
        if (IsPaused)
        {
            if (gameSpeed == 0) Time.timeScale = gameSpeed;
            return;
        }
        Time.timeScale = gameSpeed;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public Vector3 GetTerrainPos(Vector3 position) => GetTerrainPos(position, Vector3.zero);
    /// <summary>
    /// Get a correct position bounded inside the game bounds and at the right height.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="objectSize"></param>
    /// <returns>The adjusted position.</returns>
    public Vector3 GetTerrainPos(Vector3 position, Vector3 objectSize)
    {
        Vector3 terrainPos = GetBoundedPos(position, objectSize);
        terrainPos.y = GetTerrainHeight(terrainPos);
        return terrainPos;
    }

    public Vector3 GetBoundedPos(Vector3 position, Vector3 objectSize)
    {
        Bounds adaptedBounds = new Bounds(MapBounds.center, MapBounds.size);
        adaptedBounds.extents -= objectSize * 0.5f;
        return adaptedBounds.ClosestPoint(position);
    }

    /// <returns>The terrain height at position</returns>
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
