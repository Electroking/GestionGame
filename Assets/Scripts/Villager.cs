using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    public static List<Villager> list = new List<Villager>();
    public static List<Villager> listHasSlept = new List<Villager>();
    public static List<string> usedNames = new List<string>();
    public static int lifetime = 10;

    public int Age
    {
        get { return _age; }
        set
        {
            _age = value;
            if (_age > lifetime)
            {
                Die();
            }
        }
    }
    public bool isExhausted = false, hasWorked = false;
    public Job job;

    NavMeshAgent _agent;
    MeshRenderer _mr;
    Collider _coll;
    GameObject _spriteCircle;
    int _age;
    bool _isWorking = false, _isMoving = false, _isGoingToWork = false;
    bool _selected = false, _justStoppedWorking = false;
    Vector3 _workplace;

    void Awake()
    {
        _mr = GetComponentInChildren<MeshRenderer>();
        _coll = GetComponentInChildren<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _spriteCircle = transform.GetChild(1).gameObject;

        list.Add(this);
    }

    void Update()
    {
        if (GameManager.instance.IsPaused) return;
        if (!isExhausted && !GameManager.instance.isDayEnding)
        {
            GoToWork();
            _justStoppedWorking = true;
        }
        else if (_justStoppedWorking)
        {
            // If villager has a destination, get rid of it and stop moving.
            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
            _isWorking = false;
            _isGoingToWork = false;
            _justStoppedWorking = false;
        }
    }

    public void FindName()
    {
        string name;
        int nbLoop = 0;
        do
        {
            name = Utils.GetRandomName();
            nbLoop++;
        } while (usedNames.Contains(name) && nbLoop < 200); // If no name is available, escape the loop to avoid infinite loop
        if (nbLoop < 200)
        {
            usedNames.Add(name);
        }
        else
        {
            name = "Anne Honimousse";
        }
        this.name = name;
    }

    public void JobSwitch(int jobType)
    {
        AssignJob(Job.Type.Student);
        ((Student)job).jobToLearn = (Job.Type)jobType;
    }

    public void AssignJob(Job.Type jobType, bool trueJobsOnly = false)
    {
        job = Job.GetNewJob(this, jobType, trueJobsOnly);
    }
    /// <summary>
    /// Set the destination and check if arrived.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns><b>True</b> when arrived.</returns>
    public bool Move(Vector3 targetPos)
    {
        if (!_isMoving || _agent.destination != targetPos)
        {
            _agent.SetDestination(targetPos);
            _isMoving = true;
        }
        if (Vector3.Distance(_agent.destination, transform.position) <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                _isMoving = false;
                return true;
            }
        }
        return false;
    }

    public void GoToWork()
    {
        if (job == null) return;
        if (!_isWorking)
        {
            if (!_isGoingToWork)
            {
                //The villager needs a new workplace
                if (job.GetWorkplacePos(out _workplace))
                {
                    _isGoingToWork = true;
                }
            }
            if (_isGoingToWork)
            {
                //The villager moves to his workplace
                if (Move(_workplace))
                {
                    _isGoingToWork = false;
                    _isWorking = true;
                }
            }
        }
        if (_isWorking)
        {
            //The villager works
            if (job.DoTheWork())
            {
                _isWorking = false;
            }
            hasWorked = true;
        }
    }

    public IEnumerator GoToSleep(House house)
    {
        if (job != null) job.OnGoToSleep();
        while (!Move(house.transform.position))
        {
            yield return null;
        }
        listHasSlept.Add(this);
        Hide(true);
    }

    public void Hide(bool hide)
    {
        _mr.enabled = !hide;
        _coll.enabled = !hide;
        _agent.enabled = !hide;
        _spriteCircle.SetActive(hide ? false : _selected);
    }

    public void OnSelect(bool selected)
    {
        _spriteCircle.SetActive(_selected = selected);
    }

    public void Die()
    {
        if (UIManager.instance.uiVillager.villager == this)
        {
            UIManager.instance.uiVillager.ClosePanel();
        }
        if (job != null)
        {
            job.OnDie();
        }
        list.Remove(this);
        GameManager.instance.Prosperity -= 0.05f * GameManager.instance.maxProsperity;
        PoolManager.instance.Pool(this);
    }
}

