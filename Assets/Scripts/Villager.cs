﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    public static List<Villager> list = new List<Villager>();
    public static List<Villager> listHasWorked = new List<Villager>(), listHasSlept = new List<Villager>();
    public static int lifetime = 10;
    public static List<string> usedNames = new List<string>();

    //public static int nbShouldSleep, nbSleeping;
    public int Age
    {
        get { return _age; }
        set
        {
            _age = value;
            if (_age > lifetime)
            {
                Debug.Log($"{name} died of old age at {_age - 1} days old.");
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
    bool _selected = false, _booly = false;
    Vector3 _workplace;

    void Awake()
    {
        _mr = GetComponentInChildren<MeshRenderer>();
        _coll = GetComponentInChildren<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _spriteCircle = transform.GetChild(1).gameObject;

        list.Add(this);
    }
    void Start()
    {
        name = FindName();
        Age = 0;
    }

    void Update()
    {
        if (GameManager.instance.IsPaused) return;
        if (!isExhausted && !GameManager.instance.isDayEnding)
        {
            GoToWork();
            _booly = false;
        }
        else if (!_booly)
        {
            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
            _isWorking = false;
            _isGoingToWork = false;
            _booly = true;
        }
    }

    string FindName()
    {
        string name;
        int nbLoop = 0;
        do
        {
            name = Utils.GetRandomName();
            nbLoop++;
        } while (usedNames.Contains(name) && nbLoop < 200);
        if (nbLoop < 200)
        {
            usedNames.Add(name);
        }
        else
        {
            name = "Anne Honimousse";
        }
        return name;
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

    public bool Move(Vector3 targetPos)
    {
        if (!_isMoving || _agent.destination != targetPos)
        {
            bool validDestination = _agent.SetDestination(targetPos);
            //Debug.Log($"validDestination: {validDestination}");
            _isMoving = true;
        }
        //if (transform.position.x == targetPos.x && transform.position.z == targetPos.z)
        if (Vector3.Distance(_agent.destination, transform.position) <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                //Debug.Log("==> Arrived at destination.");
                _isMoving = false;
                return true;
            }
        }
        return false;
        /*
        Vector3 dir = targetPos - transform.position; // direction = player direction - ennemy direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; //define angle with both axis multiplied and convert Radiant to Degres
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); //set rotate with angle and forward vector
        float distance = Vector3.Distance(transform.position, targetPos);
        transform.Translate(Vector3.right * Time.deltaTime); //moving right by time
        if (distance < 0.05)
        {
            transform.position = targetPos; //teleport ennemy to waypoint position when the distance < 0.05
            return true;
        }
        return false;
        */
        /*transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3 / Vector3.Distance(transform.position, targetPos));
        return transform.position == targetPos;*/
    }

    public void GoToWork()
    {
        if (job == null) return;
        if (!_isWorking)
        {
            if (!_isGoingToWork)
            {
                if (job.GetWorkplacePos(out _workplace))
                {
                    _isGoingToWork = true;
                }
            }
            else
            {
                if (Move(_workplace))
                {
                    _isGoingToWork = false;
                    _isWorking = true;
                    // listHasWorked.Add(this); // TODO: memory leak (Work in progress ?)
                }
            }
        }
        else
        {
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
        //nbSleeping++;
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
        if (job != null)
        {
            job.OnDie();
        }
        list.Remove(this);
        GameManager.instance.Prosperity -= 0.05f * GameManager.instance.maxProsperity;
        //listHasWorked.Remove(this);
        Destroy(gameObject);
    }
}

