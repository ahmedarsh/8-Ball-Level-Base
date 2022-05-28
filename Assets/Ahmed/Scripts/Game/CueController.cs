using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CueController : MonoBehaviour
{

    public GameObject cue;
    public GameObject posDetector;

    public UnityEngine.UI.Text text;

    private Rigidbody myRigidbody;
    public bool isServer;
    private LineRenderer lineRenderer;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;
    public GameObject ShotPowerInd;
    public ShotPowerScript ShotPowerIndicator;
    public GameObject targetLine;
    private GameObject circle;
    private RaycastHit hitInfo = new RaycastHit();
    private Ray ray = new Ray();
    public LayerMask layerMask;
    private Vector3 endPosition;
    private float radius;
    private bool displayTouched = false;
    private bool isFirstTouch = false;
    public int steps = 0;
    public bool shouldShot = false;
    private bool firstShotDone = false;
    public int fixedCounter = 0;
    public bool startShoot = false;
    public bool dataSent = false;
    private bool canCount;
    public bool ballsInMovement = false;
    public bool canRun = false;
    private ArrayList multiplayerDataArray = new ArrayList();
    private bool startMovement;
    private int counterFixPositionMulti = 0;
    private Vector3 multiFirstShotPower;
    private Vector3 multiFirstShotDirection;
    private Vector3 circleShotPos = Vector3.zero;
    private Vector3 initShotPos = Vector3.zero;
    private bool firstShot = true;
    int updateCount = 0;
    private Vector3 shotDirection;
    private bool ballCollideFirst;
    public Vector3 trickShotAdd = Vector3.zero;
    public bool spinShowed;
    public GameObject cueSpinObject;
    public bool opponentShotStart = false;
    public bool opponentBallsStoped = false;
    public bool shotMyTurnDone = false;
    private bool raisedSixEvent = false;
    public GameObject gameControllerObject;
    private m_GameControllerScript mGameControllerScript;
    private bool movingWhiteBall = false;
    public GameObject whiteBallLimits;
    public GameObject ballHand;
    public GameObject youWonMessage;
    [FormerlySerializedAs("potedBallsGUI")] public m_PotedBallsGUIController mPotedBallsGUI;
    public GameObject whiteBallTrigger;
    public GameObject[] callPocketsButtons;
    public GameObject[] calledPockets;
    private bool canShowControllers = true;
    public GameObject prizeText;
    private AudioSource[] audioSources;
    public GameObject audioController;
    public GameObject invitiationDialog;
    public GameObject wrongBall;
    public Sprite[] cueTextures;
    public GameObject chatButton;
    public bool opponentResumed = false;
    private Vector3 touchMousePos;

	public bool hideControls;

	public GameObject cueSprite;

	public List <Vector3> last_positions;

//	public Vector3 CurrentPos,LastPos;


    void Start()
    {

		last_positions = new List<Vector3> ();
//		CurrentPos = gameObject.transform.position;
//		LastPos = CurrentPos;

//		print ("CurrentPos  => "+CurrentPos+"  Last Pos =>  "+LastPos);

        if(GameManager.Instance.roomOwner) {
			changeCueImage(GlobalGameHandler.CueNumber);
        } else {
			changeCueImage(GlobalGameHandler.CueNumber);
        }

        if(GameManager.Instance.offlineMode) {
            chatButton.SetActive(false);
        }
        


        Debug.Log("Minus Coins");

        

//        if(!GameManager.Instance.offlineMode)
//            GameManager.Instance.playfabManager.addCoinsRequest(-GameManager.Instance.payoutCoins);


        GameManager.Instance.audioSources = audioController.GetComponents<AudioSource>();
        audioSources = GetComponents<AudioSource>();

        GameManager.Instance.iWon = false;
        GameManager.Instance.iLost = false;


        setPrizeText();

        mPotedBallsGUI = GameObject.Find("PotedBallsGUI").GetComponent<m_PotedBallsGUIController>();
        GameManager.Instance.ballHand = ballHand;
        GameManager.Instance.cueController = this;
        ShotPowerIndicator = ShotPowerInd.GetComponent<ShotPowerScript>();
        mGameControllerScript = gameControllerObject.GetComponent<m_GameControllerScript>();
        GameManager.Instance.mGameControllerScript = mGameControllerScript;
		ballHand.transform.position = mGameControllerScript.White_Ball_Pos.transform.position;
		ballHand.transform.position= new Vector3(ballHand.transform.position.x,ballHand.transform.position.y,-0.7218f);
        circle = GameObject.Find("Circle");
        targetLine = GameObject.Find("TargetLine");
        lineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
//        lineRenderer2 = GameObject.Find("Line2").GetComponent<LineRenderer>();
        lineRenderer3 = GameObject.Find("Line3").GetComponent<LineRenderer>();


        // Configure when target lines will be invisible - table number
        if (GameManager.Instance.tableNumber > 6)
        {
            GameManager.Instance.showTargetLines = false;
        }
        else
        {
            GameManager.Instance.showTargetLines = true;
        }


        // Configure when calling pocket for black ball is required - table number
        if (GameManager.Instance.tableNumber > 1)
        {
            GameManager.Instance.callPocketBlack = true;
        }
        else
        {
            GameManager.Instance.callPocketBlack = false;
        }


        // Configure when calling pocket for all balls is required - table number
        if (GameManager.Instance.tableNumber > 3)
        {
            GameManager.Instance.callPocketAll = true;
        }
        else
        {
            GameManager.Instance.callPocketAll = false;
        }

        if (!GameManager.Instance.showTargetLines)
        {
//            lineRenderer2.enabled = false;
            lineRenderer3.enabled = false;
        }

        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;


        myRigidbody = GetComponent<Rigidbody>();

        // Set cue position to ball position
        Vector3 cueInitialPos = transform.position;
        cueInitialPos.x = cueInitialPos.x;
        cueInitialPos.z = cue.transform.position.z;
        cue.transform.position = cueInitialPos;

        isServer = false;




        GameManager.Instance.audioSources[1].Play();



        canCount = false;

		if (GameManager.Instance.roomOwner)
		{
//			ballHand.SetActive(true);


			print ("in room owner hand");
			GameManager.Instance.hasCueInHand = true;
			isServer = true;
			opponentBallsStoped = true;
			GameManager.Instance.audioSources[0].Play();
		}


		LockZPosition.isStiger = false;
        //drawTargetLines();

		ballHand.SetActive(false);
    }

    /// <summary>
    /// Callback sent to all game objects when the player pauses.
    /// </summary>
    /// <param name="pauseStatus">The pause state of the application.</param>
    void OnApplicationPause(bool pauseStatus)
    {
//        if (pauseStatus) {
//            PhotonNetwork.RaiseEvent(151, 1, true, null);
//
//            PhotonNetwork.SendOutgoingCommands();
//            Debug.Log("Application pause");
//        } else {
//            PhotonNetwork.RaiseEvent(152, 1, true, null);
//            PhotonNetwork.SendOutgoingCommands();
//            Debug.Log("Application resume");
//        }
    }

    public void changeCueImage(int index) {
        cue.GetComponent<SpriteRenderer>().sprite = cueTextures[index];
		cueSprite.GetComponent<SpriteRenderer>().sprite= cueTextures[index];

    }

    private void setPrizeText()
    {

        

        int prizeCoins = GameManager.Instance.payoutCoins * 2;

        if (prizeCoins >= 1000)
        {
            if (prizeCoins >= 1000000)
            {
                if (prizeCoins % 1000000.0f == 0)
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000000.0f).ToString("0") + "M";

                }
                else
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000000.0f).ToString("0.0") + "M";

                }

            }
            else
            {
                if (prizeCoins % 1000.0f == 0)
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000.0f).ToString("0") + "k";
                }
                else
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000.0f).ToString("0.0") + "k";
                }

            }
        }
        else
        {
            prizeText.GetComponent<Text>().text = prizeCoins + "";
        }

        if(GameManager.Instance.offlineMode) {
            prizeText.GetComponent<Text>().text = "Practice";
        }
    }




//    void Awake()
//    {
//        PhotonNetwork.OnEventCall += this.OnEvent;
//
//    }
//
//    public void removeOnEventCall()
//    {
//        PhotonNetwork.OnEventCall -= this.OnEvent;
//    }


    void Update()
    {

        

        if (!spinShowed && canCheckAnotherCueRotation)
            StartCoroutine(rotateCue());
        

        //

        if(!isServer) {
            drawTargetLines();
        }



        if (Input.GetMouseButtonDown(0))
        {
            steps = 0;
            isFirstTouch = true;
            displayTouched = true;
            touchMousePos = Vector3.zero;
        }


        if (Input.GetMouseButtonUp(0))
        {
            //isServer = true;
            canRun = true;
            displayTouched = false;
        }


//		if (this.gameObject.GetComponent<Rigidbody> ().velocity.magnitude >= 1.0f) {
//			lineRenderer.enabled = false;
//			lineRenderer3.enabled = false;
//
//		}

    }

    void FixedUpdate()
    {
        checkFirstCollision();
        shotOpponentTurn();
        shotMyTurn();

    }

    private void checkFirstCollision()
    {
        //have we moved more than our minimum extent? 

        if (ballCollideFirst && gameObject.layer == 11)
        {
            RaycastHit hitInfo;


            if (Physics.SphereCast(initShotPos, radius, shotDirection, out hitInfo, Vector3.Distance(initShotPos, transform.position), layerMask.value))
            {

                if (!hitInfo.transform.tag.Equals(transform.tag))
                {

                    if (!hitInfo.collider)
                        return;

                    if (hitInfo.collider.isTrigger)
                    {
                        //hitInfo.collider.SendMessage ("OnTriggerEnter", myCollider);

                    }

                    if (!hitInfo.collider.isTrigger)
                    {
                        Debug.Log("fix pos");
                        //
                        //						Vector3 vel = myRigidbody.velocity;
                        //						Vector3 angVel = myRigidbody.angularVelocity;
                        //						myRigidbody.Sleep ();

                        Vector3 fixedPos = circleShotPos;
                        fixedPos.z = transform.position.z;
                        myRigidbody.transform.position = fixedPos;
                        gameObject.layer = 8;
				
                        //						myRigidbody.velocity = vel;
                        //						myRigidbody.angularVelocity = angVel;
                    }

                }
            }
        }
    }

    public void callPocket()
    {
        GameManager.Instance.mGameControllerScript.showMessage(StaticStrings.callPocket);
        for (int i = 0; i < callPocketsButtons.Length; i++)
        {
            callPocketsButtons[i].SetActive(true);
            callPocketsButtons[i].GetComponent<Animator>().Play("CallPocketShowAnimation");
        }
    }

    public void hidePocketButtons()
    {
        for (int i = 0; i < callPocketsButtons.Length; i++)
        {
            callPocketsButtons[i].SetActive(false);
        }
    }

    private void shotMyTurn()
    {

        //		if (!initShotPos.Equals (Vector3.zero) && !circleShotPos.Equals (Vector3.zero)) {
        //			if (Vector3.Distance (transform.position, initShotPos) > Vector3.Distance (initShotPos, circleShotPos) * 0.5f) {
        //				Debug.Log ("Aa");
        //				//myRigidbody.Sleep ();
        //				gameObject.layer = 8;
        //				//GetComponent <SphereCollider> ().isTrigger = false;
        //				Vector3 fixedPos = circleShotPos;
        //				fixedPos.z = transform.position.z;
        //				myRigidbody.transform.position = fixedPos;
        //				initShotPos = Vector3.zero;
        //				circleShotPos = Vector3.zero;
        //			}
        //		}

        // Shot when its your turn

        if (GameManager.Instance.opponentActive && !GameManager.Instance.stopTimer && shouldShot && steps > 0 && isServer)
        {
            shotMyTurnDone = true;
            //GameManager.Instance.iWon = true;

            for (int i = 0; i < GameManager.Instance.balls.Length; i++)
            {
                GameManager.Instance.balls[i].GetComponent<Rigidbody>().maxAngularVelocity = 150;
            }

            // GameManager.Instance.whiteBall.GetComponent<Rigidbody>().maxAngularVelocity = 150;
            // Debug.Log(GameManager.Instance.whiteBall.GetComponent<Rigidbody>().maxAngularVelocity + " MAX ANG");

            startShoot = false;

            opponentBallsStoped = false;

            myRigidbody.velocity = Vector3.zero;
            Vector3 dir = transform.position - posDetector.transform.position;
            dir = dir.normalized;
            dir.z = 0;

            Vector3 trickShot = transform.position;
            trickShot = trickShot + trickShotAdd;
            //			trickShot.z -= 0.01f;
            //			trickShot.z += 0.5f;
            //			trickShot.z -= 1.0f;

            //			float multipleBy = 0.7f;
            //			if (firstShot) {
            //				multipleBy = 1f;
            //				firstShot = false;
            //			} 

            float multipleBy = 0.03f + (GameManager.Instance.cuePower + 1) * 0.001f;



            Vector3 shotPower = dir * steps * multipleBy;

            if (ballCollideFirst)
                gameObject.layer = 11;

            initShotPos = transform.position;
            circleShotPos = circle.transform.position;
            shotDirection = dir;


            float velSum = Mathf.Abs(shotPower.x) + Mathf.Abs(shotPower.y) + Mathf.Abs(shotPower.z);
            audioSources[audioSources.Length - 1].volume = velSum / 1.8f;
            audioSources[audioSources.Length - 1].Play();

            mGameControllerScript.stopSound();

            

            myRigidbody.AddForceAtPosition(shotPower, trickShot, ForceMode.Impulse);
            ballsInMovement = true;


            GameManager.Instance.hasCueInHand = false;



            byte evCode = 0;
            Vector3[] content = new Vector3[] { shotPower, trickShot };
//            if (!GameManager.Instance.offlineMode)
//                PhotonNetwork.RaiseEvent(evCode, content, true, null);

            targetLine.SetActive(false);

            GameManager.Instance.stopTimer = true;

            //cue.GetComponent<Renderer> ().enabled = false;
            dataSent = true;
            shouldShot = false;


        }
    }


    private void shotOpponentTurn()
    {

        // Begin movement after multiplayer data is received
        if (startMovement)
        {
            if (!firstShotDone)
            {
				print ("Not first Shoot done");
                //                myRigidbody.AddForceAtPosition (multiFirstShotPower, multiFirstShotDirection, ForceMode.Impulse);
                firstShotDone = true;
                float velSum = Mathf.Abs(multiFirstShotPower.x) + Mathf.Abs(multiFirstShotPower.y) + Mathf.Abs(multiFirstShotPower.z);
                audioSources[audioSources.Length - 1].volume = velSum / 1.8f;
                audioSources[audioSources.Length - 1].Play();
                mGameControllerScript.stopSound();
            }
            else
            {
                //if (fixedCounter == 1) {
                if (fixedCounter == 1)
                {
					print ("fixedCounter");

                    if (multiplayerDataArray.Count > 0)
                    {
						print ("multiplayerDataArra");

                        Vector3[] balls = (Vector3[])multiplayerDataArray[0];
                        multiplayerDataArray.RemoveAt(0);
                        GameObject spawnBalls = GameObject.Find("SpawnBalls");
                        m_SpawnBallsScript script = spawnBalls.GetComponent<m_SpawnBallsScript>();
                        for (int i = 0; i <= balls.Length - 3; i += 3)
                        {

                            if (!isServer /*|| shotMyTurnDone == false*/)
                            {
                                float dist = Vector3.Distance(balls[i], script.balls[i / 3].transform.position);

                                script.balls[i / 3].GetComponent<Rigidbody>().velocity = balls[i + 1];
                                script.balls[i / 3].GetComponent<Rigidbody>().angularVelocity = balls[i + 2];
                                script.balls[i / 3].transform.position = balls[i];
                            }

                        }

                    }
                    else
                    {

                        startMovement = false;

                    }


                }

                if (canCount)
                    fixedCounter++;
                if (fixedCounter > 15)
                    fixedCounter = 0;
            }
        }
    }


//    void OnDestroy()
//    {
//        PhotonNetwork.OnEventCall -= this.OnEvent;
//    }

    // Multiplayer data received
    private void OnEvent(byte eventcode, object content, int senderid)
    {

        Debug.Log("isServer: " + isServer + "  code: " + eventcode);
        if (!isServer && eventcode == 0)
        {
            Vector3[] data = (Vector3[])content;
            multiFirstShotPower = data[0];
            multiFirstShotDirection = data[1];
            firstShotDone = false;
            multiplayerDataArray.Clear();
        }
        else if (!isServer && eventcode == 1)
        {
            Debug.Log("1");
            Vector3[] balls = (Vector3[])content;

            cue.GetComponent<Renderer>().enabled = true;

            GameObject spawnBalls = GameObject.Find("SpawnBalls");
            m_SpawnBallsScript script = spawnBalls.GetComponent<m_SpawnBallsScript>();

            //transform.position = balls [0];

            Debug.Log("Received positions x-" + balls[0].x + "  y-" + balls[0].y);

            for (int i = 0; i < balls.Length; i++)
            {
                script.balls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                script.balls[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //script.balls [i].transform.position = Vector3.Lerp(balls [i], script.balls [i].transform.position, 0.5f);
                script.balls[i].transform.position = balls[i];
                //script.setBallTransform (i, balls[i].transform.position);
            }
        }
        else if (!isServer && eventcode == 2)
        {
            Debug.Log("2");



            cue.GetComponent<Renderer>().enabled = false;



            GameObject spawnBalls = GameObject.Find("SpawnBallsScript");
            m_SpawnBallsScript script = spawnBalls.GetComponent<m_SpawnBallsScript>();
            Vector3[] balls = (Vector3[])content;
            for (int i = 0; i <= balls.Length - 3; i += 3)
            {

                float dist = Vector3.Distance(balls[i], script.balls[i / 3].transform.position);
                script.balls[i / 3].GetComponent<Rigidbody>().velocity = balls[i + 1];
                script.balls[i / 3].GetComponent<Rigidbody>().angularVelocity = balls[i + 2];
                script.balls[i / 3].transform.position = balls[i];

            }


        }
        else if (!isServer && eventcode == 4)
        {
            Debug.Log("3");



            cue.GetComponent<Renderer>().enabled = true;



            GameObject spawnBalls = GameObject.Find("SpawnBallsScript");
            m_SpawnBallsScript script = spawnBalls.GetComponent<m_SpawnBallsScript>();
            Vector3[] balls = (Vector3[])content;


            multiplayerDataArray = (ArrayList)content;

        }
        else if (!isServer && eventcode == 5)
        {
            multiplayerDataArray.Add((Vector3[])content);
            counterFixPositionMulti++;
            GameManager.Instance.stopTimer = true;

            if (counterFixPositionMulti == 20)
            {
                Debug.Log("AAAA");
                raisedSixEvent = false;
                startMovement = true;
                opponentShotStart = true;
                cue.GetComponent<Renderer>().enabled = false;
                targetLine.SetActive(false);
                ballsInMovement = true;
                canCount = true;
            }
        }
        else if (!isServer && eventcode == 6)
        {

            counterFixPositionMulti = 0;
            multiplayerDataArray.Add((Vector3[])content);


        }
        else if (!isServer && eventcode == 7)
        { // Opponent rotated cue
            cue.transform.rotation = (Quaternion)content;
        }
        else if (!isServer && eventcode == 8)
        { // Shot power - move main cue
            cue.transform.position = (Vector3)content;
        }
        else if (!isServer && eventcode == 9)
        { // My turn - show cue and lines


            cue.transform.position = (Vector3)content;


            StartCoroutine(setMyTurn(true));


        }
        else if (!isServer && eventcode == 12)
        { // Opponents turn - no fault


            cue.transform.position = (Vector3)content;
            setOpponentTurn();

        }
        else if (isServer && eventcode == 10)
        { // Opponents balls stoped movement. Can show cue and lines

            

            checkShot();
        }
        else if (!isServer && eventcode == 11)
        { // Cue spin controller changed value
            cueSpinObject.GetComponent<SpinController>().changePositionOpponent((Vector3)content);
        }
        else if (eventcode == 13)
        { // Fault message
            string message = (string)content;

            //if (message.Equals (StaticStrings.potedCueBall)) {
            ballHand.SetActive(true);

			print (" hand on ");

            GameManager.Instance.hasCueInHand = true;
            //}

            if (message.Equals(StaticStrings.invalidStrike) || (!GameManager.Instance.ballsStriked && message.Equals(StaticStrings.faultCueBallDidntStrike)))
            {

                getNewWhiteBallPosition(false);
            }
            else
            {
                if (message.Equals(StaticStrings.potedCueBall))
                {


                    getNewWhiteBallPosition(true);
                }
            }

            Vector3 newBallPos = GameManager.Instance.whiteBall.transform.position;
            newBallPos.z = ballHand.transform.position.z;
            ballHand.transform.position = newBallPos;

            if (message.Contains("You"))
            {
                message = message.Replace("You", GameManager.Instance.nameOpponent);
            }

            GameManager.Instance.mGameControllerScript.showMessage(message);
            GameManager.Instance.faultMessage = "";
        }
        else if (eventcode == 14)
        { // Opponent moving white ball before strike - show limits
            whiteBallLimits.SetActive(true);
            HideAllControllers();
			print ("White ball limts");
        }
        else if (eventcode == 15)
        { // Opponent moving white ball
            transform.position = (Vector3)content;
        }
        else if (eventcode == 16)
        { // Opponent stoped moving white ball - hide limits
            whiteBallLimits.SetActive(false);
            ShowAllControllers();
			print ("White ball limts");
        }
        else if (eventcode == 17)
        { // Opponent moving white ball after strike - hide controllers
            HideAllControllers();
        }
        else if (eventcode == 18)
        { // Balls was striked correctly
            GameManager.Instance.ballsStriked = true;
        }
        else if (eventcode == 19)
        { // Opponent Won!
            HideAllControllers();
            GameManager.Instance.audioSources[3].Play();
            youWonMessage.SetActive(true);
            youWonMessage.GetComponent<YouWinMessageChangeSprite>().changeSprite();
            youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
            GameManager.Instance.iWon = false;
        }
        else if (eventcode == 20)
        { // Opponent Poted 8 ball - you won!
            HideAllControllers();
            GameManager.Instance.audioSources[3].Play();
            youWonMessage.SetActive(true);
            youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
            GameManager.Instance.iWon = true;
        }
        else if (!isServer && eventcode == 21)
        { // Player got types
            checkBallsTypes(false, false, (bool)content);
        }
        else if (!isServer && eventcode == 22)
        { // Opponent called pocket
            GameManager.Instance.mGameControllerScript.showMessage(GameManager.Instance.nameOpponent + " " + StaticStrings.opponentCalledPocket);
            calledPockets[(int)content].SetActive(true);
        }
        else if (eventcode == 151)
        {
            if(isServer)
                ShotPowerIndicator.anim.Play("ShotPowerAnimation");
            GameManager.Instance.opponentActive = false;
            GameManager.Instance.stopTimer = true;
            GameManager.Instance.mGameControllerScript.showMessage(StaticStrings.waitingForOpponent + " " + StaticStrings.photonDisconnectTimeout );
        }
        else if (eventcode == 152)
        {
            if(canShowControllers && isServer && !shotMyTurnDone)
                ShotPowerIndicator.anim.Play("MakeVisible");
            GameManager.Instance.opponentActive = true;

            if((isServer && !shotMyTurnDone) || !isServer)
                GameManager.Instance.stopTimer = false;
            GameManager.Instance.mGameControllerScript.hideBubble();
            opponentResumed = true;
        }
        
    }
    List<String> collides = new List<String>();
    void OnTriggerEnter(Collider other)
    {
		
		if (GameManager.Instance.hasCueInHand&&isServer&&other.gameObject.CompareTag ("bumper")) {
			print (GameManager.Instance.hasCueInHand+"   "+isServer);

			GlobalGameHandler.isBumper = true;
		
		}

		print(other.tag);
        if (other.tag.Contains("Ball"))
            collides.Add(other.tag);



    }


    public void checkShot() {
        
        if (GameManager.Instance.iWon)
            {

                if(!GameManager.Instance.wasFault) {
                    HideAllControllers();
                    GameManager.Instance.audioSources[3].Play();
                    youWonMessage.SetActive(true);
                    youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
//                    if (!GameManager.Instance.offlineMode)
//                        PhotonNetwork.RaiseEvent(19, null, true, null);
                } else {
                    HideAllControllers();
                    GameManager.Instance.audioSources[3].Play();
                    youWonMessage.SetActive(true);
                    youWonMessage.GetComponent<YouWinMessageChangeSprite>().changeSprite();
                    youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
//                    if (!GameManager.Instance.offlineMode)
//                        PhotonNetwork.RaiseEvent(20, null, true, null);
                }
                
            }
            else if (GameManager.Instance.iLost)
            {
                HideAllControllers();
                GameManager.Instance.audioSources[3].Play();
                youWonMessage.SetActive(true);
                youWonMessage.GetComponent<YouWinMessageChangeSprite>().changeSprite();
                youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
//                if (!GameManager.Instance.offlineMode)
//                    PhotonNetwork.RaiseEvent(20, null, true, null);
            }
            else
            {
                bool changedWhitePos = false;




                if (!GameManager.Instance.ballsStriked && !GameManager.Instance.validPot && GameManager.Instance.ballTouchBeforeStrike.Count < 4)
                {
                    GameManager.Instance.wasFault = true;
                    //if (GameManager.Instance.faultMessage.Length == 0)
                    GameManager.Instance.faultMessage = StaticStrings.invalidStrike;

                    //if (GameManager.Instance.ballTouchBeforeStrike.Count > 0) { // tutaj bylo 0
                    GameManager.Instance.ballsStriked = true;
//                    if (!GameManager.Instance.offlineMode)
//                        PhotonNetwork.RaiseEvent(18, 1, true, null);
                    //}
                    GameManager.Instance.ballTouchBeforeStrike.Clear();


                    if (!changedWhitePos)
                    {
                        //                        GameManager.Instance.whiteBall.GetComponent<Rigidbody> ().transform.position = new Vector3 (-2.9f, -0.69f, -0.24f);
                        //                        GameManager.Instance.whiteBall.GetComponent<Rigidbody> ().velocity = Vector3.zero;
                        //                        GameManager.Instance.whiteBall.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
                        //                        GameManager.Instance.whiteBall.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezePositionZ;
                        //                        GameManager.Instance.whiteBall.GetComponent<LockZPosition> ().ballActive = true;
                        //                        GameManager.Instance.whiteBall.SetActive (true);
                        getNewWhiteBallPosition(false);
                    }
                    //GameManager.Instance.whiteBall.transform.position = new Vector3 (-2.9f, -0.69f, -0.24f);


                }
                else
                {
                    //if (!GameManager.Instance.ballsStriked && GameManager.Instance.firstBallTouched) {
                    GameManager.Instance.ballsStriked = true;
                    if (!GameManager.Instance.offlineMode)
//                        PhotonNetwork.RaiseEvent(18, 1, true, null);
                    //}

                    checkBallsTypes(GameManager.Instance.wasFault, true, false);

                    if (!GameManager.Instance.firstBallTouched)
                    {
                        GameManager.Instance.wasFault = true;
                        if (GameManager.Instance.faultMessage.Length == 0)
                            GameManager.Instance.faultMessage = StaticStrings.faultCueBallDidntStrike;

                    }

                    if (GameManager.Instance.firstBallTouched && GameManager.Instance.ballTouchedBand == 0)
                    {
                        GameManager.Instance.wasFault = true;
                        if (GameManager.Instance.faultMessage.Length == 0)
                            GameManager.Instance.faultMessage = StaticStrings.invalidShotNoBandContact;

                    }

                    if (!GameManager.Instance.validPot)
                    {
                        GameManager.Instance.wasFault = true;
                        if (GameManager.Instance.faultMessage.Length == 0)
                            GameManager.Instance.faultMessage = "";
                    }

                    if (GameManager.Instance.wasFault && GameManager.Instance.faultMessage.Equals(StaticStrings.potedCueBall))
                    {

                        getNewWhiteBallPosition(true);

                    }
                }






                opponentBallsStoped = true;
                if(GameManager.Instance.offlineMode) {

                } else {
                    if (GameManager.Instance.wasFault)
                    {
                        if (GameManager.Instance.faultMessage.Length > 0)
                            GameManager.Instance.audioSources[2].Play();
                        setOpponentTurn();
                    }
                    else
                    {
                        StartCoroutine(setMyTurn(false));
                    }
                }

                
            }
    }

    private void getNewWhiteBallPosition(bool center)
    {
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().maxAngularVelocity = 150;

        // Debug.Log(GameManager.Instance.whiteBall.GetComponent<Rigidbody>().maxAngularVelocity + " MAX ANG");

        bool collides = true;


//        Vector3 newPos = new Vector3(-0f, -0.69f, -0.24f);

		Vector3 newPos = mGameControllerScript.White_Ball_Pos.transform.position;


        if (!center)
//            newPos = new Vector3(-2.9f, -0.69f, -0.24f);
			 newPos = mGameControllerScript.White_Ball_Pos.transform.position;
		print ("! ceneter ");
		

        while (collides)
        {
            collides = false;
            for (int i = 1; i < GameManager.Instance.balls.Length; i++)
            {
                if (Vector2.Distance(GameManager.Instance.balls[i].transform.position, newPos) < 0.38f)
                {
                    collides = true;
                    break;
                }
            }

            if (!collides)
            {
				GameManager.Instance.whiteBall.GetComponent<Rigidbody>().transform.position = mGameControllerScript.White_Ball_Pos.transform.position;
                GameManager.Instance.whiteBall.GetComponent<LockZPosition>().ballActive = true;
                GameManager.Instance.whiteBall.SetActive(true);
                Vector3 newBallPos1 = GameManager.Instance.whiteBall.transform.position;
                newBallPos1.z = ballHand.transform.position.z;
                ballHand.transform.position = newBallPos1;
				print ("! collides hand");
            }
            else
            {
                newPos.x -= 0.1f;
            }
        }

		GameManager.Instance.whiteBall.GetComponent<Rigidbody>().transform.position =  newPos = mGameControllerScript.White_Ball_Pos.transform.position;;
        GameManager.Instance.whiteBall.GetComponent<LockZPosition>().CancelInvoke();
    }

    public void checkBallsTypes(bool fault, bool wasServer, bool setSolid)
    {
        Debug.Log("Checking types " + GameManager.Instance.wasFault);
        if (!fault && !GameManager.Instance.playersHaveTypes)
        {
            Debug.Log("Checking types inside if");
            if (GameManager.Instance.noTypesPotedSolid && !GameManager.Instance.noTypesPotedStriped)
            {
                GameManager.Instance.playersHaveTypes = true;
                mPotedBallsGUI.potedBallsVisible = true;

                if(GameManager.Instance.offlineMode) {
                    if(GameManager.Instance.offlinePlayerTurn == 1) {
                        GameManager.Instance.offlinePlayer1OwnSolid = true;
                        GameManager.Instance.ownSolids = true;
                        mPotedBallsGUI.showPotedBalls(true);
                        
                        
                    } else {
                        GameManager.Instance.offlinePlayer1OwnSolid = false;
                        GameManager.Instance.ownSolids = true;
                        mPotedBallsGUI.showPotedBalls(false);
                        
                    }
                } else {
                    // You got solid balls
                    //if (GameManager.Instance.cueController.isServer)
                    if(wasServer)
                        mPotedBallsGUI.showPotedBalls(true);
                    else
                        mPotedBallsGUI.showPotedBalls(setSolid);
                        //potedBallsGUI.showPotedBalls(false);

//                    if (!GameManager.Instance.offlineMode && wasServer)
//                        PhotonNetwork.RaiseEvent(21, false, true, null);
                }
                
            }
            else if (!GameManager.Instance.noTypesPotedSolid && GameManager.Instance.noTypesPotedStriped)
            {
                GameManager.Instance.playersHaveTypes = true;
                mPotedBallsGUI.potedBallsVisible = true;

                if(GameManager.Instance.offlineMode) {
                    if(GameManager.Instance.offlinePlayerTurn == 1) {
                        GameManager.Instance.offlinePlayer1OwnSolid = false;
                        GameManager.Instance.ownSolids = false;
                        mPotedBallsGUI.showPotedBalls(false);
                    } else {
                        GameManager.Instance.ownSolids = false;
                        GameManager.Instance.offlinePlayer1OwnSolid = true;
                        mPotedBallsGUI.showPotedBalls(true);
                    }
                } else {
                    // You got striped balls
                    //if (GameManager.Instance.cueController.isServer)
                    if(wasServer)
                        mPotedBallsGUI.showPotedBalls(false);
                    else
                        mPotedBallsGUI.showPotedBalls(setSolid);
                        //potedBallsGUI.showPotedBalls(true);
//                    if (!GameManager.Instance.offlineMode && wasServer)
//                        PhotonNetwork.RaiseEvent(21, true, true, null);
                }
                
            }
        }

        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
    }

    void OnTriggerExit(Collider other)
    {
		if (other.gameObject.CompareTag ("bumper")) {
			print (GameManager.Instance.hasCueInHand+"   "+isServer);

			GlobalGameHandler.isBumper = false;

		}

		
        if (other.tag.Contains("Ball"))
            collides.Remove(other.tag);
    }




    void OnMouseDown()
    {
        if (GameManager.Instance.hasCueInHand)
        {
            ballHand.SetActive(false);
            HideAllControllers();
            movingWhiteBall = true;
//			print ("White ball limts");
            if (!GameManager.Instance.ballsStriked)
            {
//				print ("White ball limts");
//                whiteBallLimits.SetActive(true);
//                if (!GameManager.Instance.offlineMode)
//                    PhotonNetwork.RaiseEvent(14, 1, true, null);
//            }
//            else
//            {
////                if (!GameManager.Instance.offlineMode)    
////                    PhotonNetwork.RaiseEvent(17, 1, true, null);
            }
        }
    }

    Vector3 lastCorrectPosition;

//	void OnTriggerEnter(Collider col)
//	{
//		print (GameManager.Instance.hasCueInHand + " " + isServer);
//		if (GameManager.Instance.hasCueInHand && isServer) {
//			print ("in collidion");
//			if (col.gameObject.CompareTag ("bumper")) {
//				print ("in collidion");
//			}
//		}
//	}
//	void OnTriggerExit(Collider col){
		
//		if (GameManager.Instance.hasCueInHand && isServer) {
//			print ("Exit collidion");
//			if (col.gameObject.CompareTag ("bumper")) {
//				print ("exit collidion");
//			}
//		}
//	}


    void OnMouseDrag()
    {
        //if (collides.Count == 0) {
        if (GameManager.Instance.hasCueInHand && isServer)
        {
			

//			print (GameManager.Instance.hasCueInHand + " " + isServer);
            bool canMove = true;
            Vector3 collPos = Vector3.zero;
            float distance = 0.0f;
//			print ("White ball limts");
//            Debug.Log(Vector2.Distance(GameManager.Instance.balls[1].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            for (int i = 1; i < GameManager.Instance.balls.Length; i++)
            {
                if (Vector2.Distance(GameManager.Instance.balls[i].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.38f)
                {
//					print ("White ball limts");
                    canMove = false;
                    collPos = GameManager.Instance.balls[i].transform.position;
                    distance = Vector2.Distance(GameManager.Instance.balls[i].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    break;
                }
            }

			if (canMove) {
				
//				print ("canmove");
				float distance_to_screen = Camera.main.WorldToScreenPoint (gameObject.transform.position).z;
				Vector3 pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));


				float newX = transform.position.x;
				float newY = transform.position.y;

				if (GlobalGameHandler.TableNumber == 1 || GlobalGameHandler.TableNumber == 0 || GlobalGameHandler.TableNumber == 6 || GlobalGameHandler.TableNumber == 7) {
					


					if (GameManager.Instance.ballsStriked) {
						//					print ("White ball stricked");


						if (pos_move.x > -5.165136f && pos_move.x < 5.202874f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -5.165136f) {
								newX = -5.165136f;
								//							print ("White ball limts");
							} else {
								newX = 5.202874f;
								//							print ("White ball limts");
							}
						}
							

						if (pos_move.y > -3.097622f && pos_move.y < 1.804678f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -3.097622f) {

								newY = -3.097622f;


							} else {
								newY = 1.804678f;

							}
						}


						//x movment comment by me
					} else {


//								if (pos_move.x > -5.165136f && pos_move.x < 5.202874f) 
//								{
//									//						print ("White ball limts");
//									newX = pos_move.x;
//								}
//								else
//								{
//									//						print ("White ball limts");
//									if (pos_move.x < -5.165136f)
//									{
//										//							print ("White ball limts");
//										newX = -5.165136f;
//									}
//									else
//									{
//										//                            newX = -2.735605f;
//
//										newX = 5.202874f;
//
//										//							print ("White ball limts");
//									}
//								}
						//x movment comment by me
					}

//						if (pos_move.y > -3.097622f && pos_move.y < 1.804678f) {
//
//
//							newY = pos_move.y;
//						}
//
//						else {
//						
//							if (pos_move.y < -3.097622f) {
//								
//									newY = -3.097622f;
//
//
//							} else {
//							newY = 1.804678f;
//
//							}
//						}
//

				} else if (GlobalGameHandler.TableNumber == 2 || GlobalGameHandler.TableNumber == 3) {

					if (GameManager.Instance.ballsStriked) {

				
					
						if (pos_move.x > -2.617926f && pos_move.x < 2.247273f) {
				

							//x movment comment by me

							newX = pos_move.x;
						} else {
//						print ("White ball limts");
							if (pos_move.x < -2.617926f) {
								newX = -2.617926f;
//							print ("White ball limts");
							} else {
								newX = 2.247273f;
//							print ("White ball limts");
							}
						}
					
						//x movment comment by me
				
					
					
						if (pos_move.y > -2.931256f && pos_move.y < 1.640058f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -2.931256f) {


								newY = -2.931256f;



							} else {

								newY = 1.640058f;

							}
						}
					
					} else {

							
//						if (pos_move.x > -2.517926f && pos_move.x < 2.490257f ) 
//							{
//								//						print ("White ball limts");
//								newX = pos_move.x;
//							}
//							else
//							{
//								//	
//							if (pos_move.x < -2.517926f)
//								{
//									//							print ("White ball limts");
//								newX = -2.517926f;
//								}
//								else
//								{
//								newX = 2.490257f;
//	}
//							}
//
//					}
//			
//					if (pos_move.y > -2.996394f && pos_move.y < 1.640058f) {
//
//
//						newY = pos_move.y;
					}
//
//				else {
//			
//						if (pos_move.y < -2.996394f) {
//
//				
//							newY = 	-2.996394f;
//						
//
//
//					} else {
//					
//							newY = 1.640058f;
//						
//					}
//				}

				} else if (GlobalGameHandler.TableNumber == 4 || GlobalGameHandler.TableNumber == 5) {

					if (GameManager.Instance.ballsStriked) {




						if (pos_move.x > -1.0013934f && pos_move.x < 0.5739191f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -1.90013934f) {
								newX = -1.0013934f;
								//							print ("White ball limts");
							} else {
								newX = 0.5739191f;
								//							print ("White ball limts");
							}
						}





						//x movment comment by me


						if (pos_move.y > -2.953603f && pos_move.y < 1.640058f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -2.953603f) {


								newY = -2.953603f;



							} else {

								newY = 1.640058f;

							}
						}

					} else {

					
//						if (pos_move.x > -0.9013934f && pos_move.x < 0.9391889f ) 
//						{
//							//						print ("White ball limts");
//							newX = pos_move.x;
//						}
//						else
//						{
//							//	
//							if (pos_move.x < -0.9013934f)
//							{
//								//							print ("White ball limts");
//								newX =-0.9013934f;
//							}
//							else
//							{
//								newX = 0.9391889f;
//							}
//						}

					}

//					if (pos_move.y > -2.996394f && pos_move.y < 1.640058f) {
//
//
//						newY = pos_move.y;
//					}
//
//					else {
//
//						if (pos_move.y < -2.996394f) {
//
//
//							newY = 	-2.996394f;
//
//
//
//						} else {
//
//							newY = 1.640058f;
//
//						}
//					}

				} else if (GlobalGameHandler.TableNumber == 8 || GlobalGameHandler.TableNumber == 9) {

					if (GameManager.Instance.ballsStriked) {

				
//						print ("White ball limts  x "+pos_move.x);
//						print ("White ball limts  y "+pos_move.y);

						if (pos_move.x > -3.79368f && pos_move.x < 3.395111f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -3.79368f) {
								newX = -3.79368f;
								//							print ("White ball limts");
							} else {
								newX = 3.395111f;
								//							print ("White ball limts");
							}
						}

						//x movment comment by me

						if (pos_move.y > -1.838948f && pos_move.y < 0.5425022f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -1.838948f) {


								newY = -1.838948f;



							} else {

								newY = 0.5425022f;

							}
						}

					} else {

//					
//
//						if (pos_move.x > -3.69368f && pos_move.x < 3.490358f ) 
//						{
//							//						print ("White ball limts");
//							newX = pos_move.x;
//						}
//						else
//						{
//							//	
//							if (pos_move.x < -3.69368f)
//							{
//								//							print ("White ball limts");
//								newX =-3.69368f;
//							}
//							else
//							{
//								newX = 3.490358f;
//							}
//						}

					}

//					if (pos_move.y > -1.885196f && pos_move.y < 0.5425022f) {
//
//
//						newY = pos_move.y;
//					}
//
//					else {
//
//						if (pos_move.y < -1.885196f) {
//
//
//							newY = 	-1.885196f;
//
//
//
//						} else {
//
//							newY = 0.5425022f;
//
//						}
//					}

				} else if (GlobalGameHandler.TableNumber == 10 || GlobalGameHandler.TableNumber == 11) {
			

//					if (GlobalGameHandler.isBumper) {
//						if (pos_move.x< 0) {
//							newX = newX + 0.05f;
//						} else if (pos_move.x> 0) {
//							newX = newX - 0.05f;
//						}
//						if (pos_move.y< 0) {
//							newY = newY + 0.05f;
//						} else if (pos_move.y> 0) {
//							newY = newY - 0.05f;
//						}
//						rewind ();
//					}
//					if (!GlobalGameHandler.isBumper) {
						
//						print (GlobalGameHandler.isBumper+"   isbumper");

//						record ();
						if (GameManager.Instance.ballsStriked) {


						
//													print ("White ball limts  x "+pos_move.x);
//													print ("White ball limts  y "+pos_move.y);

						if (pos_move.x > -3.735953f && pos_move.x < 2.890714f) {
								
							//	print ("White ball limts");   1.535889  -2.234021  4.066616  -4.450589
							//x movment comment by me  y  0.3925059

								newX = pos_move.x;
							} else {
								//						print ("White ball limts");
							if (pos_move.x < -3.735953f) {
								newX = -3.735953f;
									//							print ("White ball limts");
								} else {
								newX = 2.890714f;
									//							print ("White ball limts");
								}
							}


						if (pos_move.y > -2.865357f && pos_move.y < 1.63131f) {


								newY = pos_move.y;
							} else {

							if (pos_move.y < -2.865357f) {

								newY = -2.865357f;


								} else {
								newY = 1.63131f;

								}
							}
						} else {
						}
					}
				else if (GlobalGameHandler.TableNumber == 12 || GlobalGameHandler.TableNumber == 13) {

					if (GameManager.Instance.ballsStriked) {
						//					print ("White ball stricked");

//						print ("White ball limts  x "+pos_move.x);
//						print ("White ball limts  y "+pos_move.y);

						if (pos_move.x > -4.454245f && pos_move.x < 4.052359f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -4.454245f) {
								newX = -4.454245f;
								//							print ("White ball limts");
							} else {
								newX = 4.052359f;
								//							print ("White ball limts");
							}
						}


						if (pos_move.y > -2.34434f && pos_move.y < 1.021462f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -2.34434f) {

								newY = -2.34434f;


							} else {
								newY = 1.021462f;

							}
						}


						//x movment comment by me
					}else {
					}
				}


				else if (GlobalGameHandler.TableNumber == 14 || GlobalGameHandler.TableNumber == 15) {

					if (GameManager.Instance.ballsStriked) {
						//					print ("White ball stricked");

//												print ("White ball limts  x "+pos_move.x);
//												print ("White ball limts  y "+pos_move.y);

						if (pos_move.x > -2.372183f && pos_move.x < 1.95843f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -2.372183f) {
								newX = -2.372183f;
								//							print ("White ball limts");
							} else {
								newX = 1.95843f;
								//							print ("White ball limts");
							}
						}



						if (pos_move.y > -2.936505f && pos_move.y < 1.608155f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -2.936505f) {

								newY =-2.936505f;


							} else {
								newY =1.608155f;

							}
						}


						//x movment comment by me
					}else {
					}
				}


				else if (GlobalGameHandler.TableNumber == 16 || GlobalGameHandler.TableNumber == 17) {

					if (GameManager.Instance.ballsStriked) {

//																		print ("White ball limts  x "+pos_move.x);
//																		print ("White ball limts  y "+pos_move.y);


						if (pos_move.x > -3.905833f && pos_move.x < 3.335943f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -3.905833f) {
								newX = -3.905833f;
								//							print ("White ball limts");
							} else {
								newX =3.335943f;
								//							print ("White ball limts");
							}
						}


						if (pos_move.y > -2.216145f && pos_move.y < 0.8545012f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -2.216145f) {

								newY = -2.216145f;


							} else {
								newY = 0.8545012f;

							}
						}


						//x movment comment by me
					} else {
					}
				}



				if (GlobalGameHandler.TableNumber == 18 || GlobalGameHandler.TableNumber == 19 ) {



					if (GameManager.Instance.ballsStriked) {
//						print ("White ball limts  x "+pos_move.x);
//				        print ("White ball limts  y "+pos_move.y);

						if (pos_move.x > -4.394811f && pos_move.x < 3.920192f) {
							//						print ("White ball limts");
							//x movment comment by me

							newX = pos_move.x;
						} else {
							//						print ("White ball limts");
							if (pos_move.x < -4.394811f) {
								newX = -4.394811f;
								//							print ("White ball limts");
							} else {
								newX = 3.920192f;
								//							print ("White ball limts");
							}
						}


						if (pos_move.y > -2.934318f && pos_move.y < 1.64182f) {


							newY = pos_move.y;
						} else {

							if (pos_move.y < -2.934318f) {

								newY = -2.934318f;


							} else {
								newY = 1.64182f;

							}
						}


						//x movment comment by me
					} else {
					}
				}



//				}


//				if (GlobalGameHandler.isBumper) {
//
//
//					//					print ("CurrentPos  => "+CurrentPos+"  Last Pos =>  "+LastPos);
//					if (pos_move.x >= 1) {
//						newX = newX - 0.002f;
//
//					} else if (pos_move.x <= -0.5f) {
//
//						newX = newX + 0.002f;
//					}
//
//					if (pos_move.y >= 1) {
//						newY = newY - 0.002f;
//
//					} else if (pos_move.y <= -0.5f) {
//						newY=newY+0.002f;
//					}
//					gameObject.transform.position = LastPos;
//				}

//				if (!GlobalGameHandler.isBumper) {
//				LastPos = CurrentPos;

//					print ("CurrentPos  => "+CurrentPos+"  Last Pos =>  "+LastPos);
//				if(GlobalGameHandler.TableNumber==0){
				lastCorrectPosition = transform.position;
				transform.position = new Vector3 (newX, newY, transform.position.z);

//				CurrentPos = gameObject.transform.position;

				
//				}
//				}
//				else if (GlobalGameHandler.TableNumber > 0) {
//
//					if (!GlobalGameHandler.isBumper) {
//						LastPos = CurrentPos;
//
//						//					print ("CurrentPos  => "+CurrentPos+"  Last Pos =>  "+LastPos);
//
//						lastCorrectPosition = transform.position;
//						transform.position = new Vector3 (newX, newY, transform.position.z);
//
//						CurrentPos = gameObject.transform.position;
//					}
//					print ("isbumper " + GlobalGameHandler.isBumper);
//					if (GlobalGameHandler.isBumper) {
//					
//
////					print ("CurrentPos  => "+CurrentPos+"  Last Pos =>  "+LastPos);
//						if (LastPos.x >= 1) {
//							LastPos.x = LastPos.x - 0.002f;
//
//						} else if (LastPos.x <= -0.5f) {
//
//							LastPos.x = LastPos.x + 0.002f;
//						}
//
//						if (LastPos.y >= 1) {
//							LastPos.y = LastPos.y - 0.002f;
//
//						} else if (LastPos.y <= -0.5f) {
//							LastPos.y = LastPos.y + 0.002f;
//						}
//						gameObject.transform.position = LastPos;
//					}
//					print ("CurrentPos  => " + CurrentPos + "  Last Pos =>  " + LastPos);
//				}
			}
        }
    }

	public void rewind()
	{
		if (GlobalGameHandler.isBumper) {

			if (last_positions.Count > 0) {
				transform.position = last_positions [18];
				print ("laat   " + last_positions [18]);
//				last_positions.RemoveAt (0);
			}
		}
	}

	public void record()
	{
		if (!GlobalGameHandler.isBumper) {
			last_positions.Insert (0, transform.position);
			print ("last positionss  "+last_positions[0]);

		}}
    void OnMouseUp()
    {
		print (GlobalGameHandler.isBumper + "   isbumper");

        if (GameManager.Instance.hasCueInHand)
        {
//			print ("White ball limts");
            ShowAllControllers();
            movingWhiteBall = false;

            whiteBallLimits.SetActive(false);
//            if (!GameManager.Instance.offlineMode)
//                PhotonNetwork.RaiseEvent(16, 1, true, null);
        }
    }

    private void hideCalledPockets()
    {
        for (int i = 0; i < calledPockets.Length; i++)
        {
            calledPockets[i].SetActive(false);
        }
    }


    public void setTurnOffline(bool showTurnMessage) {

        checkShot();

        GameManager.Instance.audioSources[0].Play();

        hidePocketButtons();

        if(GameManager.Instance.wasFault) {
            if(GameManager.Instance.faultMessage.Length > 0)
                GameManager.Instance.mGameControllerScript.showMessage(GameManager.Instance.faultMessage);
            Debug.Log("Fault: " + GameManager.Instance.faultMessage);
            if(GameManager.Instance.offlinePlayerTurn == 1) {
                if(GameManager.Instance.offlinePlayer1OwnSolid) {
                    GameManager.Instance.ownSolids = false;
                } else {
                    GameManager.Instance.ownSolids = true;
                }
                GameManager.Instance.offlinePlayerTurn = 2;
                if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) 
                    mGameControllerScript.resetTimers(2, showTurnMessage);
                
            } else {
                if(GameManager.Instance.offlinePlayer1OwnSolid) {
                    GameManager.Instance.ownSolids = true;
                } else {
                    GameManager.Instance.ownSolids = false;
                }
                GameManager.Instance.offlinePlayerTurn = 1;
                if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) 
                    mGameControllerScript.resetTimers(1, showTurnMessage);
                
            }

            //if (message.Equals (StaticStrings.potedCueBall)) {

            if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
               


				if (LockZPosition.isStiger) {
					ballHand.SetActive (true);

					print ("not win not lost");
					GameManager.Instance.hasCueInHand = true;

					Vector3 newBallPos = GameManager.Instance.whiteBall.transform.position;
					newBallPos.z = ballHand.transform.position.z;
					ballHand.transform.position = newBallPos;

					LockZPosition.isStiger = false;
				}
            }
            //}
        } else {
            if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
                mGameControllerScript.resetTimers(GameManager.Instance.offlinePlayerTurn, false);
                Debug.Log("Turn: " + GameManager.Instance.offlinePlayerTurn);
            }
                
        }

        
        cueSpinObject.GetComponent<SpinController>().resetPositions();
        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
        ballsInMovement = false;
        GameManager.Instance.firstBallTouched = false;
        GameManager.Instance.ballTouchedBand = 0;
        GameManager.Instance.wasFault = false;
        GameManager.Instance.faultMessage = "";
        GameManager.Instance.validPot = false;
        isServer = true;
        canShowControllers = true;
        hideCalledPockets();
        GameManager.Instance.calledPocket = true;
        canShowControllers = true;

        if(!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
            Invoke("ShowAllControllers", 0.25f);
            ShotPowerIndicator.anim.Play("MakeVisible");
        }
            
    }

    public IEnumerator setMyTurn(bool showTurnMessage)
    {

        Debug.Log("My turn");

//        GameManager.Instance.audioSources[0].Play();

		changeCueImage(GlobalGameHandler.CueNumber);

        hidePocketButtons();
        mGameControllerScript.resetTimers(1, showTurnMessage);
        cueSpinObject.GetComponent<SpinController>().resetPositions();
        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
        ballsInMovement = false;
        GameManager.Instance.firstBallTouched = false;
        GameManager.Instance.ballTouchedBand = 0;
        GameManager.Instance.wasFault = false;
        GameManager.Instance.validPot = false;
        isServer = true;
        canShowControllers = true;
        hideCalledPockets();
        GameManager.Instance.calledPocket = true;

        if (GameManager.Instance.callPocketAll)
        {
            GameManager.Instance.calledPocket = false;
            canShowControllers = false;
            callPocket();
        }


        if (GameManager.Instance.callPocketBlack)
        {
            if (GameManager.Instance.ownSolids && GameManager.Instance.solidPoted >= 7)
            {
                GameManager.Instance.calledPocket = false;
                canShowControllers = false;
                callPocket();
            }
            else if (!GameManager.Instance.ownSolids && GameManager.Instance.stripedPoted >= 7)
            {
                GameManager.Instance.calledPocket = false;
                canShowControllers = false;
                callPocket();
            }
        }


        while (GameManager.Instance.calledPocket == false)
        {
            Debug.Log("CALLED: " + GameManager.Instance.calledPocket);
            yield return new WaitForSeconds(0.2f);
        }

        canShowControllers = true;

        if(!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
            Invoke("ShowAllControllers", 0.25f);
            ShotPowerIndicator.anim.Play("MakeVisible");
        }

    }

    public void setOpponentTurn()
    {
        int oppoCueIndex = GameManager.Instance.opponentCueIndex;
        if(GameManager.Instance.opponentCueIndex >= cueTextures.Length - 1) {
            oppoCueIndex = cueTextures.Length-1;
        }
		changeCueImage(GlobalGameHandler.CueNumber);

        hidePocketButtons();
        canShowControllers = true;
        hideCalledPockets();
        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
        //opponentBallsStoped = true;
        cueSpinObject.GetComponent<SpinController>().resetPositions();
        ballsInMovement = false;
        //GameManager.Instance.wasFault = false;
        cue.GetComponent<Renderer>().enabled = true;
        Invoke("ShowAllControllers", 0.25f);
        isServer = false;
        multiplayerDataArray.Clear();
        startMovement = false;
        ShotPowerIndicator.anim.Play("ShotPowerAnimation");
        mGameControllerScript.resetTimers(2, true);
    }

    private void setCuePosition()
    {
        
    }

    private void ShowAllControllers()
    {

        if (canShowControllers)
        {
			
            Debug.Log("Showing controllers");



            targetLine.SetActive(true);
            cue.transform.position = new Vector3(GameManager.Instance.balls[0].transform.position.x, GameManager.Instance.balls[0].transform.position.y, cue.transform.position.z);
            cue.GetComponent<Renderer>().enabled = true;
			m_GameControllerScript.mGameController.pausebtn.Play ("pausebtnanimOf");
			m_GameControllerScript.mGameController.accuracy.Play ("AccuracyBackAnim");
			m_GameControllerScript.mGameController.ballpots.Play ("ballpotIn");

			if (GlobalGameHandler.isShoot) {
				if (!GlobalGameHandler.isballPocket) {
			
					GlobalGameHandler.RemaningLifes--;

					if (GlobalGameHandler.WhiteBallPoted) {
						GlobalGameHandler.WhiteBallPoted = false;
						GlobalGameHandler.RemaningLifes--;

		
					}
			
					if (GlobalGameHandler.RemaningLifes <= 0) {
				
						mGameControllerScript.levelFail ();
					}
					StartCoroutine (heartbrokenShow ());
				} else if (GlobalGameHandler.isballPocket) {
					GlobalGameHandler.isballPocket = false;

					if (GlobalGameHandler.WhiteBallPoted) {
						GlobalGameHandler.WhiteBallPoted = false;
						GlobalGameHandler.RemaningLifes--;
						GlobalGameHandler.RemaningLifes--;

						StartCoroutine (heartbrokenShow ());
			

					}
					if (GlobalGameHandler.RemaningLifes <= 0) {

						mGameControllerScript.levelFail ();

					}

				}


				mGameControllerScript.RemainingLifeTxt.text = GlobalGameHandler.RemaningLifes.ToString ();

				if (GlobalGameHandler.RemaningLifes <= 0) {
					mGameControllerScript.RemainingLifeTxt.text = "0".ToString ();
				}
			
				GlobalGameHandler.isShoot = false;

				print(PlayerPrefs.GetInt("firsttime")+"first time");
				if (PlayerPrefs.GetInt ("firsttime") == 0) {
					
					PlayerPrefs.SetInt ("firsttime",1);
					GlobalGameHandler.nextlevel = true;
					SceneManager.LoadScene ("Menu");
				}
			}

			m_GameControllerScript.mGameController.ballHideOn ();

		}

    }

	IEnumerator heartbrokenShow ()
	{

		m_GameControllerScript.mGameController.heartBeating.SetActive (false);
		m_GameControllerScript.mGameController.heartBroken.SetActive (true);
		yield return new WaitForSeconds (2f);
		m_GameControllerScript.mGameController.heartBeating.SetActive (true);
		m_GameControllerScript.mGameController.heartBroken.SetActive (false);
//		gameControllerScript.RemainingLifeTxt.text = GlobalGameHandler.RemaningLifes.ToString ();
	}

    public void HideAllControllers()
    {
        targetLine.SetActive(false);
        cue.GetComponent<Renderer>().enabled = false;

		print ("hide all controls");
    }

    public void stopTimer()
    {
        GameManager.Instance.stopTimer = true;
    }


    public void noMoreTime()
    {

    }

    private bool canCheckAnotherCueRotation = true;

    int aa;

    private IEnumerator rotateCue()
    {
        canCheckAnotherCueRotation = false;
        //yield return new WaitForSeconds(0.1f);

        if (!movingWhiteBall && !ballsInMovement && isServer)
        {
			
            float ang = 0;

            Quaternion initRot = cue.transform.rotation;
            Quaternion rot = cue.transform.rotation;

            
            

            if (!ShotPowerIndicator.mouseDown && displayTouched & !isFirstTouch)
            {
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Mouse Y")))
                {
                    ang = Input.GetAxis("Mouse X");
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > transform.position.y)
                    {
                        ang = -ang;
                    }
                }
                else
                {
                    ang = Input.GetAxis("Mouse Y");
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
                    {
                        ang = -ang;
                    }
                }

                float multAng = Vector2.Distance(touchMousePos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                multAng *= 300.0f;

                if(multAng < 1.5f) multAng = 1.5f;

            
                multAng = multAng * 0.05f;

                
                if(multAng > 6.0f) multAng = 6.0f;



                ang *=  multAng;

                touchMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                isFirstTouch = false;
            }



            //if(touchMousePos != Vector3.zero) {
                

            // } else {

            // }

            // float multAng = Vector2.Distance(touchMousePos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // multAng *= 300.0f;

            // if(multAng < 1.5f) multAng = 1.5f;

        
            // multAng = multAng * 0.05f;

            
            // if(multAng > 8.0f) multAng = 8.0f;


            // if(displayTouched)
            //     Debug.Log("Andgle: " + (aa++) + "    -     " + multAng);

            // ang *=  multAng;

            // touchMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //ang *= 0.9f;


            yield return new WaitForSeconds(0.01f);

            if (displayTouched) {
                
				if (GlobalGameHandler.isDrageDecrese) {
//					print ("rot before " + rot.z);
//
////					rot.z = rot.z / rot.z;
//
//					print ("rot  after" + rot.z);
					rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, (rot.eulerAngles.z + (ang*Time.deltaTime)));
				} else {
					rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z + ang);
				}
                cue.transform.rotation = rot;
//				print ("rot "+rot.z);
//				print ("rotate cue");
                if (isServer && initRot.eulerAngles != rot.eulerAngles) {
//                    if (!GameManager.Instance.offlineMode)
//                        PhotonNetwork.RaiseEvent(7, rot, true, null);
                }
            }

            canCheckAnotherCueRotation = true;

            drawTargetLines();
        } else {
            canCheckAnotherCueRotation = true;
        }
    }

    private IEnumerator rotateCueInvoke(Quaternion rot, Quaternion initRot) {

        yield return new WaitForSeconds(0.1f);

        if (displayTouched) {

			print ("rot "+rot);
            cue.transform.rotation = rot;

            

            if (isServer && initRot.eulerAngles != rot.eulerAngles) {
//                if (!GameManager.Instance.offlineMode)
//                    PhotonNetwork.RaiseEvent(7, rot, true, null);
            }
        } 

        yield return true;
    }

    // DRAW TARGET LINES
    private void drawTargetLines()
    {
        
        if (!ballsInMovement)
        {
            Vector3 dir = transform.position - posDetector.transform.position;
            dir.z = 0;
            dir = dir.normalized;

            Vector3 linePos = transform.position;
            linePos.z = -3;

            lineRenderer.SetPosition(0, linePos);

            ray.origin = transform.position;
            ray.direction = dir;

            if (Physics.SphereCast(ray, radius, out hitInfo, 10, layerMask))
            {

                endPosition = ray.origin + (ray.direction.normalized * hitInfo.distance);
                endPosition.z = -3;


                circle.transform.position = endPosition;
                Vector3 linePos1 = endPosition;
                linePos1.z = -3;
                lineRenderer.SetPosition(1, linePos1);


                if (hitInfo.transform.tag.Contains("Ball"))
                {
                    int ballNumber =5;
					lineRenderer3.GetComponent<LineRenderer> ().material = mGameControllerScript.line3Materials [System.Convert.ToInt32(hitInfo.transform.name)-1];
                    if (GameManager.Instance.playersHaveTypes)
                    {
                        if (isServer)
                        {
                            if ((GameManager.Instance.ownSolids && ballNumber < 8) || (!GameManager.Instance.ownSolids && ballNumber > 8) || ballNumber == 8)
                            {
                                drawLine(linePos);
                            }
                            else
                            {
                                wrongBall.SetActive(true);
                                circle.GetComponent<LineRenderer>().enabled = false;
                                ballCollideFirst = false;
//                                lineRenderer2.enabled = false;
                                lineRenderer3.enabled = false;
                            }
                        }
                        else
                        {
                            if ((!GameManager.Instance.ownSolids && ballNumber < 8) || (GameManager.Instance.ownSolids && ballNumber > 8) || ballNumber == 8)
                            {
                                drawLine(linePos);
                            }
                            else
                            {
                                wrongBall.SetActive(true);
                                circle.GetComponent<LineRenderer>().enabled = false;
                                ballCollideFirst = false;
//                                lineRenderer2.enabled = false;
                                lineRenderer3.enabled = false;
                            }
                        }

                    }
                    else
                    {
                        drawLine(linePos);

					

                    }


                }
                else
                {
                    wrongBall.SetActive(false);
                    circle.GetComponent<LineRenderer>().enabled = true;
                    ballCollideFirst = false;
//                    lineRenderer2.enabled = false;
                    lineRenderer3.enabled = false;
                }
            }
        }
    }

    public void drawLine(Vector3 linePos)
    {
        wrongBall.SetActive(false);
        circle.GetComponent<LineRenderer>().enabled = true;
		Vector3 hitBallPosition = hitInfo.transform.position;
        hitBallPosition.z = -3;

//		hitBallPosition.x = hitBallPosition.x+0.22f ;

        lineRenderer3.SetPosition(0, hitBallPosition);

        Vector3 r2dir = hitBallPosition - endPosition;

        Ray r2 = new Ray(hitBallPosition, r2dir);

        Vector3 pos3 = r2.origin + (3 + GameManager.Instance.cueAim * 0.25f) * r2dir;
        pos3.z = -3;
//		pos3.x = pos3.x - 0.22f;
        lineRenderer3.SetPosition(1, pos3);

        Vector3 l = (3 + GameManager.Instance.cueAim * 0.25f) * r2dir;
        l = Quaternion.Euler(0, 0, -90) * l + endPosition;
        l.z = -3;
//        lineRenderer2.SetPosition(0, endPosition);
        float angle = 90.0f;
        float angleBeetwen = AngleBetweenThreePoints(linePos, endPosition, l);

        if (angleBeetwen < 90.0f || angleBeetwen > 270.0f)
        {
            l = (3 + GameManager.Instance.cueAim * 0.25f) * r2dir;
            l = Quaternion.Euler(0, 0, 90) * l + endPosition;
            l.z = -3;
        }

//        lineRenderer2.SetPosition(1, l);
        ballCollideFirst = true;


        if (GameManager.Instance.showTargetLines)
        {
//            	lineRenderer2.enabled = true;
            lineRenderer3.enabled = true;
        }
    }

    public float AngleBetweenThreePoints(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float a = pointB.x - pointA.x;
        float b = pointB.y - pointA.y;
        float c = pointB.x - pointC.x;
        float d = pointB.y - pointC.y;

        float atanA = Mathf.Atan2(a, b) * Mathf.Rad2Deg;
        float atanB = Mathf.Atan2(c, d) * Mathf.Rad2Deg;

        float output = atanB - atanA;
        output = Mathf.Abs(output);



        return output;
    }
}








// pracrtice 



