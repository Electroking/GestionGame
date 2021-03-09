using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public static List<Villager> list;
    public int age;
    public bool isExhausted = false, isHungry = false;
    public Job job;

    void Start()
    {
        age = 0;
    }

    void Update()
    {
        
    }

    public void AssignJob(Job.Type jobType, bool trueJobsOnly = false) {
        job = Job.GetNewJob(jobType, trueJobsOnly);
        Debug.Log("new job: " + job?.ToString());
    }

    public bool Move(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position; // direction = player direction - ennemy direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; //define angle with both axis multiplied and convert Radiant to Degres
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); //set rotate with angle and forward vector*/
        float distance = Vector3.Distance(transform.position, targetPos);
        transform.Translate(Vector3.right * Time.deltaTime / 2); //moving right by time
        if (distance < 0.05)
        {
            transform.position = targetPos; //teleport ennemy to waypoint position when the distance < 0.05
            return true;
        }
        return false;
    }

    public IEnumerator GoToWork()
    {
        Vector3 workplace = job.GetWorkplacePos();
        while(!Move(workplace))
        {
            yield return null;
        }
        job.DoTheWork();
    }

    public void GoToSleep()
    {
        House newHouse = FindObjectOfType<House>();
        if(newHouse.inhabitant == null)
        {
            if(transform.position != newHouse.transform.position)
            {
                Move(newHouse.transform.position);
                gameObject.SetActive(false);
            }
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}

