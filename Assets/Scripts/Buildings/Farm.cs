using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    public static List<Farm> farmsList = new List<Farm>();

    protected override void OnBuilt()
    {
        farmsList.Add(this);
    }
}
