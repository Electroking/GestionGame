using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : Job
{
    public static Vector3[] treeArray;
    static Dictionary<Vector3, Villager> treeDic = null;
    float timeToWork = 1, timer = 0;

    public Lumberjack() : base() {
        if(treeDic != null) return;
        treeDic = new Dictionary<Vector3, Villager>();
        for(int i = 0; i < treeArray.Length; i++) {
            treeDic.Add(treeArray[i], null);
        }
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
        treeArray = treeArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < treeArray.Length; i++)
        {
            if (treeDic[treeArray[i]] == null || treeDic[treeArray[i]] == villager)
            {
                treeDic[treeArray[i]] = villager;
                /*Vector3 relative = treeArray[i].transform.position - villager.transform.position;
                workplace = relative - relative.normalized * treeRadius * treeArray[i].transform.localScale.x + villager.transform.position;*/
                // testing 
                workplace = treeArray[i];
                return true;
            }
        }
        return false;
    }
}