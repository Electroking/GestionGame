using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpInfo : MonoBehaviour
{
    [SerializeField] Building.Type buildingType;

    private void Awake()
    {
        Text text = GetComponentInChildren<Text>();
        text.text = "Stone: " + buildingType.GetStoneCost() + ", Wood: " + buildingType.GetWoodCost();
    }
}
