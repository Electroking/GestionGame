using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Job
{
    public static List<Villager> idleList = new List<Villager>();
    const int maxBuildersPerBuilding = 2;
    const float workAmountPerSecond = 0.1f;
    Building building;

    public Builder() : base()
    {

    }

    public override bool DoTheWork()
    {
        building.Build(workAmountPerSecond * Time.deltaTime);
        if (building.isBuilt && !idleList.Contains(villager))
        {
            idleList.Add(villager);
            return true;
        }
        return false;

        /*if (villager.isExhausted) { yield break; }
        if (building.isBuilt) {
            idleList.Add(villager);
        }*/
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        Building[] unbuiltBuildings = Building.unbuiltList.ToArray();
        for (int n = 0; n < maxBuildersPerBuilding; n++)
        {
            for (int i = 0; i < unbuiltBuildings.Length; i++)
            {
                if (!unbuiltBuildings[i].isBuilt && unbuiltBuildings[i].builders.Count == n)
                {
                    if (idleList.Contains(villager))
                    {
                        idleList.Remove(villager);
                    }
                    building = unbuiltBuildings[i];
                    building.builders.Add(villager);
                    workplace = building.transform.position; // TODO: get the closest possible position to the building
                    return true;
                }
            }
        }
        return false;
    }
}