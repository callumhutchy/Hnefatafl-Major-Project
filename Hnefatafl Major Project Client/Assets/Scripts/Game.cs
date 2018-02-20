﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public Board board;

    public int size = 11;

    public Vector2[] knightPos;
    public Vector2[] barbarianPos;

    public GameObject selectedPiece = null;

    public bool isFirstPlayer = true;

    public bool isBarbarians = false;

    public string Player1 = "";
    public string Player2 = "";

    public GameData gameData = null;

    public int knightsTaken = 0;
    public int barbariansTaken = 0;

    // Use this for initialization
    void Start()
    {

        Setup();


    }

    public void Awake1()
    {
        gameData = GameObject.FindGameObjectWithTag("game_data").GetComponent<GameData>();
        if (gameData != null)
        {
            Player1 = gameData.Player1;
            Player2 = gameData.Player2;

            if (Player1 == "Barbarian")
            {
                isBarbarians = true;
            }
            else
            {
                isBarbarians = false;
            }

            Setup();

        }
    }


    public void Setup()
    {
        knightPos = new Vector2[12] { new Vector2(5, 3), new Vector2(4, 4), new Vector2(5, 4), new Vector2(6, 4), new Vector2(3, 5), new Vector2(4, 5), new Vector2(6, 5), new Vector2(7, 5), new Vector2(4, 6), new Vector2(5, 6), new Vector2(6, 6), new Vector2(5, 7) };

        barbarianPos = new Vector2[24] { new Vector2(3, 0), new Vector2(4, 0), new Vector2(5, 0), new Vector2(6, 0), new Vector2(7, 0), new Vector2(5, 1), new Vector2(0, 3), new Vector2(0, 4), new Vector2(0, 5), new Vector2(0, 6), new Vector2(0, 7), new Vector2(1, 5), new Vector2(10, 3), new Vector2(10, 4), new Vector2(10, 5), new Vector2(10, 6), new Vector2(10, 7), new Vector2(9, 5), new Vector2(5, 9), new Vector2(3, 10), new Vector2(4, 10), new Vector2(5, 10), new Vector2(6, 10), new Vector2(7, 10) };

        board.Generate(size);
        SetupPieces();
    }


    public void NextTurn()
    {
        isBarbarians = !isBarbarians;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedPiece != null)
        {

        }
    }

    public void CheckForTaken()
    {
        if(isBarbarians){
            foreach(Vector2 knight in knightPos){
                
            }
        }else{
            foreach(Vector2 barbarian in barbarianPos){

            }
        }
    }

    void SetupPieces()
    {
        Material kingMat = (Material)Resources.Load("Materials/KingMat");
        Material knightMat = (Material)Resources.Load("Materials/KnightMat");
        Material barbarianMat = (Material)Resources.Load("Materials/BarbarianMat");

        Material kingSelMat = (Material)Resources.Load("Materials/KingSelectedMat");
        Material knightSelMat = (Material)Resources.Load("Materials/KnightSelectedMat");
        Material barbarianSelMat = (Material)Resources.Load("Materials/BarbarianSelectedMat");

        GameObject piece = (GameObject)Resources.Load("Prefabs/GamePiece");

        GameObject king = GameObject.Instantiate(piece);
        king.GetComponent<Renderer>().material = kingMat;
        king.name = "King";
        king.transform.position = new Vector3(size / 2, 0.5f, size / 2);
        king.AddComponent<Selectable>();
        king.GetComponent<Selectable>().normalMat = kingMat;
        king.GetComponent<Selectable>().selectedMat = kingSelMat;
        king.GetComponent<Selectable>().myPosition = new Vector2(5, 5);
        king.GetComponent<Selectable>().piece = Piece.King;
        king.gameObject.tag = "king";
        board.board[5, 5] = king;

        king.transform.parent = GameObject.Find("Tile x=5 z=5").transform;

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
                goKnight.AddComponent<Selectable>();
                goKnight.GetComponent<Selectable>().normalMat = knightMat;
                goKnight.GetComponent<Selectable>().selectedMat = knightSelMat;
                goKnight.GetComponent<Selectable>().piece = Piece.Knight;

                goKnight.GetComponent<Selectable>().myPosition = new Vector2(knight.x, knight.y);
                goKnight.gameObject.tag = "knight";
                string tempSearchName = "Tile x=" + knight.x + " z=" + knight.y;
                goKnight.transform.parent = GameObject.Find(tempSearchName).transform;

                board.board[(int)knight.x, (int)knight.y] = goKnight;

                index++;
            }

            index = 1;

            foreach (Vector2 barbarian in barbarianPos)
            {
                GameObject goBarbarian = GameObject.Instantiate(piece);
                goBarbarian.GetComponent<Renderer>().material = barbarianMat;
                goBarbarian.name = "Barbarian " + index;
                goBarbarian.transform.position = new Vector3(barbarian.x, 0.5f, barbarian.y);
                goBarbarian.SetActive(true);
                goBarbarian.AddComponent<Selectable>();
                goBarbarian.GetComponent<Selectable>().normalMat = barbarianMat;
                goBarbarian.GetComponent<Selectable>().selectedMat = barbarianSelMat;
                goBarbarian.GetComponent<Selectable>().piece = Piece.Barbarian;

                goBarbarian.GetComponent<Selectable>().myPosition = new Vector2(barbarian.x, barbarian.y);
                goBarbarian.gameObject.tag = "barbarian";
                string tempSearchName = "Tile x=" + barbarian.x + " z=" + barbarian.y;
                goBarbarian.transform.parent = GameObject.Find(tempSearchName).transform;

                board.board[(int)barbarian.x, (int)barbarian.y] = goBarbarian;

                index++;
            }


        }


    }


}
