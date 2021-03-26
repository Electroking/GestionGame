using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public static List<House> list = new List<House>();
    public Villager inhabitant;

    protected override void OnBuilt()
    {
        House.list.Add(this);
    }
}
