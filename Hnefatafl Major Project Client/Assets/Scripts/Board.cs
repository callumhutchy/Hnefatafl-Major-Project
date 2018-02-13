using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    int width, height;

    Material creamMat;
    Material brownMat;

    Material cornerMat;

    Material throneMat;
    public List<List<GameObject>> boardTiles = new List<List<GameObject>>();

	public GameObject[,] board = new GameObject[11,11];

    GameObject tilePrefab;

    //Initialise the board
    public void Generate(int size)
    {
        if (size % 2 != 0)
        {
            width = size;
            height = size;
        }
        else
        {
            Debug.Log("Error that size is not an odd number");
        }

        creamMat = (Material)Resources.Load("Materials/CreamMat");
        brownMat = (Material)Resources.Load("Materials/BrownMat");
        cornerMat = (Material)Resources.Load("Materials/CornerMat");
        throneMat = (Material)Resources.Load("Materials/ThroneMat");

        tilePrefab = (GameObject)Resources.Load("Prefabs/BoardTile");

		for(int i = 0; i< 11; i++){
			for(int j = 0; j < 11; j++){
				board[i,j] = null;
			}
		}

        GenerateBoard();
    }

    public void GenerateBoard()
    {
        bool isBrown = true;
        for (int i = 0; i < width; i++)
        {

            List<GameObject> row = new List<GameObject>();

            for (int j = 0; j < height; j++)
            {
                GameObject go = GameObject.Instantiate(tilePrefab);
                if (isBrown)
                {
                    go.GetComponent<Renderer>().material = brownMat;

                }else
                {
                    go.GetComponent<Renderer>().material = creamMat;
                }

               if (isCorner(i, j))
                {
                    go.GetComponent<Renderer>().material = cornerMat;
                }
                else if (i == width / 2 && j == width / 2)
                {
					go.GetComponent<Renderer>().material = throneMat;
                }
                
                isBrown = !isBrown;

                go.transform.position = new Vector3(i, 0, j);
                go.name = "Tile x=" + i + " z=" + j;
                row.Add(go);
				go.transform.parent = this.transform;
                go.SetActive(true);
				

            }
        }
    }

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
