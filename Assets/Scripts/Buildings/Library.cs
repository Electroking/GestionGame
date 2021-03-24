using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : Building
{
    public static int nbLibraries;

    protected override void Built()
    {
        nbLibraries += 1;
        StartCoroutine("UpProsperity");
    }
    IEnumerator UpProsperity()
    {
        while (GameManager.instance.Prosperity < GameManager.instance.maxProsperity)
        {
            yield return new WaitForSeconds(20);
            GameManager.instance.Prosperity += 1;
            Debug.Log("Prosperity " + GameManager.instance.Prosperity);
        }
    }
}
