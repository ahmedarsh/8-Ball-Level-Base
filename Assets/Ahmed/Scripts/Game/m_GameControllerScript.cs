using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;
using UnityEngine.SceneManagement;


public class m_GameControllerScript : MonoBehaviour {
	public GameObject [] Positions;

	public GameObject  White_Ball, White_Ball_Pos;
//	public ArrayList arraylist=new ArrayList(20);
    private Image imageClock1;
    private Image imageClock2;

	public static m_GameControllerScript mGameController;

    private Animator messageBubble;
    private Text messageBubbleText;

    private int currentImage = 1;

    public float playerTime;

    public GameObject cueController;
    private CueController cueControllerScript;
    public GameObject shotPowerObject;
    private ShotPowerScript shotPowerScript;

    private float messageTime = 0;
    private AudioSource[] audioSources;
    private bool timeSoundsStarted = false;

    int loopCount = 0;
//	bool ahmed=false;
    private float waitingOpponentTime = 0;
    // Use this for initialization
	public GameObject levelComptext , levelfailtxt;

		
	public GameObject poedBalsExtra;

	public GameObject[] tables;


	public Text RemainingLifeTxt;


	public GameObject LevelPausePanel ,targetLines;

	public Animator pausebtn ,accuracy ,ballpots;


	public GameObject heartBeating, heartBroken ,tutorialtable;

	public Material[] line3Materials;
	

	public GameObject ballHide;

	public GameObject deActivatedobj;

	void Awake()
	{
		Time.timeScale = 2f;
//		Time.timeScale = 1.0f;
//
//		Time.fixedDeltaTime = 0.02f * Time.timeScale;

//		Time.timeScale = 1f;
//	
//		GlobalGameHandler.TableNumber=8;

		ballHide.SetActive (false);
		tutorialtable.SetActive (false);
		GameManager.Instance.roomOwner = true;
		GameManager.Instance.offlineMode = true;
	
		if (GlobalGameHandler.TableNumber ==5||GlobalGameHandler.TableNumber==4) {
			poedBalsExtra.SetActive (true);
		} else {
			poedBalsExtra.SetActive (false);
		}
		foreach (GameObject obj in tables) {
			obj.SetActive (false);
		}
		if (PlayerPrefs.GetInt ("firsttime") == 0) {
			tutorialtable.SetActive (true);

		} else {
			
			tables [GlobalGameHandler.TableNumber].SetActive (true);
		}
			Positions = GameObject.FindGameObjectsWithTag ("ballposition");
		White_Ball_Pos = GameObject.FindGameObjectWithTag ("WhiteBallPos");

		White_Ball.transform.position = White_Ball_Pos.transform.position;
		White_Ball.transform.rotation = White_Ball_Pos.transform.rotation;
	}

	void Start() {

		
		GlobalGameHandler.nextlevel = false;
		GlobalGameHandler.levelcomplete = false;
		GlobalGameHandler.levelfail = false;
//		GlobalGameHandler.RemaningLifes= 5;
		GlobalGameHandler.RemaningLifes = MMainMenu.instance.Remaining_Lifes[GlobalGameHandler.levelNumer];
		LevelPausePanel.SetActive (false);
		GlobalGameHandler.isShoot = false;
		GlobalGameHandler.WhiteBallPoted = false;
		GlobalGameHandler.isballPocket = false;
		heartBeating.SetActive (true);
		heartBroken.SetActive (false);

		levelComptext.SetActive (false);
		levelfailtxt.SetActive (false);
		GlobalGameHandler.isBumper = false;
		GlobalGameHandler.LevelCompleteFlag = false;
		GlobalGameHandler.ballNumber = 0;
		mGameController = GetComponent<m_GameControllerScript> ();

        audioSources = GetComponents<AudioSource>();
        shotPowerScript = shotPowerObject.GetComponent<ShotPowerScript>();
        cueControllerScript = cueController.GetComponent<CueController>();
        playerTime = GameManager.Instance.playerTime;
        imageClock1 = GameObject.Find("AvatarClock1").GetComponent<Image>();
        imageClock2 = GameObject.Find("AvatarClock2").GetComponent<Image>();

        messageBubble = GameObject.Find("MessageBubble").GetComponent<Animator>();
        messageBubbleText = GameObject.Find("BubbleText").GetComponent<Text>();
        


        playerTime = playerTime * Time.timeScale;


        if (GameManager.Instance.roomOwner) {
            showMessage(StaticStrings.youAreBreaking);
        } else {
            showMessage(GameManager.Instance.nameOpponent + " " + StaticStrings.opponentIsBreaking);
        }

        if (!GameManager.Instance.roomOwner)
            currentImage = 2;


		RemainingLifeTxt.text = GlobalGameHandler.RemaningLifes.ToString();
	
    }


	public void OnPaueBtnClic()
	{
		LevelPausePanel.SetActive (true);
		hideAllControls ();
	}

	public void OnResumeBtnClic()
	{

		
		ShowAllControls ();
		LevelPausePanel.SetActive (false);
		print(" new time "+Time.timeScale);
		Time.timeScale = 2f;
//		Time.timeScale = 1f;


	}

	public void OnRestartClick()
	{
		GameManager.Instance.resetAllData();
		SceneManager.LoadScene ("GamePlayScene");

	}

	public void OnNextBtnClick()
	{
		GlobalGameHandler.nextlevel = true;
		GameManager.Instance.resetAllData();
		SceneManager.LoadScene ("Menu");
	}

	public void OnHomeBtnClick()
	{
		GameManager.Instance.resetAllData();
		SceneManager.LoadScene ("Menu");

	}

	public void levelComplete()
	{
		if(PlayerPrefs.GetInt("firsttime")!=0){
		if(!GlobalGameHandler.levelfail)
		StartCoroutine(LevelCompeteCor());

			if (GlobalGameHandler.RemaningLifes >= 6) {
			
				PlayerPrefs.SetInt ("victory",1);
			}

		print (GlobalGameHandler.levelNumer);
			if (PlayerPrefs.GetInt ("Levels") <= GlobalGameHandler.levelNumer) {

				PlayerPrefs.SetInt ("Levels", GlobalGameHandler.levelNumer + 1);

				PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins" + 100));

			}	}

	}

	public void levelFail()
	{
		GlobalGameHandler.levelfail = true;
		StartCoroutine( Levelfail());


	}
    // Update is called once per frame
    void Update() {
	    
		if (GlobalGameHandler.LevelCompleteFlag) {
		
			GlobalGameHandler.LevelCompleteFlag = false;
			levelComplete ();

		}
    }
	IEnumerator Levelfail()
	{
		yield return new WaitForSeconds (0.5f);
		GameManager.Instance.resetAllData();
		levelfailtxt.SetActive (true);

		hideAllControls ();

	}

	IEnumerator LevelCompeteCor()
	{
		
		yield return new WaitForSeconds (5f);
		GameManager.Instance.resetAllData();
		levelComptext.SetActive (true);
	}


    private void updateClock() {
        float minus;
        if (currentImage == 1) {
            playerTime = GameManager.Instance.playerTime;
            if (GameManager.Instance.offlineMode)
                playerTime = GameManager.Instance.playerTime + GameManager.Instance.cueTime;
            minus = 1.0f / playerTime * Time.deltaTime;

            imageClock1.fillAmount -= minus;

            if (imageClock1.fillAmount < 0.25f && !timeSoundsStarted) {
//                audioSources[0].Play();
                timeSoundsStarted = true;
            }

            if (imageClock1.fillAmount == 0) {
	            audioSources[0].Stop();
				print ("1");

					GameManager.Instance.stopTimer = true;
					shotPowerScript.resetCue ();
				
				if (!GameManager.Instance.offlineMode) {
//					PhotonNetwork.RaiseEvent (9, cueControllerScript.cue.transform.position, true, null);
					print ("2");
				} else {
					print ("3");
//					if (ahmed) {
						GameManager.Instance.wasFault = true;
						GameManager.Instance.cueController.setTurnOffline (true);
//					}
				}

					print ("4");
					GameManager.Instance.cueController.ShotPowerIndicator.deactivate ();
					GameManager.Instance.cueController.ShotPowerIndicator.resetCue ();

					GameManager.Instance.cueController.cueSpinObject.GetComponent<SpinController> ().hideController ();

					GameManager.Instance.cueController.whiteBallLimits.SetActive (false);
					GameManager.Instance.ballHand.SetActive (false);

					showMessage ("You " + StaticStrings.runOutOfTime);

                if (!GameManager.Instance.offlineMode) {
                    cueControllerScript.setOpponentTurn();
					print ("5");
                }

            }

		} else  {
//			print ("6");
//            Debug.Log(GameManager.Instance.opponentCueTime);
            playerTime = GameManager.Instance.playerTime;
            if (GameManager.Instance.offlineMode)
                playerTime = GameManager.Instance.playerTime + GameManager.Instance.opponentCueTime;
            minus = 1.0f / playerTime * Time.deltaTime;
            imageClock2.fillAmount -= minus;

            if (GameManager.Instance.offlineMode && imageClock2.fillAmount < 0.25f && !timeSoundsStarted) {
//                audioSources[0].Play();
                timeSoundsStarted = true;
            }

            if (imageClock2.fillAmount == 0) {
                GameManager.Instance.stopTimer = true;

                if (GameManager.Instance.offlineMode) {
                    showMessage("You " + StaticStrings.runOutOfTime);
                } else {
                    showMessage(GameManager.Instance.nameOpponent + " " + StaticStrings.runOutOfTime);
                }

                if (GameManager.Instance.offlineMode) {
                    GameManager.Instance.wasFault = true;
                    GameManager.Instance.cueController.setTurnOffline(true);
                }
            }
        }

    }

    public void showMessage(string message) {
	    
        float timeDiff = Time.time - messageTime;


        if (timeDiff > 6) {
            messageBubbleText.text = message;
            messageBubble.Play("ShowBubble");
            if (!message.Contains(StaticStrings.waitingForOpponent))
                Invoke("hideBubble", 5.0f);
            else {
                waitingOpponentTime = StaticStrings.photonDisconnectTimeout;
                StartCoroutine(updateMessageBubbleText());
            }
            messageTime = Time.time;
        } else {
//            Debug.Log("Show message with delay");
            StartCoroutine(showMessageWithDelay(message, (6.0f - timeDiff) / 1.0f));
        }
    }

    public void hideBubble() {
        messageBubble.Play("HideBubble");
    }

    IEnumerator showMessageWithDelay(string message, float delayTime) {
        yield return new WaitForSeconds(delayTime);

        messageBubbleText.text = message;

        messageBubble.Play("ShowBubble");
        if (!message.Contains(StaticStrings.waitingForOpponent))
            Invoke("hideBubble", 5.0f);
        else {
            waitingOpponentTime = StaticStrings.photonDisconnectTimeout;
            StartCoroutine(updateMessageBubbleText());
        }
        messageTime = Time.time;

    }

    public IEnumerator updateMessageBubbleText() {
        yield return new WaitForSeconds(1.0f * 2);
        waitingOpponentTime -= 1;
        if (!GameManager.Instance.opponentDisconnected) {
            if (!messageBubbleText.text.Contains("disconnected from room"))
                messageBubbleText.text = StaticStrings.waitingForOpponent + " " + waitingOpponentTime;
        }
        if (waitingOpponentTime > 0 && !GameManager.Instance.opponentActive && !GameManager.Instance.opponentDisconnected) {
            StartCoroutine(updateMessageBubbleText());
        }
    }

    public void stopSound() {
        audioSources[0].Stop();
    }

    public void resetTimers(int currentTimer, bool showMessageBool) {

        stopSound();
        timeSoundsStarted = false;
        imageClock1.fillAmount = 1;
        imageClock2.fillAmount = 1;

        this.currentImage = currentTimer;

        if (GameManager.Instance.offlineMode) {
            if (showMessageBool) {

                if (currentTimer == 2) {
                    showMessage(StaticStrings.offlineModePlayer2Name + " turn");
                } else {
                    showMessage(StaticStrings.offlineModePlayer1Name + " turn");
                }

            }

        } else {
            if (currentTimer == 1 && showMessageBool) {
                showMessage("It's your turn");
            }
        }

        GameManager.Instance.stopTimer = false;
    }


	public void hideAllControls()
	{

		deActivatedobj.SetActive(false);
		ballHide.SetActive (false);
		ShotPowerScript.instance.anim.Play("ShotPowerAnimation");
		ShotPowerScript.instance.cueMain.GetComponent<Renderer>().enabled = false;
       	pausebtn.Play ("pausebtnAnim");

		accuracy.Play ("accuracyAnim");
	

		ballpots.Play ("ballpotOut");

		targetLines.SetActive (false);

	}

	public void ShowAllControls()
	{
		deActivatedobj.SetActive(true);
		cueControllerScript.targetLine.SetActive(true);
		cueControllerScript.cue.transform.position = new Vector3(GameManager.Instance.balls[0].transform.position.x, GameManager.Instance.balls[0].transform.position.y, cueControllerScript.cue.transform.position.z);
		cueControllerScript.cue.GetComponent<Renderer>().enabled = true;
	    pausebtn.Play ("pausebtnanimOf");
		accuracy.Play ("AccuracyBackAnim");
	
		ballpots.Play ("ballpotIn");
		ShotPowerScript.instance.anim.Play("MakeVisible");
		targetLines.SetActive (true);


	}

	public void ballHideOn()
	{
	
		StartCoroutine (BallHideOn());
	}


	IEnumerator BallHideOn()
	{
		yield return new WaitForSeconds (1.6f);
		ballHide.SetActive (true);
	

	}



}
