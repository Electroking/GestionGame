using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    public static List<Farm> list = new List<Farm>();
    public static int multiplier = 2;

    public int workersCapacity = 4;
    public List<Villager> workers = new List<Villager>();

    protected override void OnBuilt()
    {
        list.Add(this);
    }

    public bool TryToAddNewWorker(Villager newWorker)
    {
        if (workers.Contains(newWorker)) return true;
        if (workers.Count < workersCapacity)
        {
            workers.Add(newWorker);
            return true;
        }
        return false;
    }
    public void RemoveWorker(Villager worker)
    {
        if (workers.Contains(worker)) workers.Remove(worker);
    }
}
