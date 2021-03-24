using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class School : Building
{
    public List<Villager> students = new List<Villager>();
    public static List<School> list = new List<School>();

    protected override void OnBuilt() {
        // when building gets buit
    }

    public void ChangeJob(Villager villager)
    {

    }
}
