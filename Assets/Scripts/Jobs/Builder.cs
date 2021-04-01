using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Job
{
    public static List<Villager> idleList = new List<Villager>();

    const int maxBuildersPerBuilding = 2;
    const float workAmountPerSecond = 1f;

    public Building building;

    public Builder() : base()
    {

    }

    public override bool DoTheWork()
    {
        building.Build(workAmountPerSecond * Time.deltaTime);
        if (building.isBuilt && !idleList.Contains(villager)) //Stop the contruction of the building when done.
        {
            building = null;
            idleList.Add(villager);
            return true;
        }
        return false;
    }

    public override bool GetWorkplacePos(out Vector3 workplace)
    {
        workplace = Vector3.zero;
        if (building != null)
        {
            if (!building.isBuilt)
            {
                AssignToBuilding(building);
                workplace = building.transform.position;
                return true;
            }
            building = null;
        }
        Building[] unbuiltBuildings = Building.unbuiltList.ToArray();
        for (int n = 0; n < maxBuildersPerBuilding; n++)
        {
            for (int i = 0; i < unbuiltBuildings.Length; i++)
            {
                if (!unbuiltBuildings[i].isBuilt && unbuiltBuildings[i].builders.Count == n)
                {
                    AssignToBuilding(unbuiltBuildings[i]);
                    workplace = building.transform.position;
                    return true;
                }
            }
        }
        return false;
    }

    void AssignToBuilding(Building building)
    {
        if (idleList.Contains(villager))
        {
            idleList.Remove(villager);
        }
        this.building = building;
        this.building.builders.Add(villager);
    }

    public override void OnGoToSleep()
    {
        base.OnGoToSleep();
    }
}