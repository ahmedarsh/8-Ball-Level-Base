using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;
//using PlayFab.ClientModels;

public class LockZPosition : MonoBehaviour {

	public static bool isStiger;

    private Rigidbody rigid;
    private bool poted = false;
    private float minVelocity = 0.005f;//0.002f;
    private float minAngularVelocity = 0.25f; //0.15f;
    public bool ballActive = true;
    public PhysicMaterial material;
    private m_PotedBallsGUIController mPotedBallsGUI;
    public GameObject youWonMessage;
    private bool ballMoved = false;
    private bool audioDisabled = false;
    // Use this for initialization
    public AudioSource[] audioSources;
    private bool ballInactivePlayedAudio = false;
	Vector3 velocity;

    void Start() {



        audioSources = GetComponents<AudioSource>();
        rigid = GetComponent<Rigidbody>();

        // Set the sorting layer and order.
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "tableLayer";
        mPotedBallsGUI = GameObject.Find("PotedBallsGUI").GetComponent<m_PotedBallsGUIController>();

    }

    // Update is called once per frame
    bool ballInMovement = false;

    void Update() {
        if (ballActive && rigid.velocity.sqrMagnitude < minVelocity &&
            rigid.angularVelocity.magnitude < minAngularVelocity) {

            rigid.Sleep();
            ballInMovement = false;
//			print ("rigidbody Saleep"+rigid);
        }

        if (!rigid.IsSleeping() && !rigid.velocity.Equals(Vector3.zero) && !rigid.angularVelocity.Equals(Vector3.zero))
            ballInMovement = true;

        if (ballMoved && !audioDisabled && rigid.velocity.sqrMagnitude < minVelocity &&
            rigid.angularVelocity.magnitude < minAngularVelocity) {
            Debug.Log("AUDIO STOP");
            audioSources[3].Stop();
            audioDisabled = true;
        }

    }


    private bool firstBallPot = false;

	IEnumerator OnTriggerEnter(Collider other) {

        Debug.Log("TRIGGER: " + other.tag);


        if (!poted && other.tag.Contains("Pot")) {



			if (this.gameObject.CompareTag ("Ball")) {
//				Rigidbody rb = this.gameObject.GetComponent<Rigidbody> ();
//
//				velocity = rb.velocity;
//				print (" velocilty " + velocity);
//				rb.velocity = rb.velocity / 10f;
//				rb.maxAngularVelocity = 150;
//				rb.velocity = Vector3.zero;
//
//
//				rb.mass = 2f;
//				rb.drag = 2f;
//				rb.angularDrag = 2f;
//				rb.isKinematic = true;
//				rb.constraints = RigidbodyConstraints.FreezePositionX| RigidbodyConstraints.FreezeRotationX;
//				rb.constraints = RigidbodyConstraints.FreezePositionY| RigidbodyConstraints.FreezeRotationY;
//				rb.constraints = RigidbodyConstraints.FreezePositionZ| RigidbodyConstraints.FreezeRotationZ;

				print (" velocilty " + this.gameObject.GetComponent<Rigidbody> ().velocity);
			}

            audioSources[1].Play();
            GameManager.Instance.ballTouchedBand++;

            //if (!potedBallsGUI.potedBallsVisible) {
            if (!firstBallPot) {

                if (transform.tag.Equals("WhiteBall")) {
					print ("white ball poted");
					isStiger = true;
                    // White Ball Poted - nothing to do
                } else {
                    int ballNumber = 5;
                    // if (GameManager.Instance.ballsStriked) {
                    if (ballNumber == 8) {
                        // Black ball poted - game over
                        GameManager.Instance.iLost = true;
                    } else if (ballNumber < 8) {
                        firstBallPot = true;
                        GameManager.Instance.noTypesPotedSolid = true;
                    } else if (ballNumber > 8) {
                        firstBallPot = true;
                        GameManager.Instance.noTypesPotedStriped = true;
                    }
                }

            }


            Debug.Log("POT");

            if (transform.tag.Equals("WhiteBall")) {
                GameManager.Instance.wasFault = true;
				isStiger = true;
				print ("white ball poted");
				isStiger = true;
                GameManager.Instance.faultMessage = StaticStrings.potedCueBall;
                DisableWhiteBall();


            } else {
//                int ballNumber = System.Int32.Parse(transform.tag.Replace("Ball", ""));
				int ballNumber = 5;
                if (GameManager.Instance.cueController.isServer) {




                    if (GameManager.Instance.playersHaveTypes) {

                        //if (GameManager.Instance.validPotsCount >= 7) {
                        if ((GameManager.Instance.ownSolids && GameManager.Instance.solidPoted >= 7) ||
                            (!GameManager.Instance.ownSolids && GameManager.Instance.stripedPoted >= 7)) {
                            if (ballNumber == 8) {
                                if (GameManager.Instance.callPocketBlack) {
                                    if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID)) {
                                        GameManager.Instance.iWon = true;
                                    } else {
                                        GameManager.Instance.iLost = true;
                                    }
                                }
                                // else if (GameManager.Instance.callPocketAll)
                                // {
                                //     if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID))
                                //     {
                                //         GameManager.Instance.iWon = true;
                                //     }
                                //     else
                                //     {
                                //         GameManager.Instance.iLost = true;
                                //     }
                                // }
                                else {
                                    GameManager.Instance.iWon = true;
                                }
                            }
                        } else {


                            if (GameManager.Instance.ownSolids) {
                                if (ballNumber < 8) {
                                    if (GameManager.Instance.callPocketAll) {
                                        if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID)) {
                                            GameManager.Instance.validPotsCount++;
                                            GameManager.Instance.validPot = true;
                                        }
                                    } else {
                                        GameManager.Instance.validPotsCount++;
                                        GameManager.Instance.validPot = true;
                                    }
                                } else if (ballNumber == 8) {
                                    Debug.Log("Poted 8 ball - game over");
                                    GameManager.Instance.iLost = true;

                                }
                            } else {
                                if (ballNumber > 8) {
                                    if (GameManager.Instance.callPocketAll) {
                                        if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID)) {
                                            GameManager.Instance.validPotsCount++;
                                            GameManager.Instance.validPot = true;
                                        }

                                    } else {
                                        GameManager.Instance.validPotsCount++;
                                        GameManager.Instance.validPot = true;
                                    }
                                } else if (ballNumber == 8) {
                                    Debug.Log("Poted 8 ball - game over");
                                    GameManager.Instance.iLost = true;
                                }
                            }
                        }



                    } else {
                        if (ballNumber != 8) {
                            GameManager.Instance.validPotsCount++;
                            GameManager.Instance.validPot = true;
                        } else {
                            GameManager.Instance.iLost = true;
                        }
                    }


                }

                if (ballNumber < 8) {
                    GameManager.Instance.solidPoted++;
                } else if (ballNumber > 8) {
                    GameManager.Instance.stripedPoted++;
                }

                poted = true;
                DisableBall(transform.tag);
            }



			if (this.gameObject.CompareTag ("Ball")) {
		
				GlobalGameHandler.isballPocket = true;

				this.gameObject.transform.tag = "Untagged";
				GlobalGameHandler.ballNumber++;
				CheckBallTags ();

				this.gameObject.transform.localScale = new Vector3 (0.33f,0.33f,0.33f);
//				yield return new WaitForSeconds(2f);
//				this.gameObject.SetActive (false);
				this.gameObject.transform.parent=m_GameControllerScript.mGameController.ballpots.gameObject.transform;


				yield return new WaitForSeconds(1f);
//				this.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
				yield return new WaitForSeconds(1f);
				this.gameObject.GetComponent<Rigidbody> ().velocity = velocity;
				this.gameObject.GetComponent<Rigidbody> ().mass = 0.1f;
				this.gameObject.GetComponent<Rigidbody> ().drag = 0.12f;
				this.gameObject.GetComponent<Rigidbody> ().angularDrag = 0.45f;

			}

			if (transform.tag.Equals ("WhiteBall")) {
			
				GlobalGameHandler.WhiteBallPoted = true;	

//				int RL =	System.Convert.ToInt32(GameControllerScript.gameController.RemainingLifeTxt.text);
//				RL = RL - 2;
//				GameControllerScript.gameController.RemainingLifeTxt.text= RL .ToString();
//
//				GameControllerScript.gameController.heartBeating.SetActive (false);
//				GameControllerScript.gameController.heartBroken.SetActive (true);
			}

            //Invoke ("DisableBall", 0.0f);
		
        }
    }


	public void CheckBallTags()
	{
		print ("Ball Number"+GlobalGameHandler.ballNumber +"  balls length  "+GameManager.Instance.balls.Length);
		if (GlobalGameHandler.ballNumber == GameManager.Instance.balls.Length-1) {
			print ("level succesflag");
			GlobalGameHandler.LevelCompleteFlag = true;
		}

	}

    int ballNumber;
    void OnCollisionEnter(Collision collision) {

//        Debug.Log("COLLISION: " + collision.transform.tag);

        if (transform.tag.Contains("Ball") && collision.collider.tag.Equals("bumper")) {
            Vector3 v = GetComponent<Rigidbody>().velocity;
            float velSum = Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
            audioSources[2].volume = velSum / 8.0f;
            audioSources[2].Play();
        }

        if (GameManager.Instance.firstBallTouched && transform.tag.Contains("Ball") && collision.collider.tag.Equals("bumper")) {
            GameManager.Instance.ballTouchedBand++;
        }

        if (collision.collider.tag.Contains("Ball") && transform.tag.Contains("Ball")) {

            if (ballActive) {
                // Ball - ball collision
                if (audioSources.Length > 0) {
                    Vector3 v = GetComponent<Rigidbody>().velocity;

                    float velSum = Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);

                    Vector3 v2 = collision.gameObject.GetComponent<Rigidbody>().velocity;

                    float velSum2 = Mathf.Abs(v2.x) + Mathf.Abs(v2.y) + Mathf.Abs(v2.z);

                    if (velSum > velSum2) velSum = velSum2;

                    audioSources[0].volume = velSum / 10.0f;

                    audioSources[0].Play();

                }
            } else {
                if (!ballInactivePlayedAudio && audioSources.Length > 0) {
                    ballInactivePlayedAudio = true;
                    audioSources[0].volume = 0.6f;
                    audioSources[0].Play();
                }
            }



        }

        if (transform.tag.Equals("WhiteBall")) {
            if (collision.collider.tag.Contains("Ball") && !collision.collider.tag.Equals("WhiteBall") && GameManager.Instance.cueController.isServer) {
                // Check if touched my ball - otherwise fault
//				print ("white ball poted");
                if (!GameManager.Instance.firstBallTouched) {
                    GameManager.Instance.firstBallTouched = true;
//					print ("white ball poted");
//                    ballNumber = System.Int32.Parse(collision.collider.tag.Replace("Ball", ""));

                    if (GameManager.Instance.playersHaveTypes) {
                        Debug.Log("Inside");

                        //if (GameManager.Instance.validPotsCount >= 7) {
                        if ((GameManager.Instance.ownSolids && GameManager.Instance.solidPoted >= 7) ||
                            (!GameManager.Instance.ownSolids && GameManager.Instance.stripedPoted >= 7)) {
                            if (ballNumber != 8) {
                                GameManager.Instance.wasFault = true;
                                GameManager.Instance.faultMessage = StaticStrings.failedToHit8Ball;
                            }
                        } else {
                            if (GameManager.Instance.ownSolids) {
                                if (ballNumber >= 8) {
                                    // Touched not mine ball first - fault
                                    GameManager.Instance.wasFault = true;
                                    GameManager.Instance.faultMessage = StaticStrings.failedToHitSolid;
                                }
                            } else {
                                if (ballNumber <= 8) {
                                    // Touched not mine ball first - fault
                                    GameManager.Instance.wasFault = true;
                                    GameManager.Instance.faultMessage = StaticStrings.failedToHitStriped;
                                }
                            }
                        }
                    }
                }
            }
        } else {
            if (transform.tag.Contains("Ball") && collision.collider.tag.Equals("bumper")) {
                if (!GameManager.Instance.ballTouchBeforeStrike.Contains(transform.tag))
                    GameManager.Instance.ballTouchBeforeStrike.Add(transform.tag);
            }
        }
    }

    private void DisableWhiteBall() {


        Debug.Log("Disable White ball");
        //GetComponent <SphereCollider>().material = material;

        //rigid.Sleep ();
        ballActive = false;
        rigid.constraints = RigidbodyConstraints.None;
        Vector3 vel = rigid.velocity;
        //      if(vel.x > 1)
        //          vel.x = vel.x / (vel.x * vel.x);
        //      if(vel.y > 1)
        //          vel.y = vel.y / (vel.y * vel.y);
        vel.z = 5.0f;
        //vel.z = 0;
        rigid.velocity = vel;

        //rigid.angularVelocity = rigid.angularVelocity / 5;


        Invoke("deactiveWhite", 1.0f);
        //Invoke ("movePosition", 3.5f);

    }

    private void DisableBall(string ii) {



        Debug.Log("Disable");

        int i = 0;

        try {
            i = int.Parse(ii.Replace("Ball", ""));
        } catch (System.Exception e) { }

        if (i > 0 && i != 8) {
            mPotedBallsGUI.hidePotedBall(i - 1);
        }

        GetComponent<SphereCollider>().material = material;
        ballActive = false;
        //rigid.Sleep ();
        rigid.constraints = RigidbodyConstraints.None;
        Vector3 vel = rigid.velocity;
        //		if(vel.x > 1)
        //			vel.x = vel.x / (vel.x * vel.x);
        //		if(vel.y > 1)
        //			vel.y = vel.y / (vel.y * vel.y);
        vel.z = 5.0f;
        //vel.z = 0;
        rigid.velocity = vel;

        //rigid.angularVelocity = rigid.angularVelocity / 5;


        Invoke("disableMeshRenderer", 1.0f);
        Invoke("showMessage", 3.5f);

    }



    public void showMessage() {


        //Debug.Log ("Time " + (Time.time - messageTime));
        //        if(Time.time - messageTime > )

        float timeDiff = Time.time - GameManager.Instance.messageTime;

        Debug.Log("Time diff: " + timeDiff);


        if (timeDiff > 2) {
            movePosition();
			Debug.Log("Move Position");
            GameManager.Instance.messageTime = Time.time;
        } else {
            Debug.Log("Show message with delay");
//            StartCoroutine(showMessageWithDelay((2.0f - timeDiff) / 1.0f));
        }
    }

    IEnumerator showMessageWithDelay(float delayTime) {
        yield return new WaitForSeconds(delayTime);



        movePosition();
        GameManager.Instance.messageTime = Time.time;

    }


    public void EnableBall() {

    }

    private void deactiveWhite() {
        gameObject.SetActive(false);
    }



    private void disableMeshRenderer() {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void movePosition() {
        Debug.Log("Play sound!!");
        //audioSources[0].PlayOneShot(audioSources[3].clip, 1.0f);
        audioSources[3].Play();

        rigid.Sleep();
        GetComponent<MeshRenderer>().enabled = true;
        //		rigid.useGravity = false;
        rigid.transform.position = new Vector3(5.61f, 1.317f, 5.45f);
        //ballMoved = true;
        Invoke("setBallMoved", 1.0f);
    }

    private void setBallMoved() {
        ballMoved = true;
    }


}
