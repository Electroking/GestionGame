using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class School : Building
{
    public List<Villager> students = new List<Villager>();
    public static List<School> list = new List<School>();

    protected override void OnBuilt() {
        list.Add(this);
    }

    public void ChangeJob(Villager villager)
    {

    }
}
