using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    public static List<Farm> list = new List<Farm>();
    public int workersCapacity = 4, nbWorkers;

    protected override void Built()
    {
        list.Add(this);
    }
}
