using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Dimensions of the board
    int width, height;

    //Material references that will be used when generating tiles
    Material creamMat;
    Material brownMat;
    Material cornerMat;
    Material throneMat;

    //2D reference to the board
	public GameObject[,] board = new GameObject[11,11];

    //The blank object to be turned into different tiles
    GameObject tilePrefab;

    //Initialise the board
    public void Generate(int size)
    {   
        //if the size is odd, set the width and height equal to it
        if (size % 2 != 0)
        {
            width = size;
            height = size;
        }
        else
        {
            Debug.Log("Error that size is not an odd number");
        }

        //Load the Material resources
        creamMat = (Material)Resources.Load("Materials/CreamMat");
        brownMat = (Material)Resources.Load("Materials/BrownMat");
        cornerMat = (Material)Resources.Load("Materials/CornerMat");
        throneMat = (Material)Resources.Load("Materials/ThroneMat");
        //Load the tile object
        tilePrefab = (GameObject)Resources.Load("Prefabs/BoardTile");

        //Set every position on the board to null before generating tiles
		for(int i = 0; i< 11; i++){
			for(int j = 0; j < 11; j++){
				board[i,j] = null;
			}
		}

        GenerateBoard();
    }

    //Create a tile for each space
    public void GenerateBoard()
    {   
        //This variable controls the checkered pattern of the board
        bool isBrown = true;
        //Go down the width
        for (int i = 0; i < width; i++)
        {
            //Go down the hight
            for (int j = 0; j < height; j++)
            {
                //Create an object that is a copy of the tile retrieved earlier, we will edit some feature in a minute
                GameObject go = GameObject.Instantiate(tilePrefab);
                //If true, change the colour to be brown
                if (isBrown)
                {
                    go.GetComponent<Renderer>().material = brownMat;
					go.tag = "main_board";

                }
                //Otherwise set it to cream
                else
                {
                    go.GetComponent<Renderer>().material = creamMat;
					go.tag = "main_board";
                }
                //If it is a corner then it must have its own special colour
               if (isCorner(i, j))
                {
                    go.GetComponent<Renderer>().material = cornerMat;
					go.tag = "corner";
                }
                //If the cooridinates are in the middle of the board, this is the throne
                else if (i == width / 2 && j == width / 2)
                {
					go.GetComponent<Renderer>().material = throneMat;
					go.tag = "throne";
                }
                
                //Flip the checkered bool, for the next tile
                isBrown = !isBrown;

                //Set the position of the tile equal to the width and height
                go.transform.position = new Vector3(i, 0, j);
                //Give it a relevant name
                go.name = "Tile x=" + i + " z=" + j;
                //Attach it to this object 
				go.transform.parent = this.transform;
                //Enable it
                go.SetActive(true);
				

            }
        }
    }

    //Determines if the provided coordinates are a corner
    private bool isCorner(int x, int y)
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

        return false;
    }

}
