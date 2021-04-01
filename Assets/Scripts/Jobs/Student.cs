using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : Job
{
    public Job.Type jobToLearn;
    float timeToLearn = 20, timer = 0;

    public Student() : base()
    {

    }

    public override bool DoTheWork()
    {
        timer += Time.deltaTime;
        if (timer >= timeToLearn)
        {
            villager.AssignJob(jobToLearn);
            return true;
        }
        return false;
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        if (School.list.Count == 0) return false;

        // Search for the closest school.
        int schoolIndex = 0;
        for (int i = 0; i < School.list.Count; i++)
        {
            if (Vector3.Distance(villager.transform.position, School.list[schoolIndex].transform.position) > Vector3.Distance(villager.transform.position, School.list[i].transform.position))
            {
                schoolIndex = i;
            }
        }
        workplace = School.list[schoolIndex].transform.position;
        return true;
    }

    public override string ToString()
    {
        return base.ToString() + $" [{jobToLearn} ({(int)(100 * timer / timeToLearn)}%)]";
    }
}