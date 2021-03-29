using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Job
{
    public static Vector3[] bushArray;
    static Dictionary<Vector3, Villager> bushDic = null;
    float timeToWork = 2, timer = 0;
    bool isWorkingInFarm = false;
    int foodAmount = 1;

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
            GameManager.instance.Food += foodAmount * (isWorkingInFarm ? Farm.multiplier : 1);
            timer -= timeToWork;
            return true;
        }
        return false;
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        if (Farm.list.Count >= 0)
        {
            List<Farm> farmListOrdered = Farm.list.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToList();
            for (int i = 0; i < farmListOrdered.Count; i++)
            {
                if (farmListOrdered[i].nbWorkers < farmListOrdered[i].workersCapacity)
                {
                    workplace = farmListOrdered[i].transform.position;
                    isWorkingInFarm = true;
                    return true;
                }
            }
        }
        bushArray = bushArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < bushArray.Length; i++)
        {
            if (bushDic[bushArray[i]] == null || bushDic[bushArray[i]] == villager)
            {
                bushDic[bushArray[i]] = villager;
                /*Vector3 relative = bushArray[i].transform.position - villager.transform.position;
                workplace = relative - relative.normalized * bushRadius * bushArray[i].transform.localScale.x + villager.transform.position;*/
                workplace = bushArray[i];
                isWorkingInFarm = false;
                return true;
            }
        }
        return false;
    }
}