using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MMainMenu : MonoBehaviour {

	public GameObject MenuPanel, LevelSelectionPanel  ;
	public loadingMainMenu loadingMainMenu;
	public int[] Remaining_Lifes; 

	public static MMainMenu instance;

	public GameObject[] Buybutton;

	public int[] CuePrice;

	public Text CoinsText;

	public GameObject[] levels;

	public Animator MoreGames;

	bool more=false;
	public Button  levelUnlock;

	void Awake(){

		Time.timeScale = 2f;
	
	if(!instance)	instance = this;
	
	}


    void Start() {


		PlayerPrefs.SetInt ("Coins",500);

		GlobalGameHandler.CueNumber = 0;

		GlobalGameHandler.OponCues = 0;

		GlobalGameHandler.TableNumber = 2;

		GameManager.Instance.mMainMenu = this;
		int level=PlayerPrefs.GetInt("Levels");
		print (level);
		LeveUnlock (level);

		MenuPanel.SetActive (true);
		LevelSelectionPanel.SetActive (false);


		for (int i = 0; i <= Buybutton.Length; i++) {
			if (PlayerPrefs.GetInt ("opencue" + i) == 1) {
				Buybutton [i].SetActive (false);
			}
		}

			
		CoinsText.text = PlayerPrefs.GetInt ("Coins").ToString();


		if (PlayerPrefs.GetInt ("Levels") == 1) {
			
			levelUnlock.interactable =false;
			levelUnlock.gameObject.GetComponent<Animator> ().enabled = false;

		}
	
    }

	public void LeveUnlock(int level)
	{
		for(int i=0;i<level;i++)
		{
			levels [i].transform.GetComponent<Image> ().enabled = true;
			levels[i].transform.GetChild (0).gameObject.SetActive (false);
			levels[i].transform.GetChild (1).gameObject.SetActive (true);
		}

	}

	public void OnPlayButton()
	{
		
		if (PlayerPrefs.GetInt ("firsttime") == 0) {
		
			SceneManager.LoadScene ("GamePlayScene");
		} else {
			MenuPanel.SetActive (false);
			LevelSelectionPanel.SetActive (true);

		
		}
	}

	public void OnCueSelectionButton()
	{
		MenuPanel.SetActive (false);
		LevelSelectionPanel.SetActive (false);
//	    LoadingPanel.SetActive (false);


	}

	public void OnLevelButton(int level ) {


		GameManager.Instance.offlineMode = true;
		GameManager.Instance.roomOwner = true;

		GlobalGameHandler.levelNumer = level;
		GlobalGameHandler.TableNumber = level;
		LevelSelectionPanel.SetActive (false);
		GlobalGameHandler.RemaningLifes = Remaining_Lifes[level];
		
	
		StartCoroutine (LoadingCorutine());


	}

	IEnumerator LoadingCorutine(){
		loadingMainMenu.gameObject.SetActive (true);
		
		yield return new WaitForSeconds(2f);
		loadingMainMenu.Fade_anim.enabled = true;

		loadingMainMenu.fadeAnim();

		yield return new WaitForSeconds (4f);


	}


	public void OnBackButtonPres()
	{
		MenuPanel.SetActive (true);
		LevelSelectionPanel.SetActive (false);
	}


	public void selectCue(int cue)
	{
		GlobalGameHandler.CueNumber = cue;




	}

	public void CueBuy(int cue)
	{
	
		int coins = PlayerPrefs.GetInt ("Coins");
			if(coins>=CuePrice[cue])
			{

			Buybutton [cue].SetActive (false);
			PlayerPrefs.SetInt ("opencue" + cue, 1);
			int remaining = PlayerPrefs.GetInt ("Coins") - CuePrice [cue];

			PlayerPrefs.SetInt ("Coins", remaining);
			CoinsText.text = PlayerPrefs.GetInt ("Coins").ToString();

			}
	}


	public void  cangeVOlume(float volume)
	{

		AudioListener.volume = volume;
	}

	public void setSettings(int qualityIndex)
	{

		QualitySettings.SetQualityLevel (qualityIndex);
	}
	

	public void OnQuit(){

		Application.Quit();
	}

	public void OnMoreGameOn()
	{
		more = !more;
		if (more) {
			MoreGames.Play ("moreGame Anim");
		} else {
			MoreGames.Play ("more game off");
		}

	}


	}
