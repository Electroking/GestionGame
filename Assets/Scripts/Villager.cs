using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    public static List<Villager> list = new List<Villager>();
    public static List<Villager> listHasWorked = new List<Villager>(), listHasSlept = new List<Villager>();
    //public static int nbShouldSleep, nbSleeping;
    public int age;
    public bool isExhausted = false, hasWorked = false;
    public Job job;

    NavMeshAgent _agent;
    MeshRenderer _mr;
    Collider _coll;
    bool _isWorking = false, _isMoving = false, _isGoingToWork = false;
    Vector3 _workplace;

    void Awake()
    {
        _mr = GetComponentInChildren<MeshRenderer>();
        _coll = GetComponentInChildren<Collider>();
        _agent = GetComponent<NavMeshAgent>();

        list.Add(this);
    }
    void Start()
    {
        age = 0;
    }

    void Update()
    {
        if (!isExhausted)
        {
            GoToWork();
        } else {
            _isWorking = false;
            _isGoingToWork = false;
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //jobMenu.SetActive(true);
        }
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
        if (!_isMoving)
        {
            bool validDestination = _agent.SetDestination(targetPos);
            //Debug.Log($"validDestination: {validDestination}");
            _isMoving = true;
        }
        //if (transform.position.x == targetPos.x && transform.position.z == targetPos.z)
        if (Vector3.Distance(_agent.destination, transform.position) <= _agent.stoppingDistance) {
            if(!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) {
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
                    listHasWorked.Add(this);
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
        while (!Move(house.transform.position))
        {
            yield return null;
        }
        //nbSleeping++;
        listHasSlept.Add(this);
        Hide(true);
    }

    public void Hide(bool hide) {
        _mr.enabled = !hide;
        _coll.enabled = !hide;
        _agent.enabled = !hide;
    }

    public void Die()
    {
        list.Remove(this);
        //listHasWorked.Remove(this);
        Destroy(gameObject);
    }
}

