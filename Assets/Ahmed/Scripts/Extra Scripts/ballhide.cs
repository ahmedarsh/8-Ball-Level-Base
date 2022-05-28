using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballhide : MonoBehaviour {


	IEnumerator OnTriggerEnter(Collider col)
	{
		yield return new WaitForSeconds (1f);

		if (col.gameObject.layer==10) {
			print ("ballname" + col.name);
			col.gameObject.SetActive (false);
		}
	}
}
