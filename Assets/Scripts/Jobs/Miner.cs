using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Job
{
    public static Dictionary<GameObject, Villager> mineDic = new Dictionary<GameObject, Villager>();
    public static GameObject[] mineArray;
    float mineRadius = 1;

    public Miner() : base()
    {
    }


    public override IEnumerator DoTheWork()
    {
        while (!villager.isExhausted)
        {
            yield return new WaitForSeconds(1);
            GameManager.instance.Stone += 1;
        }
    }

    public override Vector3 GetWorkplacePos()
    {
        mineArray = mineArray.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < mineArray.Length; i++)
        {
            if (mineDic[mineArray[i]] == null || mineDic[mineArray[i]] == villager)
            {
                mineDic[mineArray[i]] = villager;
                Vector3 relative = mineArray[i].transform.position - villager.transform.position;
                return relative - relative.normalized * mineRadius * mineArray[i].transform.localScale.x + villager.transform.position;
            }
        }
        return Vector3.zero;
    }
}