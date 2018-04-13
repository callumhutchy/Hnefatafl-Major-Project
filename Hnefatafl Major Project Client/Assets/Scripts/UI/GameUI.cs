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

    public NetManager netMan;
    public bool netGame = false;

    private void Awake()
    {
        netMan = GameObject.FindGameObjectWithTag("net_man").GetComponent<NetManager>();
        if (netMan != null)
        {
            netGame = true;
        }
    }

    public void ExitButton(){
		gameUI.SetActive(false);
		exitUI.SetActive(true);
	}

	public void ExitYesButton(){
		GameObject gameData = GameObject.FindGameObjectWithTag("game_data");
		GameObject.Destroy(gameData);
        if (netGame)
        {
            netMan.quit = true;
        }
        
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

    public bool oVikingsWin = false;
    public bool oBarbariansWin = false;
    public bool vikingsWin = false;
    public bool barbariansWin = false;

    void OVikingsWin()
    {
        oVikingsWin = true;
    }

    void OBarbariansWin()
    {
        oBarbariansWin = true;
    }

    private void Update()
    {
        if (oVikingsWin)
        {
            gameUI.SetActive(false);
            winText.text = "Enemy Vikings Win!";
            winUI.SetActive(true);
        }
        else if (oBarbariansWin)
        {
            gameUI.SetActive(false);
            winText.text = "Enemy Barbarians Win!";
            winUI.SetActive(true);
        }else if (vikingsWin)
        {
            VikingsWin();
        }else if (barbariansWin)
        {
            BarbariansWin();
        }
    }

}
