using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpInfo : MonoBehaviour
{
    Text text;
    public Building.Type bType;
    private void Awake()
    {
        text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }
    public void SetText(int numType)
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
    }

}
