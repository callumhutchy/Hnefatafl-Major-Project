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

    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Awake()
    {
        audioSource = GameObject.FindGameObjectWithTag("audio_man").GetComponent<AudioSource>();
    }

    void PlayButtonSound()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void OnTwoPlayerGameClick(){
        PlayButtonSound();
        MainMenuPanel.SetActive(false);
		HotSeatMenuPanel.SetActive(true);
        
	}

	public void OnMultiplayerGameClick(){
        PlayButtonSound();
        SceneManager.LoadScene("MultiplayerLobby");
        
    }

	public void ExitGame(){
        PlayButtonSound();
        Application.Quit();
        
    }


}
