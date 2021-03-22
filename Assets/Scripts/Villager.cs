using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    public static List<Villager> list = new List<Villager>();
    public static List<Villager> listHasWorked = new List<Villager>();
    public int age;
    public bool isExhausted = false;
    public Job job;

    NavMeshAgent _agent;
    bool _isWorking = false, _isMoving = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        list.Add(this);
    }
    void Start()
    {
        age = 0;
        StartCoroutine(GoToWork());
    }

    void Update()
    {
        if (!_isWorking && !isExhausted) {
            StartCoroutine(GoToWork());
        }
    }

    public void AssignJob(Job.Type jobType, bool trueJobsOnly = false)
    {
        job = Job.GetNewJob(jobType, trueJobsOnly);
        if (job != null) job.villager = this;
    }

    public bool Move(Vector3 targetPos)
    {
        if (!_isMoving) {
            bool validDestination = _agent.SetDestination(targetPos);
            Debug.Log($"validDestination: {validDestination}");
            _isMoving = true;
        }
        if (transform.position.x == targetPos.x && transform.position.z == targetPos.z) {
            Debug.Log("==> Arrived at destination.");
            _isMoving = false;
            return true;
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

    public IEnumerator GoToWork()
    {
        if (job == null) yield break;
        _isWorking = true;
        Vector3 workplace;
        while ((workplace = job.GetWorkplacePos()) == Vector3.zero)
        {
            yield return null;
        }
        Debug.Log(workplace);
        while (!Move(workplace))
        {
            yield return null;
        }
        listHasWorked.Add(this);
        yield return job.DoTheWork();
        _isWorking = false;
    }

    public void GoToSleep()
    {
        House newHouse = FindObjectOfType<House>();
        if (newHouse.inhabitant == null)
        {
            if (transform.position != newHouse.transform.position)
            {
                Move(newHouse.transform.position);
                gameObject.SetActive(false);
                isExhausted = false;
            }
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}

