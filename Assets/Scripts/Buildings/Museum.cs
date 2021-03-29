using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Museum : Building
{
    public static int nbMuseums;
    public float nbprosperity;

    [SerializeField] float prosperityPerSecond = 2f;

    protected override void OnBuilt()
    {
        nbMuseums += 1;
        StartCoroutine(nameof(UpProsperity));
    }
    IEnumerator UpProsperity()
    {
        while (GameManager.instance.Prosperity < GameManager.instance.maxProsperity)
        {
            yield return null;
            GameManager.instance.Prosperity += prosperityPerSecond * Time.deltaTime;
            //Debug.Log("Prosperity " + GameManager.instance.Prosperity);
        }
    }
}
