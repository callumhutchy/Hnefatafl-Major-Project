using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	public Game gameController;

	public Material normalMat;
	public Material selectedMat;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameController.selectedPiece != this.transform.gameObject){
			//GetComponent<cakeslice.Outline>().enabled = false;
			GetComponent<Renderer>().material = normalMat;
		}
	}

	void OnMouseEnter(){
		Debug.Log("Enter");
	}

	void OnMouseOver(){
		Debug.Log("Over");
		if(Input.GetMouseButton(0)){
			gameController.selectedPiece = this.transform.gameObject;
			GetComponent<Renderer>().material = selectedMat;
		}
	}

	void OnMouseExit(){
		Debug.Log("Exit");
	}

}
