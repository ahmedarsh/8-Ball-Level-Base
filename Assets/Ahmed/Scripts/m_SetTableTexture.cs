using UnityEngine;


public class m_SetTableTexture : MonoBehaviour {

    public GameObject downside;

    public Sprite[] sprites;

	// Use this for initialization
	void Start () {
		
		gameObject.GetComponent<SpriteRenderer>().sprite = sprites[GlobalGameHandler.TableNumber];
		downside.GetComponent<SpriteRenderer>().sprite = sprites[GlobalGameHandler.TableNumber];
	
		if (PlayerPrefs.GetInt ("firsttime") == 0) {
			gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
			downside.GetComponent<SpriteRenderer>().sprite = sprites[0];

		}
		
	}
	
}
