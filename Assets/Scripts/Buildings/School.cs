using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class School : Building
{
    public static List<School> list = new List<School>();

    protected override void OnBuilt()
    {
        list.Add(this);
        if (list.Count == 1)
        {
            UIManager.instance.uiVillager.OnFirstSchoolBuilt();
        }
    }

    public void ChangeJob(Villager villager)
    {

    }
}
