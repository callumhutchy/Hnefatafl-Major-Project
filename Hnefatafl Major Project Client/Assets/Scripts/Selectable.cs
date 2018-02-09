using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	public Game gameController;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameController.selectedPiece != this.transform.gameObject){
			
		}
	}

	void OnMouseEnter(){
		Debug.Log("Enter");
	}

	void OnMouseOver(){
		Debug.Log("Over");
		if(Input.GetMouseButton(0)){
			gameController.selectedPiece = this.transform.gameObject;
		}
	}

	void OnMouseExit(){
		Debug.Log("Exit");
	}

}
