using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Museum : Building
{
    public static int nbMuseums;
    public float nbprosperity;

    protected override void Built()
    {
        nbMuseums += 1;
        StartCoroutine("UpProsperity");
    }
    IEnumerator UpProsperity()
    {
        while (GameManager.instance.Prosperity < 100)
        {
            yield return new WaitForSeconds(20);
            GameManager.instance.Prosperity += 5;
            Debug.Log("Prosperity " + GameManager.instance.Prosperity);
        }
    }
}
