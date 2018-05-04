using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour {
	
    //The main menu screen
	public GameObject MainMenuPanel;
	//Offline team selection screen
    public GameObject HotSeatMenuPanel;
    //Help display
    public GameObject HelpMenu;
    //Audio source for button clicks
    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Awake()
    {
        //Get the audio source in the scene
        audioSource = GameObject.FindGameObjectWithTag("audio_man").GetComponent<AudioSource>();
    }

    //Play button click
    void PlayButtonSound()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    //When the offline multiplayer button is clicked, make the team selection panel active
    public void OnTwoPlayerGameClick(){
        PlayButtonSound();
        MainMenuPanel.SetActive(false);
		HotSeatMenuPanel.SetActive(true);
        
	}

    //When the online multiplayer button is clicked, load the matchmaking scene
	public void OnMultiplayerGameClick(){
        PlayButtonSound();
        SceneManager.LoadScene("MultiplayerLobby");
        
    }

    //Exit the application
	public void ExitGame(){
        PlayButtonSound();
        Application.Quit();
        
    }

    //When the help icon is click, display the help information
    public void OnHelpClick(){
        PlayButtonSound();
        MainMenuPanel.SetActive(false);
        HelpMenu.SetActive(true);
    }

    //When the back button on the help screen is clicked
    public void HelpBackClick(){
        PlayButtonSound();
        HelpMenu.SetActive(false);
        MainMenuPanel.SetActive(true);
    }


}
