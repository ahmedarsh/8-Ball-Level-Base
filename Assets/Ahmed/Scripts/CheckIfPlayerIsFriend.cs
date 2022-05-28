using System.Collections;
using System.Collections.Generic;
//using PlayFab;
//using PlayFab.ClientModels;
using UnityEngine;

public class CheckIfPlayerIsFriend : MonoBehaviour {

    public GameObject AddFriendButton;
    public GameObject mainObject;
    // Use this for initialization
    void Start() {
        
        AddFriendButton.SetActive(false);
        mainObject.GetComponent<RectTransform>().sizeDelta = new Vector2(mainObject.GetComponent<RectTransform>().sizeDelta.x, 260.0f);

    }

}
