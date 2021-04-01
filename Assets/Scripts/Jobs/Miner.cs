using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Job
{
    public static Vector3[] mineArray;
    static Dictionary<Vector3, Villager> mineDic = null;

    Vector3? currentMine;

    float timeToWork = 3, timer = 0;

    public Miner() : base() // Initialize mineDic with empty values.
    {
        if (mineDic != null) return; // So only the first Miner does it.
        mineDic = new Dictionary<Vector3, Villager>();
        for (int i = 0; i < mineArray.Length; i++)
        {
            mineDic.Add(mineArray[i], null);
        }
    }


    public override bool DoTheWork()
    {
        timer += Time.deltaTime;
        if (timer >= timeToWork)
        {
            GameManager.instance.Stone += 1;
            timer -= timeToWork;
        }
        return false;
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        if (currentMine != null) // If the villager already got a mine, he stays on it.
        {
            workplace = (Vector3)currentMine;
            return true;
        }

        // Search for the nearest free mine.
        mineArray = mineArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < mineArray.Length; i++)
        {
            if (mineDic[mineArray[i]] == null)
            {
                mineDic[mineArray[i]] = villager;
                currentMine = workplace = mineArray[i];
                return true;
            }
        }
        return false;
    }
}