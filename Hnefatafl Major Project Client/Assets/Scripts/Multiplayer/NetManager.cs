using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

public class NetManager : MonoBehaviour
{
    private Socket clientSocket;
    private byte[] buffer;

    private const int port = 7995;

    public Guid myId;

    public Guid opponentId;
    public Guid gameId;

    public Guid clientId;

    public Team ourTeam;

    public bool areWeFirst;

    private bool loadMuliplayerGame = false;

    private bool loadMenu;
    public bool nextTurn;

    public MultiplayerGame multiplayerGame;

    public GameObject waitingIcon;
    public TMP_Text waitingText;
    public static bool waiting;

    public bool quit;

    public bool weWon;


    static bool sceneLoaded;


    public static string waitingString;


    static bool movePieces;


    bool allowQuitting;
    static bool exit;

    static bool disconnect;

    public void SetVariables()
    {
        disconnect = false;
        exit = false;
        allowQuitting = false;
        movePieces = false;
        sceneLoaded = false;
        weWon = false;
        quit = false;
        waiting = true;
        loadMenu = false;
        nextTurn = false;
        

    }

    void Awake()
    {
        SetVariables();


        GameObject.DontDestroyOnLoad(this);
        clientId = Guid.NewGuid();
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connect to server
            try{

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                // IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry("ec2-35-178-83-188.eu-west-2.compute.amazonaws.com").AddressList[0], port);
            Debug.Log("Attempting to connect");
            //Begin connection
            clientSocket.BeginConnect(endPoint, ConnectCallback, null);

            }catch(Exception ex){
                /*
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            Debug.Log("Attempting to connect");
            //Begin connection
            clientSocket.BeginConnect(endPoint, ConnectCallback, null);
            */
                Debug.Log(ex);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError(ex);
        }


    }
    
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        waiting = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name == "MultiplayerScene")
        {
            loadMuliplayerGame = false;
            sceneLoaded = true;
            multiplayerGame = GameObject.FindGameObjectWithTag("GameController").GetComponent<MultiplayerGame>();
            if(areWeFirst){
                multiplayerGame.ourTurn = true;
            }else{
                multiplayerGame.ourTurn = false;
            }

        }else if(scene.name == "Main Menu"){
            GameObject.Destroy(GameObject.FindGameObjectWithTag("net_man"));
            
        }
    }

    Thread receiveData;

    void Log(string text)
    {
        Debug.Log(text);
        if (waiting)
        {
            waitingString = text;
        }
        
    }

    //Start the callback loop
    void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            buffer = new byte[clientSocket.ReceiveBufferSize];
            Log("Connecting");


            Log("Sending client id : " + clientId.ToString());
            Send(new Message(MessageType.CONNECT, clientId.ToString()).Serialize(), clientSocket);

            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void LeaveQueueButton()
    {
        disconnect = true;
    }

    //This method will loop 
    void ReceiveCallback(IAsyncResult ar)
    {
                try
        {
            int received = clientSocket.EndReceive(ar);

            if (received == 0)
            {
                return;
            }

            string reply = Encoding.ASCII.GetString(buffer);
            //Logic goes here for response

            Message msg = Message.Deserialize(reply);
            Debug.Log(msg.type + " : " + msg.message);

            if (!myId.Equals(msg.userId))
            {
                Log("Our id from the server doesn't match");
            }

            if (disconnect)
            {
                Log("Sending a leave message");
                Send(new Message(MessageType.DISCONNECT, "Can we leave", myId, clientId).Serialize(), clientSocket);
                disconnect = false;
            }

            if (weWon)
            {
                Log("We won sending to server");
                BoardState board = new BoardState(multiplayerGame.knightPos, multiplayerGame.barbarianPos, multiplayerGame.kingPos);

                Send(new Message(MessageType.GAME_OVER, board.Serialize(), myId, clientId).Serialize(), clientSocket);
                return;
            }

            switch (msg.type)
            {
                case MessageType.CONNECT:

                    myId = msg.userId;
                    Log("We've connected and here is our id : " + myId);
                    Send(new Message(MessageType.FIND_GAME, "Please find a game", myId, clientId).Serialize(), clientSocket);

                    break;
                case MessageType.WAITING_FOR_PLAYER:
                    Log("We are waiting for another player");
                    System.Threading.Thread.Sleep(2000);
                    Send(new Message(MessageType.WAITING_FOR_PLAYER, "Ok we will wait", myId, clientId).Serialize(), clientSocket);
                    break;

                case MessageType.PLAYER_FOUND:
                    Log("Opponent found");
                    Send(new Message(MessageType.GAME_SETUP, msg.message, myId, clientId).Serialize(), clientSocket);

                    break;

                case MessageType.GAME_SETUP:

                    GameData gameData = GameData.Deserialise(msg.message);
                    Debug.Log(gameData.player1);
                    Debug.Log(gameData.player2);
                    Debug.Log(gameData.gameId);
                    Debug.Log(gameData.firstPlayer);
                    Debug.Log(gameData.piece1);
                    Debug.Log(gameData.piece2);
                    if (gameData.firstPlayer == myId)
                    {
                        areWeFirst = true;
                        opponentId = gameData.player2;
                        
                        ourTeam = gameData.piece1;
                    }
                    else
                    {
                        areWeFirst = false;
                        ourTeam = gameData.piece2;
                        opponentId = gameData.player1;
                    }

                    //Scene move to multiplayer game;
                    loadMuliplayerGame = true;


                    while(!sceneLoaded){

                        //Wasting time
                        
                    }
                    waiting = false;
                    if (areWeFirst)
                    {
                        Send(new Message(MessageType.TAKING_OUR_TURN, "We are just taking our turn", myId, clientId).Serialize(), clientSocket);
                    }
                    else
                    {
                        Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "We are waiting for our turn", myId, clientId).Serialize(), clientSocket);
                    }


                    break;
                case MessageType.TAKING_OUR_TURN:
                    System.Threading.Thread.Sleep(2000);
                    if (quit)
                    {
                        Send(new Message(MessageType.QUIT, "We are quitting", myId, clientId).Serialize(), clientSocket);
                    }
                    else
                    {
                        if (nextTurn)
                        {
                            Log("Sending turn over");
                            BoardState board = new BoardState(multiplayerGame.knightPos, multiplayerGame.barbarianPos, multiplayerGame.kingPos);
                            Send(new Message(MessageType.NEXT_TURN, board.Serialize(), myId, clientId).Serialize(), clientSocket);
                            nextTurn = false;
                        }
                        else
                        {
                            Send(new Message(MessageType.TAKING_OUR_TURN, "We are just taking our turn", myId, clientId).Serialize(), clientSocket);
                        }
                    }
                    break;
                case MessageType.WAITING_FOR_OUR_TURN:
                   System.Threading.Thread.Sleep(2000);
                    if (quit)
                    {
                        Send(new Message(MessageType.QUIT, "We are quitting", myId, clientId).Serialize(), clientSocket);
                    }
                    else
                    {
                        Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "We are waiting for our turn", myId, clientId).Serialize(), clientSocket);
                    }
                    break;
                case MessageType.YOUR_TURN:
                    Log("Our turn");
                    BoardState boardS = BoardState.Deserialise(msg.message);
                    multiplayerGame.knightPos = new List<Vector2>();
                    multiplayerGame.knightPos = boardS.knightsPos;
                    multiplayerGame.barbarianPos = new List<Vector2>();
                    multiplayerGame.barbarianPos = boardS.barbariansPos;
                    Debug.Log(multiplayerGame.knightPos.Count + " : " + multiplayerGame.barbarianPos.Count);
                    multiplayerGame.kingPos = boardS.kingPos;
                    multiplayerGame.ourTurn = true;
                    movePieces = true;

                    Send(new Message(MessageType.TAKING_OUR_TURN, "We are just taking our turn", myId, clientId).Serialize(), clientSocket);

                    break;
                case MessageType.DISCONNECT:
                    Log("We can disconnect");
                    exit = true;
                    break;

                case MessageType.OPPONENT_DISCONNECT:
                    loadMenu = true;
                    break;
                case MessageType.GAME_OVER:
                    Log("We lost");
                    BoardState boardState = BoardState.Deserialise(msg.message);
                    multiplayerGame.knightPos = new List<Vector2>();
                    multiplayerGame.knightPos = boardState.knightsPos;
                    multiplayerGame.barbarianPos = new List<Vector2>();
                    multiplayerGame.barbarianPos = boardState.barbariansPos;
                    Debug.Log(multiplayerGame.knightPos.Count + " : " + multiplayerGame.barbarianPos.Count);
                    multiplayerGame.kingPos = boardState.kingPos;
                    multiplayerGame.ourTurn = true;
                    movePieces = true;

                    multiplayerGame.WeLost();

                    break;
                default:
                    Send(new Message(MessageType.IGNORE, "Waiting", myId, clientId).Serialize(), clientSocket);

                    break;
            }




            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }
    
    void Update()
    {
        if(exit && SceneManager.GetActiveScene().name == "MultiplayerLobby")
        {
            SceneManager.LoadScene("Main Menu");
        }

        if (loadMuliplayerGame)
        {
            SceneManager.LoadScene("MultiplayerScene");
        }
        if(movePieces){
            multiplayerGame.MovePieces();
            movePieces= false;
        }
        if(loadMenu){
            SceneManager.LoadScene("Main Menu");
        }
        if (waiting)
        {
            waitingText.text = waitingString;
        }
        
    }

    private void FixedUpdate()
    {
        if (waiting)
        {
            waitingIcon.transform.Rotate(waitingIcon.transform.rotation.x, waitingIcon.transform.rotation.y, waitingIcon.transform.rotation.z - 5);
        }
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => sceneLoaded == true);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Attempting to quit");
        StartCoroutine(TimeToQuit());
        if (!allowQuitting)
        {
            Application.CancelQuit();
        }

    }

    IEnumerator TimeToQuit()
    {
        disconnect = true;
        Debug.Log("Waiting for exit");
        yield return new WaitUntil(() => exit == true);
        Debug.Log("We can quit");
        allowQuitting = true;
        Application.Quit();
    }

    private static void Send(string text, Socket socket)
    {
        byte[] data = Encoding.ASCII.GetBytes(text);
        //Debug.Log("Began sending " + data.Length + " bytes");
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
            //Debug.Log("Sent data");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

}
