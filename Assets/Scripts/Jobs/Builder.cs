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

    public override IEnumerator DoTheWork()
    {
        while (!villager.isExhausted && !building.isBuilt) {
            yield return null;
            building.Build(workAmountPerSecond * Time.deltaTime);
        }
        if (!idleList.Contains(villager)) {
            idleList.Add(villager);
        }
        /*if (villager.isExhausted) { yield break; }
        if (building.isBuilt) {
            idleList.Add(villager);
        }*/
    }

    public override Vector3 GetWorkplacePos()
    {
        Building[] unbuiltBuildings = Building.unbuiltList.ToArray();
        for (int n=0; n<maxBuildersPerBuilding; n++) {
            for(int i = 0; i < unbuiltBuildings.Length; i++) {
                if(!unbuiltBuildings[i].isBuilt && unbuiltBuildings[i].builders.Count == n) {
                    if (idleList.Contains(villager)) {
                        idleList.Remove(villager);
                    }
                    building = unbuiltBuildings[i];
                    building.builders.Add(villager);
                    return building.transform.position; // TODO: get the closest possible position to the building
                }
            }
        }
        return Vector3.zero;
    }
}