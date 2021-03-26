using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpInfo : MonoBehaviour
{
    //Text text;
    [SerializeField] Building.Type buildingType;

    private void Awake()
    {
        Text text = GetComponentInChildren<Text>();
        text.text = "Stone: " + buildingType.GetStoneCost() + ", Wood: " + buildingType.GetWoodCost();
    }

    /*public void SetText(int numType)
    {
        int stone, wood;
        switch (numType)
        {
            case 0:
                stone = 0; wood = 0;
                text.text = "House Cost :\nStone : " + stone + "\nWood : " + wood;
                break;
            case 1:
                stone = 0; wood = 0;
                text.text = "School Cost :\nStone : " + stone + "\nWood : " + wood;
                break;
            case 2:
                stone = 0; wood = 0;
                text.text = "Farm Cost :\nStone : " + stone + "\nWood : " + wood;
                break;
            case 3:
                stone = 0; wood = 0;
                text.text = "Museum Cost :\nStone : " + stone + "\nWood : " + wood;
                break;
            case 4:
                stone = 0; wood = 0;
                text.text = "Library Cost :\nStone : " + stone + "\nWood : " + wood;
                break;
        }
    }*/

}
