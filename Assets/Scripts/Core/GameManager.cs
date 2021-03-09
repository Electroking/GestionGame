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

    //privates
    float timeOfDay;
    int food;
    Queue<Building> buildQueue;
    [SerializeField] int startVillagerCount = 5;
    [SerializeField] float spawnRadius = 1;

    //publics
    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartGame();
    }


    void StartGame()
    {
        for (int i = 0; i < startVillagerCount; i++)
        {
            Villager villager = PoolManager.instance.UnpoolVillager();
            villager.transform.position = Quaternion.Euler(0, i * 360 / startVillagerCount, 0) * new Vector3(spawnRadius, 0, 0);
            villager.AssignJob((Job.Type)i, true);
        }
    }

    void StartDay()
    {

    }

    void EndDay() {
        int foodDeficit = Villager.list.Count - food;
        if(foodDeficit > 0) {
            for(int i = 0; i < foodDeficit; i++) {
                Villager.list[Random.Range(0, Villager.list.Count - 1 - i)].Die();
            }
        }

        bool gotEnoughHouses = House.nbHouses >= Villager.list.Count;
        if(gotEnoughHouses) {
            int nblist = Villager.list.Count;
            for(int i = 0; i < nblist; i++) {
                Villager.list[i].GoToSleep();
            }
        } else {
            for(int i = 0; i < House.nbHouses; i++) {
                Villager.list[Random.Range(0, Villager.list.Count - 1 - i)].GoToSleep();
            }
        }
    }

    void ChangeGameSpeed(int timeScale) {
        Time.timeScale = timeScale;
    }
}
