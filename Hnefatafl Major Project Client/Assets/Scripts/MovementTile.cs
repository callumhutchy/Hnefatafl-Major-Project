using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTile : MonoBehaviour {

	public GameObject owner;




	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver(){
		if(Input.GetKeyDown(KeyCode.Mouse0)){
			owner.GetComponent<Selectable>().MoveToLocation(this.transform);
		}
	}

}
