using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    //Reference to the board
    public Board board;
    //Size of the board
    public int size = 11;
    //List that contain the position of the pieces
    public List<Vector2> knightPos = new List<Vector2>();
    public List<Vector2> barbarianPos = new List<Vector2>();
    //The kings position
    public Vector2 kingPos;
    //The currently selected piece
    public GameObject selectedPiece = null;
    //Are we the first player
    public bool isFirstPlayer = true;
    //Is the current turn barbarians or vikings
    public bool isBarbarians = false;
    //The names of the two players
    public string Player1 = "";
    public string Player2 = "";
    //Get a reference of the menu data
    public MenuData gameData = null;
    //Number of each side taken
    public int knightsTaken = 0;
    public int barbariansTaken = 0;
    //The game ui object
    public GameUI gameUI;
    //The materials for the pieces
    public Material kingMat, knightMat, barbarianMat, kingSelMat, knightSelMat, barbarianSelMat;
    //The prefab of a piece
    public GameObject piece;

    public GameObject barbarianPiece;
    public GameObject knightPiece;
    public GameObject kingPiece;

    //Text objects for the user interface
    public TMP_Text turnlabel;
    public TMP_Text vikingsTakenLabel;
    public TMP_Text barbariansTakenLabel;


    public void Start()
    {
        //Get the text elements and assign them
        turnlabel = GameObject.FindGameObjectWithTag("turn_label").GetComponent<TMP_Text>();
        vikingsTakenLabel = GameObject.FindGameObjectWithTag("vikings_taken").GetComponent<TMP_Text>();
        barbariansTakenLabel = GameObject.FindGameObjectWithTag("barbarians_taken").GetComponent<TMP_Text>();
    }

    public void Awake()
    {   
        //Load the materials
        kingMat = (Material)Resources.Load("Materials/KingMat");
        knightMat = (Material)Resources.Load("Materials/KnightMat");
        barbarianMat = (Material)Resources.Load("Materials/BarbarianMat");
        kingSelMat = (Material)Resources.Load("Materials/KingSelectedMat");
        knightSelMat = (Material)Resources.Load("Materials/KnightSelectedMat");
        barbarianSelMat = (Material)Resources.Load("Materials/BarbarianSelectedMat");
        //Load the piece prefab
        piece = (GameObject)Resources.Load("Prefabs/GamePiece");
        
        barbarianPiece = (GameObject)Resources.Load("Prefabs/Character_Warrior_White");
        knightPiece = (GameObject)Resources.Load("Prefabs/Character_Soldier_01_Yellow");
        kingPiece = (GameObject)Resources.Load("Prefabs/Character_Knight_01_Yellow");
            
        
        //Get the game data
        gameData = GameObject.FindGameObjectWithTag("game_data").GetComponent<MenuData>();
        if (gameData != null)
        {   
            //Assign the first player team
            if (gameData.Player1 == Team.BARBARIAN)
            {
                isBarbarians = true;
            }
            else
            {
                isBarbarians = false;
            }

            //Setup the board
            Setup();

        }
    }

    public void Setup()
    {
        //Destroy the game data now
        GameObject.Destroy(gameData);

        //Fill the positions of the pieces
        FillKnightPos();
        FillBarbarianPos();
        kingPos = new Vector2(5, 5);
        //Generate a board
        board.Generate(size);
        //Create the pieces
        SetupPieces();
    }

    //The set positions for the vikings
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

    //The set positions for the barbarians
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
    
    //Flip the turn
    public void NextTurn()
    {
        isBarbarians = !isBarbarians;
    }

    // Update is called once per frame
    void Update()
    {
        //Changed the pieces taken messages relative to the size of the corresponding lists
        knightsTaken = 12 - knightPos.Count;
        barbariansTaken = 24 - barbarianPos.Count;

        if (isBarbarians)
        {
            turnlabel.text = "Turn: Barbarians";

        }
        else
        {
            turnlabel.text = "Turn: Vikings";
        }
        vikingsTakenLabel.text = "Vikings Taken: " + knightsTaken.ToString();
        barbariansTakenLabel.text = "Barbarians Taken: " + barbariansTaken.ToString();
    }

    //Pieces that need removing from the board
    public List<Vector2> removalList = new List<Vector2>( );

    //Checks if there are any peices that have been taken and adds them to the removal list
    public void CheckForTaken()
    {
        removalList = new List<Vector2>();

        if (isBarbarians)
        {
            foreach (Vector2 knight in knightPos)
            {
                takenAlgo("barbarian", knight, false);
            }

            takenAlgo("barbarian", kingPos, true);
            foreach(Vector2 piece in removalList){
                knightPos.RemoveAt(knightPos.FindIndex(x => x.x == piece.x && x.y == piece.y));
             
            }
        }
        else
        {
            foreach (Vector2 barbarian in barbarianPos)
            {
                takenAlgo("knight", barbarian, false);

            }
            foreach(Vector2 piece in removalList){
               barbarianPos.RemoveAt(barbarianPos.FindIndex(x => x.x == piece.x && x.y == piece.y));
                
            }
        }
    }

    //The main algorithm that checks if a piece has been taken
    public void takenAlgo(string side, Vector2 piece, bool isKing)
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
                Debug.Log("Barbarians win!!!");
                gameUI.BarbariansWin();
            }


        }


    }

    //Create the pieces for the game board
    public void SetupPieces()
    {

        GameObject king = GameObject.Instantiate(kingPiece);
        //king.GetComponent<Renderer>().material = kingMat;
        king.name = "King";
        king.transform.position = new Vector3(size / 2, 0.5f, size / 2);
        king.AddComponent<Selectable>();
        //king.GetComponent<Selectable>().normalMat = kingMat;
        //king.GetComponent<Selectable>().selectedMat = kingSelMat;
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
                GameObject goKnight = GameObject.Instantiate(knightPiece);
                //goKnight.GetComponent<Renderer>().material = knightMat;
                goKnight.name = "Knight " + index;
                goKnight.transform.position = new Vector3(knight.x, 0.5f, knight.y);
                goKnight.SetActive(true);
                goKnight.AddComponent<Selectable>();
                //goKnight.GetComponent<Selectable>().normalMat = knightMat;
                //goKnight.GetComponent<Selectable>().selectedMat = knightSelMat;
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
                GameObject goBarbarian = GameObject.Instantiate(barbarianPiece);
                //goBarbarian.GetComponent<Renderer>().material = barbarianMat;
                goBarbarian.name = "Barbarian " + index;
                goBarbarian.transform.position = new Vector3(barbarian.x, 0.5f, barbarian.y);
                goBarbarian.SetActive(true);
                goBarbarian.AddComponent<Selectable>();
                //goBarbarian.GetComponent<Selectable>().normalMat = barbarianMat;
                //goBarbarian.GetComponent<Selectable>().selectedMat = barbarianSelMat;
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
