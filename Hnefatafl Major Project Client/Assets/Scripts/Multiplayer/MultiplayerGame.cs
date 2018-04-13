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
                turnlabel.text = "Turn: Your Vikings";
            }
            else
            {
                turnlabel.text = "Turn: Your Barbarians";
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

    public void WeLost()
    {
        if (netMan.ourTeam == Team.VIKING)
        {
            Debug.Log("Enemy Barbarians win!!! We lose");
            gameUI.oBarbariansWin = true;
        }
        else
        {
            Debug.Log("Enemy Vikings Win");
            gameUI.oVikingsWin= true;
        }
    }
    public new void takenAlgo(string side, Vector2 piece, bool isKing)
    {
        //Debug.Log(piece.x + " " + piece.y);

        bool isTaken = false;

        Vector2 north, east, south, west;
        north = new Vector2(piece.x, piece.y + 1);
        east = new Vector2(piece.x + 1, piece.y);
        south = new Vector2(piece.x, piece.y - 1);
        west = new Vector2(piece.x - 1, piece.y);

        bool northNull = false, eastNull = false, southNull = false, westNull = false;
        bool northPiece = false, eastPiece = false, southPiece = false, westPiece = false;

        if (north.x < 0 || north.y < 0 || north.x >= size || north.y >= size)
        {
            northNull = true;
            //Debug.Log("North Null");
        }
        else if (board.board[(int)north.x, (int)north.y] != null)
        {
            if (board.board[(int)north.x, (int)north.y].gameObject.tag.Equals(side))
            {
                //Debug.Log("Theres a north piece");
                northPiece = true;
            }
        }

        if (south.x < 0 || south.y < 0 || south.x >= size || south.y >= size)
        {
            southNull = true;
            //Debug.Log("South Null");
        }
        else if (board.board[(int)south.x, (int)south.y] != null)
        {
            if (board.board[(int)south.x, (int)south.y].gameObject.tag.Equals(side))
            {
                //Debug.Log("Theres a south piece");
                southPiece = true;
            }
        }

        if (east.x < 0 || east.y < 0 || east.x >= size || east.y >= size)
        {
            eastNull = true;
            //Debug.Log("East Null");
        }
        else if (board.board[(int)east.x, (int)east.y] != null)
        {
            if (board.board[(int)east.x, (int)east.y].gameObject.tag.Equals(side))
            {
                //Debug.Log("Theres an east piece");
                eastPiece = true;
            }
        }

        if (west.x < 0 || west.y < 0 || west.x >= size || west.y >= size)
        {
            westNull = true;
            //Debug.Log("West Null");
        }
        else if (board.board[(int)west.x, (int)west.y] != null)
        {
            if (board.board[(int)west.x, (int)west.y].gameObject.tag.Equals(side))
            {
                //Debug.Log("There a west piece");
                westPiece = true;
            }
        }

        if (northPiece && southPiece && !isKing)
        {
            isTaken = true;
        }
        if (eastPiece && westPiece && !isKing)
        {
            isTaken = true;
        }

        if ((northPiece || northNull) && (eastPiece || eastNull) && (southPiece || southNull) && (westPiece || westNull) && isKing)
        {
            isTaken = true;
        }


        if (isTaken)
        {
            GameObject.Destroy(board.board[(int)piece.x, (int)piece.y]);

            if (side == "barbarian" && !isKing)
            {

                //knightPos.RemoveAt(knightPos.FindIndex(x => x.x == piece.x && x.y == piece.y));
                removalList.Add(new Vector2(piece.x, piece.y));

                knightsTaken++;
            }
            else if (side == "knight" && !isKing)
            {
                //barbarianPos.RemoveAt(barbarianPos.FindIndex(x => x.x == piece.x && x.y == piece.y));
                removalList.Add(new Vector2(piece.x, piece.y));
                barbariansTaken++;
            }
            else if (isKing)
            {
                if(netMan.ourTeam == Team.BARBARIAN)
                {
                    Debug.Log("Barbarians win!!!");
                    gameUI.barbariansWin = true;
                    netMan.weWon = true;
                }
                
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
