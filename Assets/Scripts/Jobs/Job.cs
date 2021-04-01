using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Job
{
    public enum Type
    {
        Builder, Miner, Lumberjack, Gatherer, Student
    }
    public Material material;
    public Villager villager;

    public Job()
    {

    }

    /// <summary>
    /// Get the position of a free workplace.
    /// </summary>
    /// <param name="workplace">Position of the workplace.</param>
    /// <returns><b>True</b> if a workplace was found.</returns>
    public abstract bool GetWorkplacePos(out Vector3 workplace);

    public abstract bool DoTheWork();

    public virtual void OnGoToSleep()
    {

    }

    public virtual void OnDie()
    {

    }

    public override string ToString()
    {
        return GetType().Name;
    }

    /// <summary>
    /// Get a new instance of a job based on a Job.Type and assign it to a villager.
    /// </summary>
    /// <param name="villager">The villager to set the new job to.</param>
    /// <param name="jobType"></param>
    /// <param name="trueJobsOnly">If <b>true</b>, ignore the student Job.Type</param>
    /// <returns>The new Job.</returns>
    public static Job GetNewJob(Villager villager, Job.Type jobType, bool trueJobsOnly)
    {
        Job job = null;
        Color color = Color.white;
        switch (jobType)
        {
            case Job.Type.Builder:
                job = new Builder();
                color = Color.black;
                break;
            case Job.Type.Miner:
                job = new Miner();
                color = Color.red;
                break;
            case Job.Type.Lumberjack:
                job = new Lumberjack();
                color = Color.cyan;
                break;
            case Job.Type.Gatherer:
                job = new Gatherer();
                color = Color.green;
                break;
            case Job.Type.Student:
                if (!trueJobsOnly)
                {
                    job = new Student();
                    color = Color.yellow;
                }
                break;
            default:
                break;
        }

        if (job != null) job.villager = villager;
        villager.GetComponentInChildren<Renderer>().material.color = color;
        return job;
    }
}