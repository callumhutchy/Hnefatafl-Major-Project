﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour {

	public GameObject gameUI;
	public GameObject exitUI;

	public GameObject winUI;
	public TMP_Text winText;

	public void ExitButton(){
		gameUI.SetActive(false);
		exitUI.SetActive(true);
	}

	public void ExitYesButton(){
		GameObject gameData = GameObject.FindGameObjectWithTag("game_data");
		GameObject.Destroy(gameData);
		SceneManager.LoadScene("Main Menu");

	}

	public void ExitNoButton(){
		exitUI.SetActive(false);
		gameUI.SetActive(true);
	}

	public void BarbariansWin(){
		gameUI.SetActive(false);
		winText.text = "Barbarians Win!";
		winUI.SetActive(true);
	}

	public void VikingsWin(){
		gameUI.SetActive(false);
		winText.text = "Vikings Win!";
		winUI.SetActive(true);
	}
	
}
