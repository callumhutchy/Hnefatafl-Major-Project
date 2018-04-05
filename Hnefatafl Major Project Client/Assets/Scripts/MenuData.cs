﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuData : MonoBehaviour {

	public Team Player1;
	public Team Player2;

	public Sprite orangeButton;
	public Sprite greyButton;

	public GameObject player1Viking;
	public GameObject player1Barbarian;

	public GameObject player2Viking;
	public GameObject player2Barbarian;

	public bool p1v = false;
	public bool p1b = false;
	public bool p2v = false;
	public bool p2b = false;

	public void OnClickPlayer1Viking(){
        if (!p1v)
        {
            p1v = true;
            p2v = false;
            p1b = false;
            p2b = true;
            player1Viking.GetComponent<Image>().sprite = greyButton;
            player1Barbarian.GetComponent<Image>().sprite = orangeButton;
            player2Viking.GetComponent<Image>().sprite = orangeButton;
            player2Barbarian.GetComponent<Image>().sprite = greyButton;
        }
        else
        {
            p1v = false;
            p2v = true;
            p1b = true;
            p2b = false;
            player1Viking.GetComponent<Image>().sprite = orangeButton;
            player1Barbarian.GetComponent<Image>().sprite = greyButton;
            player2Viking.GetComponent<Image>().sprite = greyButton;
            player2Barbarian.GetComponent<Image>().sprite = orangeButton;
        }
	}

	public void OnClickPlayer1Barbarian(){
        if (!p1b)
        {

            p1v = false;
            p2v = true;
            p1b = true;
            p2b = false;
            player1Viking.GetComponent<Image>().sprite = orangeButton;
            player1Barbarian.GetComponent<Image>().sprite = greyButton;
            player2Viking.GetComponent<Image>().sprite = greyButton;
            player2Barbarian.GetComponent<Image>().sprite = orangeButton;
        }
        else
        {
            p1v = true;
            p2v = false;
            p1b = false;
            p2b = true;
            player1Viking.GetComponent<Image>().sprite = greyButton;
            player1Barbarian.GetComponent<Image>().sprite = orangeButton;
            player2Viking.GetComponent<Image>().sprite = orangeButton;
            player2Barbarian.GetComponent<Image>().sprite = greyButton;
        }
    }

	public void OnClickPlayer2Viking(){
        if (!p2v)
        {
            
            p1v = false;
            p2v = true;
            p1b = true;
            p2b = false;
            player1Viking.GetComponent<Image>().sprite = orangeButton;
            player1Barbarian.GetComponent<Image>().sprite = greyButton;
            player2Viking.GetComponent<Image>().sprite = greyButton;
            player2Barbarian.GetComponent<Image>().sprite = orangeButton;
        }
        else
        {
            p1v = true;
            p2v = false;
            p1b = false;
            p2b = true;
            player1Viking.GetComponent<Image>().sprite = greyButton;
            player1Barbarian.GetComponent<Image>().sprite = orangeButton;
            player2Viking.GetComponent<Image>().sprite = orangeButton;
            player2Barbarian.GetComponent<Image>().sprite = greyButton;
        }
    }



	public void OnClickPlayer2Barbarian(){
        if (!p2b)
        {
            p1v = true;
            p2v = false;
            p1b = false;
            p2b = true;
            player1Viking.GetComponent<Image>().sprite = greyButton;
            player1Barbarian.GetComponent<Image>().sprite = orangeButton;
            player2Viking.GetComponent<Image>().sprite = orangeButton;
            player2Barbarian.GetComponent<Image>().sprite = greyButton;
        }
        else
        {
            p1v = false;
            p2v = true;
            p1b = true;
            p2b = false;
            player1Viking.GetComponent<Image>().sprite = orangeButton;
            player1Barbarian.GetComponent<Image>().sprite = greyButton;
            player2Viking.GetComponent<Image>().sprite = greyButton;
            player2Barbarian.GetComponent<Image>().sprite = orangeButton;
        }
    }

	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnPlayClick(){
		bool play = false;
		if(p1v != p2v && p1b != p2b){
			if(p1v && p2b){
				Player1 = Team.VIKING;
				Player2 = Team.BARBARIAN;
			}else{
				Player1 = Team.BARBARIAN;
				Player2 = Team.VIKING;
			}
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
