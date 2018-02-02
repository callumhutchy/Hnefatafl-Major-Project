using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public Board board;

    public int size = 11;

    public Vector2[] knightPos;
    public Vector2[] barbarianPos;

    // Use this for initialization
    void Start()
    {
        knightPos = new Vector2[12] { new Vector2(5, 3), new Vector2(4, 4), new Vector2(5, 4), new Vector2(6, 4), new Vector2(3, 5), new Vector2(4, 5), new Vector2(6, 5), new Vector2(7, 5), new Vector2(4, 6), new Vector2(5, 6), new Vector2(6, 6), new Vector2(5, 7) };

        barbarianPos = new Vector2[24] { new Vector2(3, 0), new Vector2(4, 0), new Vector2(5, 0), new Vector2(6, 0), new Vector2(7, 0), new Vector2(5, 1), new Vector2(0, 3), new Vector2(0, 4), new Vector2(0, 5), new Vector2(0, 6), new Vector2(0, 7), new Vector2(1, 5), new Vector2(10, 3), new Vector2(10, 4), new Vector2(10, 5), new Vector2(10, 6), new Vector2(10, 7), new Vector2(9, 5), new Vector2(5, 9), new Vector2(3, 10), new Vector2(4, 10), new Vector2(5, 10), new Vector2(6, 10), new Vector2(7, 10) };

        board.Generate(size);
        SetupPieces();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetupPieces()
    {
        Material kingMat = (Material)Resources.Load("Materials/KingMat");
        Material knightMat = (Material)Resources.Load("Materials/KnightMat");
        Material barbarianMat = (Material)Resources.Load("Materials/BarbarianMat");

        GameObject piece = (GameObject)Resources.Load("Prefabs/GamePiece");

        GameObject king = GameObject.Instantiate(piece);
        king.GetComponent<Renderer>().material = kingMat;
        king.name = "King";
        king.transform.position = new Vector3(size / 2, 0.5f, size / 2);
        king.SetActive(true);

        if (size == 11)
        {
            int index = 1;
            foreach (Vector2 knight in knightPos)
            {
                GameObject goKnight = GameObject.Instantiate(piece);
                goKnight.GetComponent<Renderer>().material = knightMat;
                goKnight.name = "Knight " + index;
                goKnight.transform.position = new Vector3(knight.x, 0.5f, knight.y);
                goKnight.SetActive(true);
            }

			index = 1;

			foreach (Vector2 barbarian in barbarianPos){
				GameObject goBarbarian = GameObject.Instantiate(piece);
				goBarbarian.GetComponent<Renderer>().material = barbarianMat;
				goBarbarian.name = "Barbarian " + index;
				goBarbarian.transform.position = new Vector3(barbarian.x, 0.5f, barbarian.y);
				goBarbarian.SetActive(true);
			}


        }


    }


}
