using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : Job
{
    public static Dictionary<GameObject, Villager> treeDic = new Dictionary<GameObject, Villager>();
    public static GameObject[] treeArray;
    float treeRadius = 1;

    public Lumberjack() : base()
    {
    }


    public override IEnumerator DoTheWork()
    {
        while (!villager.isExhausted)
        {
            yield return new WaitForSeconds(1);
            GameManager.instance.Wood += 1;
        }
    }

    public override Vector3 GetWorkplacePos()
    {
        treeArray = treeArray.OrderBy((d) => (d.transform.position - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < treeArray.Length; i++)
        {
            if (treeDic[treeArray[i]] == null || treeDic[treeArray[i]] == villager)
            {
                treeDic[treeArray[i]] = villager;
                Vector3 relative = treeArray[i].transform.position - villager.transform.position;
                return relative - relative.normalized * treeRadius * treeArray[i].transform.localScale.x + villager.transform.position;
            }
        }
        return Vector3.zero;
    }
}