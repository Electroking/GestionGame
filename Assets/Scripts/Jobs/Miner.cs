using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Job
{
    public static Vector3[] mineArray;
    static Dictionary<Vector3, Villager> mineDic = null;

    float timeToWork = 1, timer = 0;

    public Miner() : base() {
        if (mineDic != null) return;
        mineDic = new Dictionary<Vector3, Villager>();
        for(int i = 0; i < mineArray.Length; i++) {
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
        mineArray = mineArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < mineArray.Length; i++)
        {
            if (mineDic[mineArray[i]] == null || mineDic[mineArray[i]] == villager)
            {
                mineDic[mineArray[i]] = villager;
                /*Vector3 relative = mineArray[i] - villager.transform.position;
                workplace = relative - relative.normalized * mineRadius * mineArray[i].transform.localScale.x + villager.transform.position;*/
                workplace = mineArray[i];
                return true;
            }
        }
        return false;
    }
}