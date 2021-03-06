﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : Job
{
    public static Vector3[] treeArray;
    static Dictionary<Vector3, Villager> treeDic = null;

    float timeToWork = 3, timer = 0;
    Vector3? currentTree;

    public Lumberjack() : base() // Initialize treeDic with empty values.
    {
        if (treeDic != null) return; // So only the first Lumberjack does it.
        treeDic = new Dictionary<Vector3, Villager>();
        for (int i = 0; i < treeArray.Length; i++)
        {
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
        if (currentTree != null) // If the villager already got a tree, he stays on it.
        {
            workplace = (Vector3)currentTree;
            return true;
        }

        // Search for the nearest free bush.
        treeArray = treeArray.OrderBy((d) => (d - villager.transform.position).sqrMagnitude).ToArray();
        for (int i = 0; i < treeArray.Length; i++)
        {
            if (treeDic[treeArray[i]] == null)
            {
                treeDic[treeArray[i]] = villager;
                currentTree = workplace = treeArray[i];
                return true;
            }
        }
        return false;
    }
}