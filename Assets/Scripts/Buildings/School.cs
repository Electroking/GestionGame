using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class School : Building
{
    public List<Villager> students = new List<Villager>();
    public static List<School> list = new List<School>();

    protected override void OnBuilt()
    {
        if (list.Count == 0)
        {
            UIManager.instance.uiVillager.OnFirstSchoolBuilt();
        }
        list.Add(this);
    }

    public void ChangeJob(Villager villager)
    {

    }
}
