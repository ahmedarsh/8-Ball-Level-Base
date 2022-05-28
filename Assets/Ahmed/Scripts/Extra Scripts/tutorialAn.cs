using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialAn : MonoBehaviour {
	public GameObject tutorial1 , tutorial2,lineRend;
	// Use this for initialization
	public bool isTouched = false;


	void Start () {
		GlobalGameHandler.tutoral_anim = 0;
		tutorial1.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0))
		{
			isTouched = true;	
		}


		if (!Input.GetMouseButtonDown (0) && isTouched == true) {
			isTouched = false;
			if (GlobalGameHandler.tutoral_anim == 0) {
				tutorial1.SetActive (true);
				print ("anim 1");
			} else if (GlobalGameHandler.tutoral_anim ==1 && lineRend.activeSelf == true) {
				tutorial1.SetActive (false);
				tutorial2.SetActive (true);

				print ("anim 2");
			} else if (GlobalGameHandler.tutoral_anim >= 2) {
				tutorial1.SetActive (false);
				tutorial2.SetActive (false);

				print ("anim 3");
			}
				GlobalGameHandler.tutoral_anim++;



		}

	}
}
