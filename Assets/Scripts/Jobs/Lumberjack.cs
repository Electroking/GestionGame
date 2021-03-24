using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : Job
{
    public static Dictionary<GameObject, Villager> treeDic = new Dictionary<GameObject, Villager>();
    public static GameObject[] treeArray;
    float treeRadius = 1, timeToWork = 1, timer = 0;

    public Lumberjack() : base()
    {
    }


    public override bool DoTheWork()
    {
        timer += Time.deltaTime;
        if (timer >= timeToWork)
        {
            GameManager.instance.Wood += 1;
            timer -= timeToWork;
        }
        return false;
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        treeArray = treeArray.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < treeArray.Length; i++)
        {
            if (treeDic[treeArray[i]] == null || treeDic[treeArray[i]] == villager)
            {
                treeDic[treeArray[i]] = villager;
                Vector3 relative = treeArray[i].transform.position - villager.transform.position;
                workplace = relative - relative.normalized * treeRadius * treeArray[i].transform.localScale.x + villager.transform.position;
                return true;
            }
        }
        return false;
    }
}