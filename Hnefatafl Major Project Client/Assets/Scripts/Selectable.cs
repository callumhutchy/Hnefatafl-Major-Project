using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    //Reference to the game
    public Game gameController;
    
    //What piece we are
    public Piece piece;

    //Our current position
    public Vector2 myPosition;

    //Our selected and unselected material
    public Material normalMat;
    public Material selectedMat;

    //The movement tile prefab
    public GameObject movementTile;

    //Whether this is a netgame
    public bool netGame = false;

    //List of our movement tiles
    public List<GameObject> selectableTiles = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        //Establish whether this is an online game
        if (GameObject.FindGameObjectWithTag("net_man"))
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MultiplayerGame>();
            netGame = true;
        }
        else
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        }

        //Load the movement tile prefab
        movementTile = (GameObject)Resources.Load("Prefabs/MovementPlace");
    }

    // Update is called once per frame
    void Update()
    {
        //If the piece is not selected, change the material to the normal mat and delete the movement tiles
        if (gameController.selectedPiece != this.transform.gameObject)
        {
            GetComponent<Renderer>().material = normalMat;
            foreach (GameObject go in selectableTiles)
            {
                Object.Destroy(go);
            }
        }


    }

    void OnMouseOver()
    {
        //If its a multiplayer game then we have to handle selection differently
        if (netGame)
        {
            Debug.Log("Mouing over");
            MultiplayerGame game = (MultiplayerGame)gameController;
            if (game.ourTurn)
            {
                Debug.Log("Its our turn");
                if (Input.GetMouseButtonDown(0) && ((game.netMan.ourTeam.ToString().ToLower() == "barbarian" && this.gameObject.tag.Equals("barbarian")) || (game.netMan.ourTeam.ToString().ToLower() != "barbarian" && !this.gameObject.tag.Equals("barbarian"))))
                {
                    Debug.Log("Clickity");
                    gameController.selectedPiece = this.transform.gameObject;
                    GetComponent<Renderer>().material = selectedMat;
                    FindPossibleMoves();

                }
            }else{
                Debug.Log("Hmmm");
            }

        }
        //If its not an online game we just change selection using a simple toggle
        else
        {
            if (Input.GetMouseButtonDown(0) && ((gameController.isBarbarians && this.gameObject.tag.Equals("barbarian")) || (!gameController.isBarbarians && !this.gameObject.tag.Equals("barbarian"))))
            {
                gameController.selectedPiece = this.transform.gameObject;
                GetComponent<Renderer>().material = selectedMat;
                FindPossibleMoves();

            }
        }

    }

    void FindPossibleMoves()
    {
        GameObject[,] board = gameController.board.board;
        List<string> directions = new List<string>();

        //Check for all the null positions in the 4 directions
        //Is tile west null
        if (myPosition.x == 0)
        {
            //Debug.Log("West null");
        }
        else
        {
            if (board[(int)myPosition.x - 1, (int)myPosition.y] == null)
            {
                directions.Add("west");
            }

        }

        //Is tile north null
        if (myPosition.y == gameController.size - 1)
        {
            //Debug.Log("North Null");
        }
        else
        {
            if (board[(int)myPosition.x, (int)myPosition.y + 1] == null)
            {
                directions.Add("north");
            }

        }

        //Is tile east null
        if (myPosition.x == gameController.size - 1)
        {
            //Debug.Log("East Null");
        }
        else
        {
            if (board[(int)myPosition.x + 1, (int)myPosition.y] == null)
            {
                directions.Add("east");
            }

        }

        //Is tile south null
        if (myPosition.y == 0)
        {
            //Debug.Log("South Null");
        }
        else
        {
            if (board[(int)myPosition.x, (int)myPosition.y - 1] == null)
            {
                directions.Add("south");
            }
        }

        if (myPosition.x < 0 || myPosition.x >= gameController.size || myPosition.y < 0 || myPosition.y >= gameController.size)
        {
            return;
        }

        //Generate movement tiles for all of the directions add the the direction list
        
        if (directions.Contains("south"))
        {

            int index = 1;
            bool indexIsOOB = false;
            while (!indexIsOOB && board[(int)myPosition.x, (int)myPosition.y - index] == null && ((!IsCorner((int)myPosition.x, (int)myPosition.y - index, gameController.size) && piece != Piece.King) || piece == Piece.King))
            {

                GenerateTile((int)myPosition.x, (int)myPosition.y - index);

                index++;

                if (index >= gameController.size || myPosition.y - index < 0)
                {
                    indexIsOOB = true;
                }
            }

        }

        if (directions.Contains("north"))
        {

            int index = 1;

            bool indexIsOOB = false;
            while (!indexIsOOB && board[(int)myPosition.x, (int)myPosition.y + index] == null && ((!IsCorner((int)myPosition.x, (int)myPosition.y + index, gameController.size) && piece != Piece.King) || piece == Piece.King))
            {
                GenerateTile((int)myPosition.x, (int)myPosition.y + index);
                index++;
                if (index >= gameController.size || myPosition.y + index >= gameController.size)
                {
                    indexIsOOB = true;
                }

            }



        }

        if (directions.Contains("east"))
        {

            int index = 1;

            bool indexIsOOB = false;
            while (!indexIsOOB && board[(int)myPosition.x + index, (int)myPosition.y] == null && ((!IsCorner((int)myPosition.x + index, (int)myPosition.y, gameController.size) && piece != Piece.King) || piece == Piece.King))
            {

                GenerateTile((int)myPosition.x + index, (int)myPosition.y);
                index++;
                if (index >= gameController.size || myPosition.x + index >= gameController.size)
                {
                    indexIsOOB = true;
                }
            }



        }

        if (directions.Contains("west"))
        {
            int index = 1;

            bool indexIsOOB = false;
            while (!indexIsOOB && board[(int)myPosition.x - index, (int)myPosition.y] == null && ((!IsCorner((int)myPosition.x - index, (int)myPosition.y, gameController.size) && piece != Piece.King) || piece == Piece.King))
            {
                GenerateTile((int)myPosition.x - index, (int)myPosition.y);
                index++;
                if (index >= gameController.size || myPosition.x - index < 0)
                {
                    indexIsOOB = true;
                }
            }

        }

    }

    //Changed the material to the normal material and unselect the piece
    void SetNormal()
    {
        gameController.selectedPiece = null;
        GetComponent<Renderer>().material = normalMat;
    }

    public void MoveToLocation(Transform tran)
    {
        Debug.Log("Size of the lists before moving " + gameController.knightPos.Count + ":" + gameController.barbarianPos.Count);
        
        //Destroy the movement tiles
        foreach (GameObject go in selectableTiles)
        {
            Object.Destroy(go);
        }

        //The old location on the board is now free
        gameController.board.board[(int)this.transform.position.x, (int)this.transform.position.z] = null;

        //Depending on which piece it is, remove it from its old positon array and re add it a the new position
        if (this.gameObject.tag.Equals("barbarian"))
        {
            gameController.barbarianPos.RemoveAt(gameController.barbarianPos.FindIndex(x => x.x == this.transform.position.x && x.y == this.transform.position.z));
            gameController.barbarianPos.Add(new Vector2(tran.position.x, tran.position.z));
        }
        else if (this.gameObject.tag.Equals("knight"))
        {
           
            gameController.knightPos.RemoveAt(gameController.knightPos.FindIndex(x => x.x == this.transform.position.x && x.y == this.transform.position.z));
            gameController.knightPos.Add(new Vector2(tran.position.x, tran.position.z));
        }
        else if (this.gameObject.tag.Equals("king"))
        {
            gameController.kingPos = new Vector2(tran.position.x, tran.position.z);
            if (IsCorner((int)gameController.kingPos.x, (int)gameController.kingPos.y, 11) && !IsThrone((int)gameController.kingPos.x, (int)gameController.kingPos.y))
            {
                if (netGame)
                {
                    Debug.Log("Knights Win");
                    gameController.gameUI.vikingsWin = true;
                    MultiplayerGame mg = (MultiplayerGame)gameController;
                    mg.netMan.weWon = true;
                }
                else
                {
                    Debug.Log("Knights Win");
                    gameController.gameUI.VikingsWin();
                }
                

            }
        }

        //Assign the new position to the 3D object
        this.transform.position = new Vector3( tran.position.x, 0.5f, tran.position.z);
        //Save our 2d locaton
        myPosition = new Vector2(tran.position.x, tran.position.z);
        //Add the new object to the board
        gameController.board.board[(int)myPosition.x, (int)myPosition.y] = this.gameObject;
        //Set the piece back to normal
        SetNormal();
        //Now check if any pieces were taken in the game
        gameController.CheckForTaken();

        //Tell the server we have had out turn if it is a multiplayer game
        if(netGame){
            Debug.Log("Moved piece");
            MultiplayerGame game = (MultiplayerGame) gameController;
            game.NextTurn();
        }else{
            gameController.NextTurn();
        }
        Debug.Log("Size of the lists after moving " + gameController.knightPos.Count + ":" + gameController.barbarianPos.Count);
    }

    //Generate a movement tile at the specified location
    void GenerateTile(int posX, int posY)
    {
        GameObject g2 = GameObject.Instantiate(movementTile);
        g2.transform.position = new Vector3(posX, 0.25f, posY);
        g2.AddComponent<MovementTile>();
        g2.GetComponent<MovementTile>().owner = this.gameObject;
        selectableTiles.Add(g2);
    }

    //Determinds if the coordinates are the corners
    bool IsCorner(int x, int y, int width)
    {
        if (x == 0 && y == 0)
        {
            return true;
        }
        else if (x == 0 && y == width - 1)
        {
            return true;
        }
        else if (y == 0 && x == width - 1)
        {
            return true;
        }
        else if (x == width - 1 && y == width - 1)
        {
            return true;
        }
        else if (x == 5 && y == 5)
        {
            return true;
        }

        return false;
    }

    //Determines if the coordinates are the throne
    bool IsThrone(int x, int y)
    {
        if (x == 5 && y == 5)
        {
            return true;
        }
        return false;
    }


}

public enum Piece
{
    King,
    Barbarian,
    Knight
}