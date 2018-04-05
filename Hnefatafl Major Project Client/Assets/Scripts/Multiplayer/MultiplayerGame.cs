using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGame : Game
{
    public NetManager netMan = null;

    

    public bool ourTurn = false;

    public new void Awake()
    {
        kingMat = (Material)Resources.Load("Materials/KingMat");
        knightMat = (Material)Resources.Load("Materials/KnightMat");
        barbarianMat = (Material)Resources.Load("Materials/BarbarianMat");

        kingSelMat = (Material)Resources.Load("Materials/KingSelectedMat");
        knightSelMat = (Material)Resources.Load("Materials/KnightSelectedMat");
        barbarianSelMat = (Material)Resources.Load("Materials/BarbarianSelectedMat");

        piece = (GameObject)Resources.Load("Prefabs/GamePiece");

        netMan = GameObject.FindGameObjectWithTag("net_man").GetComponent<NetManager>();
        if (netMan != null)
        {
            if (netMan.areWeFirst)
            {
                Player1 = netMan.myId.ToString();
                Player2 = netMan.opponentId.ToString();
                isFirstPlayer = true;
            }
            else
            {
                Player1 = netMan.opponentId.ToString();
                Player2 = netMan.myId.ToString();
                isFirstPlayer = false;
            }

            if (netMan.ourTeam == Team.BARBARIAN)
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

    public new void Setup()
    {

        FillKnightPos();
        FillBarbarianPos();
        kingPos = new Vector2(5, 5);

        board.Generate(size);
        SetupPieces();
    }

    public void Update()
    {
        knightsTaken = 12 - knightPos.Count;
        barbariansTaken = 24 - barbarianPos.Count;

        if (ourTurn)
        {
            if (netMan.ourTeam == Team.VIKING)
            {
                turnlabel.text = "Turn: Your Barbarians";
            }
            else
            {
                turnlabel.text = "Turn: Your Vikings";
            }

        }
        else
        {   if(netMan.ourTeam == Team.VIKING)
            {
                turnlabel.text = "Turn: Enemy Barbarians";
            }
            else
            {
                turnlabel.text = "Turn: Enemy Vikings";
            }
            
        }
        vikingsTakenLabel.text = "Vikings Taken: " + knightsTaken.ToString();
        barbariansTakenLabel.text = "Barbarians Taken: " + barbariansTaken.ToString();
    }

     public new void CheckForTaken()
    {
        Debug.Log("Checking for taken");
        if(netMan.ourTeam == Team.BARBARIAN){
            foreach (Vector2 knight in knightPos)
            {
                takenAlgo("barbarian", knight, false);
            }

            takenAlgo("barbarian", kingPos, true);
        }else{
            foreach (Vector2 barbarian in barbarianPos)
            {
                takenAlgo("knight", barbarian, false);
            }
        }
        
    }

    public new void NextTurn()
    {
        Debug.Log("Next turn");
        netMan.nextTurn = true;
        ourTurn = false;
    }

    public void MovePieces()
    {
        board.board = new GameObject[11, 11];
        GameObject.Destroy(GameObject.FindGameObjectWithTag("king"));
        foreach(GameObject knight in GameObject.FindGameObjectsWithTag("knight")){
            GameObject.Destroy(knight);
        }
        foreach(GameObject barbarian in GameObject.FindGameObjectsWithTag("barbarian")){
            GameObject.Destroy(barbarian);
        }

        float kingx = kingPos.x;
        float kingy = kingPos.y;

        GameObject king = GameObject.Instantiate(piece);
        king.GetComponent<Renderer>().material = kingMat;
        king.name = "King";
        king.transform.position = new Vector3(kingx, 0.5f, kingy);
        if(netMan.ourTeam == Team.VIKING)
        {
            king.AddComponent<Selectable>();
            king.GetComponent<Selectable>().normalMat = kingMat;
            king.GetComponent<Selectable>().selectedMat = kingSelMat;
            king.GetComponent<Selectable>().myPosition = new Vector2(kingx, kingy);
            king.GetComponent<Selectable>().piece = Piece.King;
        }
        
        king.gameObject.tag = "king";
        board.board[(int)kingx, (int)kingy] = king;
        
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
                if(netMan.ourTeam == Team.VIKING)
                {
                    goKnight.AddComponent<Selectable>();
                    goKnight.GetComponent<Selectable>().normalMat = knightMat;
                    goKnight.GetComponent<Selectable>().selectedMat = knightSelMat;
                    goKnight.GetComponent<Selectable>().piece = Piece.Knight;
                    goKnight.GetComponent<Selectable>().myPosition = new Vector2(knight.x, knight.y);
                }
                
                goKnight.gameObject.tag = "knight";
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
                if(netMan.ourTeam == Team.BARBARIAN)
                {
                    goBarbarian.AddComponent<Selectable>();
                    goBarbarian.GetComponent<Selectable>().normalMat = barbarianMat;
                    goBarbarian.GetComponent<Selectable>().selectedMat = barbarianSelMat;
                    goBarbarian.GetComponent<Selectable>().piece = Piece.Barbarian;
                    goBarbarian.GetComponent<Selectable>().myPosition = new Vector2(barbarian.x, barbarian.y);
                }
                
                goBarbarian.gameObject.tag = "barbarian";
                board.board[(int)barbarian.x, (int)barbarian.y] = goBarbarian;
                index++;
            }

        }
    }

}
