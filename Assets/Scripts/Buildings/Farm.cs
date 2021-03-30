using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    public static List<Farm> list = new List<Farm>();
    public static int multiplier = 2;

    public int workersCapacity = 1, nbWorkers;

    protected override void OnBuilt()
    {
        list.Add(this);
    }
}
