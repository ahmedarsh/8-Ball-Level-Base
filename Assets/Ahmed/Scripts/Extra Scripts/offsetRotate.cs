using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offsetRotate : MonoBehaviour {

//	Vector3 mPrevPos= Vector3.zero;
//	Vector3 mPosDelta= Vector3.zero;





	Renderer rend;

	void Start()
	{
		GlobalGameHandler.isDrageDecrese = false;
		rend = this.gameObject.GetComponent<Renderer> ();

	}
	void OnMouseDown()
	{

		GlobalGameHandler.isDrageDecrese = true;

//		print ("is drage "+GlobalGameHandler.isDrageDecrese);
	}

	void OnMouseUp()
	{
		GlobalGameHandler.isDrageDecrese = false;

		this.gameObject.GetComponent<AudioSource> ().enabled = false;
//		print ("is drage "+GlobalGameHandler.isDrageDecrese);
	}

	void OnMouseDrag()
	{
		this.gameObject.GetComponent<AudioSource> ().enabled = true;
		float newYPos = Camera.main.ScreenToWorldPoint (Input.mousePosition).y;

		newYPos = newYPos / 15;

		rend.material.SetTextureOffset("_MainTex", new Vector2(0,newYPos));

		this.gameObject.GetComponent<AudioSource> ().Play();



	}









//	void Update()
//	{
	
//		if (Input.GetMouseButton (0)) 
//		{
//		
//			mPosDelta = Input.mousePosition - mPrevPos;
//
//			float newYPos = Camera.main.ScreenToWorldPoint (Input.mousePosition).y;
//
////			float newYPos = mPrevPos.x + (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - mPrevPos.x);
//
//			rend.material.SetTextureOffset("_MainTex", new Vector2(newYPos, 0.1f));
//
//			print ("mouse pos"+newYPos);
////			this.gameObject.G
//		}
//		mPrevPos = Input.mousePosition;
//	}



}
