using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour {

	public string Player1Name = "";
	public string Player2Name = "";
	
	public GameObject MainMenuPanel;
	public GameObject HotSeatMenuPanel;
	
	public void OnTwoPlayerGameClick(){
		MainMenuPanel.SetActive(false);
		HotSeatMenuPanel.SetActive(true);
	}

	public void OnMultiplayerGameClick(){
		SceneManager.LoadScene("MultiplayerLobby");
	}

	public void ExitGame(){
		Application.Quit();
	}


}
