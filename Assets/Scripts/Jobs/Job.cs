using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Job
{
    public Material material;
    public enum Type
    {
        Builder, Miner, Lumberjack, Gatherer, Student
    }

    public Villager villager;

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

    public Job()
    {

    }

    public abstract bool GetWorkplacePos(out Vector3 workplace); // TODO: Make a common Method for all Job scripts

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
}