using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Job
{
    public static Vector3[] bushArray;
    static Dictionary<Vector3, Villager> bushDic = null;
    float timeToWork = 2, timer = 0;
    int foodAmount = 1;
    public Farm farm;
    Vector3? currentBush;

    public Gatherer() : base()
    {
        if (bushDic != null) return;
        bushDic = new Dictionary<Vector3, Villager>();
        for (int i = 0; i < bushArray.Length; i++)
        {
            bushDic.Add(bushArray[i], null);
        }
    }

    public override bool DoTheWork()
    {
        timer += Time.deltaTime;
        if (timer >= timeToWork)
        {
            GameManager.instance.Food += foodAmount * (farm != null ? Farm.multiplier : 1);
            timer -= timeToWork;
            // (farm != null) farm.nbWorkers--; //TEMPORARY SOLUTION! Because whenever DoTheWork() returns true, GetWorkplacePos is called. TODO: rework the code.
            return true;
        }
        return false;
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        if (farm != null)
        {
            workplace = farm.transform.position;
            return true;
        }
        if (Farm.list.Count > 0)
        {
            List<Farm> farmListOrdered = Farm.list.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToList();
            for (int i = 0; i < farmListOrdered.Count; i++)
            {
                if (farmListOrdered[i].TryToAddNewWorker(villager))
                {
                    if (currentBush != null)
                    {
                        bushDic[(Vector3)currentBush] = null;
                        currentBush = null;
                    }
                    farm = farmListOrdered[i];
                    workplace = farm.transform.position;
                    return true;
                }
            }
        }
        if (currentBush != null)
        {
            workplace = (Vector3)currentBush;
            return true;
        }
        bushArray = bushArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < bushArray.Length; i++)
        {
            if (bushDic[bushArray[i]] == null)
            {
                if (farm != null)
                {
                    farm.RemoveWorker(villager);
                    farm = null;
                }
                bushDic[bushArray[i]] = villager;
                currentBush = workplace = bushArray[i];
                return true;
            }
        }
        return false;
    }

    public override void OnDie()
    {
        base.OnDie();
        if (farm != null)
        {
            farm.RemoveWorker(villager);
        }
    }
}