using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour {

	bool isRewinding = false;

	public float recordTime = 0.2f;

	List<PointInTime> pointsInTime;

	Rigidbody rb;


	// Use this for initialization
	void Start () {
		pointsInTime = new List<PointInTime>();
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalGameHandler.isBumper)
			StartRewind();
		if (!GlobalGameHandler.isBumper)
			StopRewind();
	}

	void FixedUpdate ()
	{
		if (isRewinding)
			Rewind();
		else
			Record();
	}

	void Rewind ()
	{
		if (pointsInTime.Count > 0)
		{
			PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			pointsInTime.RemoveAt(0);
//			print (GlobalGameHandler.isBumper+"   isbumper");
		} else
		{
			StopRewind();
		}
		
	}

	void Record ()
	{
		if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
		{
			pointsInTime.RemoveAt(pointsInTime.Count - 1);
		}

		pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));

		print (pointsInTime [10]+"position ");

		print (pointsInTime [0]+"position ");
	}

	public void StartRewind ()
	{
		isRewinding = true;
		rb.isKinematic = true;
	}

	public void StopRewind ()
	{
		isRewinding = false;
		rb.isKinematic = false;
	}
}
