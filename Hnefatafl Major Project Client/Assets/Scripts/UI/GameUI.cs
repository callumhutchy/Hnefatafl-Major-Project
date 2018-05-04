using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//Provides ui functionality in the game
public class GameUI : MonoBehaviour {

    //Public references to server UI elments in the game
	public GameObject gameUI;
	public GameObject exitUI;

	public GameObject winUI;
	public TMP_Text winText;

    //Reference to the network manager if it is a online game
    public NetManager netMan;
    public bool netGame = false;

    private void Awake()
    {
        //Find the network manager then if its an online game assign it to the net man
        netMan = GameObject.FindGameObjectWithTag("net_man").GetComponent<NetManager>();
        if (netMan != null)
        {
            netGame = true;
        }
    }

    //Show exit confirmations
    public void ExitButton(){
		gameUI.SetActive(false);
		exitUI.SetActive(true);
	}

    //Exit yes button, if it is a net game the opponent needs to be told that we are quiting
    //Then load the main menu
	public void ExitYesButton(){
		GameObject gameData = GameObject.FindGameObjectWithTag("game_data");
		GameObject.Destroy(gameData);
        if (netGame)
        {
            netMan.quit = true;
        }
        
		SceneManager.LoadScene("Main Menu");

	}

    //Exit no button, close exit confirmation dialog and return to the game
	public void ExitNoButton(){
		exitUI.SetActive(false);
		gameUI.SetActive(true);
	}

    //If the barbarians win, display related text
	public void BarbariansWin(){
		gameUI.SetActive(false);
		winText.text = "Barbarians Win!";
		winUI.SetActive(true);
	}

    //If vikings win, display related text
	public void VikingsWin(){
		gameUI.SetActive(false);
		winText.text = "Vikings Win!";
		winUI.SetActive(true);
	}

    //Opponent variables
    public bool oVikingsWin = false;
    public bool oBarbariansWin = false;
    //Our variables
    public bool vikingsWin = false;
    public bool barbariansWin = false;

    //Opponent vikings win
    void OVikingsWin()
    {
        oVikingsWin = true;
    }

    //Opponent Barbarians win
    void OBarbariansWin()
    {
        oBarbariansWin = true;
    }


    private void Update()
    {
        //if the opponent vikings have won then display information
        if (oVikingsWin)
        {
            gameUI.SetActive(false);
            winText.text = "Enemy Vikings Win!";
            winUI.SetActive(true);
        }
        //If the opponent barbarians win, display information
        else if (oBarbariansWin)
        {
            gameUI.SetActive(false);
            winText.text = "Enemy Barbarians Win!";
            winUI.SetActive(true);
        }
        //else if we win display which team we won.
        else if (vikingsWin)
        {

            VikingsWin();
        }else if (barbariansWin)
        {
            BarbariansWin();
        }
    }

}
