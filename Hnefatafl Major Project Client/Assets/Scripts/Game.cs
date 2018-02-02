using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public Board board;

	public int size = 11;

	// Use this for initialization
	void Start () {
		board.Generate(size);
		SetupPieces();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetupPieces(){
		Material kingMat = (Material) Resources.Load("Materials/KingMat");
		Material knightMat = (Material) Resources.Load("Materials/KnightMat");
		Material BarbarianMat = (Material) Resources.Load("Materials/BarbarianMat");

		GameObject piece = (GameObject) Resources.Load("Prefabs/GamePiece");

		GameObject king = GameObject.Instantiate(piece);
		king.GetComponent<Renderer>().material = kingMat;
		king.name = "King";
		king.transform.position = new Vector3(size / 2, 0.5f, size / 2);
		king.SetActive(true);

	}


}
