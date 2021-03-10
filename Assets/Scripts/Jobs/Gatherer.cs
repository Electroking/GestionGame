using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Job
{
    public static Dictionary<GameObject, Villager> bushDic = new Dictionary<GameObject, Villager>();
    public static GameObject[] bushArray;
    float bushRadius = 1;

    public Gatherer() : base()
    {
    }


    public override IEnumerator DoTheWork()
    {
        while (!villager.isExhausted)
        {
            yield return new WaitForSeconds(1);
            GameManager.instance.Food += 1;
        }
    }

    public override Vector3 GetWorkplacePos()
    {
        bushArray = bushArray.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < bushArray.Length; i++)
        {
            if (bushDic[bushArray[i]] == null || bushDic[bushArray[i]] == villager)
            {
                bushDic[bushArray[i]] = villager;
                Vector3 relative = bushArray[i].transform.position - villager.transform.position;
                return relative - relative.normalized * bushRadius * bushArray[i].transform.localScale.x + villager.transform.position;
            }
        }
        return Vector3.zero;
    }
}