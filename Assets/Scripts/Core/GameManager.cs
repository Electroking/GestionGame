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
            //UIManager.instance.UpdateUI();
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
            //UIManager.instance.UpdateUI();
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
        for (int i = 0; i <= 4; i++)
        {
            //Villager villager = PoolManager.instance.UnpoolVillager();
            //villager.transform.position = Quaternion.Euler(0, i * 360 / startVillagerCount, 0) * new Vector3(spawnRadius, 0, 0);
            //villager.AssignJob((Job.Type)i);
        }
    }

    void StartDay()
    {

    }

    void EndDay()
    {
        /*int foodDeficit = Villager.villagers.count - food;
        if (foodDeficit > 0) 
        {
            for (int i = 0; i < foodDeficit; i++)
            {
                Villager.villagers[Random.Range(0, Villager.villagers.count - 1 - i)].Die();
            }
        }
        */

        /*bool gotEnoughHouses = House.nbHouses >= Villager.villagers.count;
        if (gotEnoughHouses) 
        {
            int nbVillagers = Villager.villagers.count;
            for (int i = 0; i < nbVillagers; i++)
            {
                Villager.villagers[i].GoToSleep();
            }
        }
        else
        {
            for (int i = 0; i < House.nbHouses; i++)
            {
                Villager.villagers[Random.Range(0, Villager.villagers.count - 1 - i)]).GoToSleep();
            }
        }
        */
    }

    void ChangeGameSpeed(int timeScale)
    {
        Time.timeScale = timeScale;
    }
}
public class Building
{

}
