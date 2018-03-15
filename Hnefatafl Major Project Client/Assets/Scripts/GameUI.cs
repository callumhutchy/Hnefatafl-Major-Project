using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	public GameObject gameUI;
	public GameObject exitUI;

	public void ExitButton(){
		gameUI.SetActive(false);
		exitUI.SetActive(true);
	}

	public void ExitYesButton(){
		SceneManager.LoadScene("Main Menu");
	}

	public void ExitNoButton(){
		exitUI.SetActive(false);
		gameUI.SetActive(true);
	}
	
}
