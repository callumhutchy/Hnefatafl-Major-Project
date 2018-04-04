using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public Board board;

    public int size = 11;

    public List<Vector2> knightPos = new List<Vector2>();
    public List<Vector2> barbarianPos = new List<Vector2>();

    public Vector2 kingPos;

    public GameObject selectedPiece = null;

    public bool isFirstPlayer = true;

    public bool isBarbarians = false;

    public string Player1 = "";
    public string Player2 = "";

    public MenuData gameData = null;

    public int knightsTaken = 0;
    public int barbariansTaken = 0;

    public GameUI gameUI;

    // Use this for initialization
    void Start()
    {

       // Setup();


    }

    public void Awake()
    {
        gameData = GameObject.FindGameObjectWithTag("game_data").GetComponent<MenuData>();
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

        GameObject.Destroy(gameData);


        FillKnightPos();
        FillBarbarianPos();
        kingPos = new Vector2(5, 5);

        board.Generate(size);
        SetupPieces();
    }

    protected void FillKnightPos()
    {
        knightPos.Add(new Vector2(5, 3));
        knightPos.Add(new Vector2(4, 4));
        knightPos.Add(new Vector2(5, 4));
        knightPos.Add(new Vector2(6, 4));
        knightPos.Add(new Vector2(3, 5));
        knightPos.Add(new Vector2(4, 5));
        knightPos.Add(new Vector2(6, 5));
        knightPos.Add(new Vector2(7, 5));
        knightPos.Add(new Vector2(4, 6));
        knightPos.Add(new Vector2(5, 6));
        knightPos.Add(new Vector2(6, 6));
        knightPos.Add(new Vector2(5, 7));

    }

    protected void FillBarbarianPos()
    {
        barbarianPos.Add(new Vector2(3, 0));
        barbarianPos.Add(new Vector2(4, 0));
        barbarianPos.Add(new Vector2(5, 0));
        barbarianPos.Add(new Vector2(6, 0));
        barbarianPos.Add(new Vector2(7, 0));
        barbarianPos.Add(new Vector2(5, 1));
        barbarianPos.Add(new Vector2(0, 3));
        barbarianPos.Add(new Vector2(0, 4));
        barbarianPos.Add(new Vector2(0, 5));
        barbarianPos.Add(new Vector2(0, 6));
        barbarianPos.Add(new Vector2(0, 7));
        barbarianPos.Add(new Vector2(1, 5));
        barbarianPos.Add(new Vector2(10, 3));
        barbarianPos.Add(new Vector2(10, 4));
        barbarianPos.Add(new Vector2(10, 5));
        barbarianPos.Add(new Vector2(10, 6));
        barbarianPos.Add(new Vector2(10, 7));
        barbarianPos.Add(new Vector2(9, 5));
        barbarianPos.Add(new Vector2(5, 9));
        barbarianPos.Add(new Vector2(3, 10));
        barbarianPos.Add(new Vector2(4, 10));
        barbarianPos.Add(new Vector2(5, 10));
        barbarianPos.Add(new Vector2(6, 10));
        barbarianPos.Add(new Vector2(7, 10));
    }


    public void NextTurn()
    {
        isBarbarians = !isBarbarians;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForTaken()
    {
        if (isBarbarians)
        {
            foreach (Vector2 knight in knightPos)
            {
                takenAlgo("barbarian", knight, false);
            }

            takenAlgo("barbarian", kingPos, true);

        }
        else
        {
            foreach (Vector2 barbarian in barbarianPos)
            {
                takenAlgo("knight", barbarian, false);
            }
        }
    }

    protected void takenAlgo(string side, Vector2 piece, bool isKing)
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
                knightsTaken++;
            }
            else if (side == "knight" && !isKing)
            {
                barbariansTaken++;
            }
            else if (isKing)
            {
                Debug.Log("Barbarians win!!!");
                gameUI.BarbariansWin();
            }


        }


    }

    protected void SetupPieces()
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
