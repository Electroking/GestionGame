using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public static List<Villager> list = new List<Villager>();
    public static List<Villager> listHasWorked = new List<Villager>();
    public int age;
    public bool isExhausted = false;
    public Job job;

    void Awake()
    {
        list.Add(this);
    }
    void Start()
    {
        age = 0;
        StartCoroutine(GoToWork());
    }

    void Update()
    {

    }

    public void AssignJob(Job.Type jobType, bool trueJobsOnly = false)
    {
        job = Job.GetNewJob(jobType, trueJobsOnly);
        if (job != null) job.villager = this;
    }

    public bool Move(Vector3 targetPos)
    {
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
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3 / Vector3.Distance(transform.position, targetPos));
        return transform.position == targetPos;
    }

    public IEnumerator GoToWork()
    {
        if (job == null) yield break;
        while (job.GetWorkplacePos() == Vector3.zero)
        {
            yield return null;
        }
        Vector3 workplace = job.GetWorkplacePos();
        Debug.Log(workplace);
        while (!Move(workplace))
        {
            yield return null;
        }
        listHasWorked.Add(this);
        StartCoroutine(job.DoTheWork());
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

