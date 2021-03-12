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
    public void SetText()
    {
        int stone = 0, wood = 0;
        text.text = "House Cost :\nStone : " + stone + "\nWood : " + wood;
    }

}
