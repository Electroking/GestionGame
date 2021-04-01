using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : Building
{
    [SerializeField] float prosperityPerSecond = 0.2f;

    protected override void OnBuilt()
    {
        StartCoroutine(nameof(IncrementProsperity));
    }
    IEnumerator IncrementProsperity()
    {
        while (GameManager.instance.Prosperity < GameManager.instance.maxProsperity)
        {
            yield return null;
            GameManager.instance.Prosperity += prosperityPerSecond * Time.deltaTime;
        }
    }
}
