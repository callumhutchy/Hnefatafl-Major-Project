using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGame : Game {

    

    public NetManager netMan = null;

    public bool ourTurn = false;

    public new void Awake()
    {
        netMan = GameObject.FindGameObjectWithTag("net_man").GetComponent<NetManager>();
        if (netMan != null)
        {
			if(netMan.areWeFirst){
				Player1 = netMan.myId.ToString();
				Player2 = netMan.opponentId.ToString();
				isFirstPlayer = true;
			}else{
				Player1 = netMan.opponentId.ToString();
				Player2 = netMan.myId.ToString();
				isFirstPlayer = false;
			}

            if(netMan.ourTeam == Team.BARBARIAN){
                isBarbarians = true;
            }else{
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

       public void NextTurn()
    {
        netMan.nextTurn = true;
        ourTurn = false;
    }

    public void MovePieces(){

    }

}
