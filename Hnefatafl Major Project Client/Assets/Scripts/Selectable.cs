using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{

    public Game gameController;

    public Vector2 myPosition;

    public Material normalMat;
    public Material selectedMat;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.selectedPiece != this.transform.gameObject)
        {
            //GetComponent<cakeslice.Outline>().enabled = false;
            GetComponent<Renderer>().material = normalMat;
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("Enter");
    }

    void OnMouseOver()
    {
        Debug.Log("Over");
        if (Input.GetMouseButton(0))
        {
            gameController.selectedPiece = this.transform.gameObject;
            GetComponent<Renderer>().material = selectedMat;
            FindPossibleMoves();
        }
    }

    void OnMouseExit()
    {
        Debug.Log("Exit");
    }

    void FindPossibleMoves()
    {
        GameObject[,] board = gameController.board.board;
        Vector2 north, south, east, west;

		List<Vector2> directions = new List<Vector2>(); 

        if (myPosition.x == 0)
        {
            north = new Vector2(-1, -1);
        }else{
			north = new Vector2(myPosition.x - 1, myPosition.y);
			directions.Add(north);
		}

        if (myPosition.y == 0)
        {
            west = new Vector2(-1, -1);
        }else{
			west = new Vector2(myPosition.x, myPosition.y -1);
			directions.Add(west);
		}

        if (myPosition.x == gameController.size)
        {
            south = new Vector2(-1, -1);
        }else{
			south = new Vector2(myPosition.x + 1, myPosition.y);
			directions.Add(south);
		}

        if (myPosition.y == gameController.size)
        {
            east = new Vector2(-1, -1);
        }else{
			east = new Vector2(myPosition.x, myPosition.y + 1);
			directions.Add(east);
		}




    }

}
