using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job {
    public enum Type {
        Builder, Miner, Lumberjack, Gatherer, Student
    }

    public static Job GetNewJob(Job.Type jobType, bool trueJobsOnly = false) {
        Job job = null;
        switch (jobType) {
            case Job.Type.Builder:
                job = new Builder();
                break;
            case Job.Type.Miner:
                job = new Miner();
                break;
            case Job.Type.Lumberjack:
                job = new Lumberjack();
                break;
            case Job.Type.Gatherer:
                job = new Gatherer();
                break;
            case Job.Type.Student:
                if (!trueJobsOnly) {
                    job = new Student();
                }
                break;
            default:
                break;
        }
        return job;
    }

    public Job() {

    }

    public abstract Vector3 GetWorkplacePos();

    public abstract void DoTheWork();
}