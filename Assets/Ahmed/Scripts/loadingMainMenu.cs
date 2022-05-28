using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadingMainMenu : MonoBehaviour {


	public Animator Fade_anim;

	void Start () {
		

	}

	public void LoadingGamePlay()
	{
		SceneManager.LoadScene ("GamePlayScene");
	}

	public void  fadeAnim()
	{
		Fade_anim.SetTrigger ("fadein");

	}
}
