using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuData : MonoBehaviour {

	public string Player1 = "";
	public string Player2 = "";

	public TMP_Dropdown player1DD;
	public TMP_Dropdown player2DD;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPlayClick(){
		bool play = false;
		if(player1DD.value == 0 && player2DD.value == 1){
			Player1 = "Viking";
			Player2 = "Barbarian";
			play = true;
		}else if(player1DD.value == 1 && player2DD.value == 0){
			Player1 = "Barbarian";
			Player2 = "Viking";
			play = true;
		}else{
			Debug.Log("Teams are not opposite");
			play = false;
		}
		if(play){
			DontDestroyOnLoad(this);
			SceneManager.LoadScene("GameScene");
		}
	}

}
