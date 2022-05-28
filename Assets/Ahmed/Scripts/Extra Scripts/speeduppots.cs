using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speeduppots : MonoBehaviour {

	Vector3 velocity;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
	
		if (col.gameObject.CompareTag ("Ball")) {
			velocity = col.gameObject.GetComponent<Rigidbody> ().velocity;
			print (col.gameObject.name+" v   "+col.gameObject.GetComponent<Rigidbody> ().velocity);
			col.gameObject.GetComponent<Rigidbody> ().velocity = col.gameObject.GetComponent<Rigidbody> ().velocity * 15f;

			print (col.gameObject.name+" v   "+col.gameObject.GetComponent<Rigidbody> ().velocity);
		}
	}
	void OnTriggerExit(Collider col)
	{

		if (col.gameObject.CompareTag ("Ball")) {
			
			print (col.gameObject.name+" v   "+col.gameObject.GetComponent<Rigidbody> ().velocity);
			col.gameObject.GetComponent<Rigidbody> ().velocity = velocity;

			print (col.gameObject.name+" v   "+col.gameObject.GetComponent<Rigidbody> ().velocity);
		}



	}



	void OnTriggerStay(Collider col)
	{
	
		if (col.gameObject.CompareTag ("Ball")) {

			col.gameObject.GetComponent<Rigidbody> ().velocity = velocity;

		}
	}

}

