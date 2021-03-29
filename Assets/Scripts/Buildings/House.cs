using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public static List<House> list = new List<House>();
    public List<Villager> inhabitants = new List<Villager>();
    public int maxInhabitant = 1;

    protected override void OnBuilt()
    {
        House.list.Add(this);
    }
}
