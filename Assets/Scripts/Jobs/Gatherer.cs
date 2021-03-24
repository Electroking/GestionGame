using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Job
{
    public static Vector3[] bushArray;
    static Dictionary<Vector3, Villager> bushDic = null;
    float timeToWork = 1, timer = 0;

    public Gatherer() : base()
    {
        if(bushDic != null) return;
        bushDic = new Dictionary<Vector3, Villager>();
        for(int i = 0; i < bushArray.Length; i++) {
            bushDic.Add(bushArray[i], null);
        }
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
        bushArray = bushArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < bushArray.Length; i++)
        {
            if (bushDic[bushArray[i]] == null || bushDic[bushArray[i]] == villager)
            {
                bushDic[bushArray[i]] = villager;
                /*Vector3 relative = bushArray[i].transform.position - villager.transform.position;
                workplace = relative - relative.normalized * bushRadius * bushArray[i].transform.localScale.x + villager.transform.position;*/
                workplace = bushArray[i];
                return true;
            }
        }
        return false;
    }
}