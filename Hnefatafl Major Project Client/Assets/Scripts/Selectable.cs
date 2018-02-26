using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{

    public Game gameController;

    public Piece piece;

    public Vector2 myPosition;

    public Material normalMat;
    public Material selectedMat;

    public GameObject movementTile;

    public List<GameObject> selectableTiles = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        movementTile = (GameObject)Resources.Load("Prefabs/MovementPlace");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.selectedPiece != this.transform.gameObject)
        {
            //GetComponent<cakeslice.Outline>().enabled = false;
            GetComponent<Renderer>().material = normalMat;
            foreach (GameObject go in selectableTiles)
            {
                DestroyObject(go);
            }
        }


    }


    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && ((gameController.isBarbarians && this.gameObject.tag.Equals("barbarian")) || (!gameController.isBarbarians && !this.gameObject.tag.Equals("barbarian"))))
        {

            gameController.selectedPiece = this.transform.gameObject;
            GetComponent<Renderer>().material = selectedMat;
            FindPossibleMoves();

        }
    }

    void FindPossibleMoves()
    {
        GameObject[,] board = gameController.board.board;
        List<string> directions = new List<string>();

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

    void SetNormal()
    {
        gameController.selectedPiece = null;
        GetComponent<Renderer>().material = normalMat;
    }

    public void MoveToLocation(Transform tran)
    {
        foreach (GameObject go in selectableTiles)
        {
            DestroyObject(go);
        }
        gameController.board.board[(int)this.transform.position.x, (int)this.transform.position.z] = null;
        this.transform.position = new Vector3(tran.position.x, 0.5f, tran.position.z);
        if (this.gameObject.tag.Equals("barbarian"))
        {
            gameController.barbarianPos.Remove(new Vector2(this.transform.position.x, this.transform.position.y));
            gameController.barbarianPos.Add(new Vector2(tran.position.x, tran.position.z));
        }
        else if (this.gameObject.tag.Equals("knight"))
        {
            gameController.knightPos.Remove(new Vector2(this.transform.position.x, this.transform.position.y));
            gameController.knightPos.Add(new Vector2(tran.position.x, tran.position.z));
        }
        else if (this.gameObject.tag.Equals("king"))
        {
            gameController.kingPos = new Vector2(tran.position.x, tran.position.z);
            if(IsCorner((int)gameController.kingPos.x, (int)gameController.kingPos.y, 11) && !IsThrone((int)gameController.kingPos.x, (int)gameController.kingPos.y)){
                Debug.Log("Knights Win");
            }
        }

        myPosition = new Vector2(tran.position.x, tran.position.z);

        gameController.board.board[(int)myPosition.x, (int)myPosition.y] = this.gameObject;

        SetNormal();
        gameController.CheckForTaken();
        gameController.NextTurn();

    }

    void GenerateTile(int posX, int posY)
    {
        GameObject g2 = GameObject.Instantiate(movementTile);
        g2.transform.position = new Vector3(posX, 0.25f, posY);
        g2.AddComponent<MovementTile>();
        g2.GetComponent<MovementTile>().owner = this.gameObject;
        selectableTiles.Add(g2);
    }

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
        }else if(x == 5 && y == 5){
            return true;
        }
       
        return false;
    }

    bool IsThrone(int x, int y){
        if(x == 5 && y == 5){
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