using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Job
{
    public static Dictionary<GameObject, Villager> bushDic = new Dictionary<GameObject, Villager>();
    public static GameObject[] bushArray;
    float bushRadius = 1, timeToWork = 1, timer = 0;

    public Gatherer() : base()
    {
    }


    public override bool DoTheWork()
    {
        timer += Time.deltaTime;
        if (timer >= timeToWork)
        {
            GameManager.instance.Food += 1;
            timer -= timeToWork;
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
                    return true;
                }
            }
        }
        bushArray = bushArray.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < bushArray.Length; i++)
        {
            if (bushDic[bushArray[i]] == null || bushDic[bushArray[i]] == villager)
            {
                bushDic[bushArray[i]] = villager;
                workplace = bushArray[i].transform.position;
                return true;
            }
        }
        return false;
    }
}