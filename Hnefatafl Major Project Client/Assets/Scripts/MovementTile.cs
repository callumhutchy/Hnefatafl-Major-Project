using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to make the movement tiles interactable
public class MovementTile : MonoBehaviour {

	//Which game piece was the movement tile spawned from
	public GameObject owner;

	//Move the piece to this location if we are selected.
	void OnMouseOver(){
		if(Input.GetKeyDown(KeyCode.Mouse0)){
			owner.GetComponent<Selectable>().MoveToLocation(this.transform);
		}
	}

}
