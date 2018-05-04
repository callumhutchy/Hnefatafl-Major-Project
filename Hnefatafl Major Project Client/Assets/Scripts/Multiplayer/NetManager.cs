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
    //Some network code used here was edited from https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    //Where these sections are will be noted accordingly


    //The socket relating to the client
    private Socket clientSocket;
    //A buffer to store incoming messages
    private byte[] buffer;
    //The port in which the server is on
    private const int port = 7995;

    //A collection of ids
    public Guid myId;
    public Guid opponentId;
    public Guid gameId;
    public Guid clientId;

    //Which team we are on
    public Team ourTeam;

    //Whether we are first
    public bool areWeFirst;

    //Can we load the multiplayer scene?
    private bool loadMuliplayerGame = false;

    //Can we load the main menu?
    private bool loadMenu;

    //Is it the next turn?
    public bool nextTurn;

    //Reference to the child class of game for multiplayer games
    public MultiplayerGame multiplayerGame;

    //The swirl icon used while waiting
    public GameObject waitingIcon;
    //Waiting text to display what is happening
    public TMP_Text waitingText;
    //Whether the client is waiting
    public static bool waiting;

    //Can we quit?
    public bool quit;
    //Did we win?
    public bool weWon;
    //Was the scene loaded?
    static bool sceneLoaded;
    //The string output during waiting
    public static string waitingString;
    //Used to tell the game to move the pieces when received from the server
    static bool movePieces;
    //We are allowed to quit.
    bool allowQuitting;
    //Can we exit safely
    static bool exit;
    //Can we disconnect safely
    static bool disconnect;

    //Set the variables to their default values
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
        //Reset varaiables
        SetVariables();

        //Don't destroy this object
        GameObject.DontDestroyOnLoad(this);
        //Create a new id for us
        clientId = Guid.NewGuid();
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connect to server
            try
            {
                //The ip address we want to connect to.
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                //IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry("ec2-35-178-103-152.eu-west-2.compute.amazonaws.com").AddressList[0], port);
                Debug.Log("Attempting to connect");
                //Begin connection
                clientSocket.BeginConnect(endPoint, ConnectCallback, null);

            }
            catch (Exception ex)
            {
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
        //Start an event listener for when scenes are loaded.
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Start waiting
        waiting = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name == "MultiplayerScene")
        {
            loadMuliplayerGame = false;
            sceneLoaded = true;
            multiplayerGame = GameObject.FindGameObjectWithTag("GameController").GetComponent<MultiplayerGame>();
            if (areWeFirst)
            {
                multiplayerGame.ourTurn = true;
            }
            else
            {
                multiplayerGame.ourTurn = false;
            }

        }
        else if (scene.name == "Main Menu")
        {
            GameObject.Destroy(GameObject.FindGameObjectWithTag("net_man"));

        }
    }

    //Thread used to receive incoming messages
    Thread receiveData;

    //Out put debug logs and write to the waiting message
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
            //Assign the buffer a new byte array the size of the incoming data
            buffer = new byte[clientSocket.ReceiveBufferSize];
            Log("Connecting");
            Log("Sending client id : " + clientId.ToString());

            //Send connection message to the server
            Send(new Message(MessageType.CONNECT, clientId.ToString()).Serialize(), clientSocket);
            //Begin receiving the response from the server
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    //Used for the exit button while queuing
    public void LeaveQueueButton()
    {
        disconnect = true;
    }

    //This method will loop 
    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            //Finish receiving the message from the server
            int received = clientSocket.EndReceive(ar);

            //If the message size is only 0 there has been an error and we need to exit.
            if (received == 0)
            {
                return;
            }
            //Convert the byte array received into a string
            string reply = Encoding.ASCII.GetString(buffer);
       
            //Turn the string into a Message object
            Message msg = Message.Deserialize(reply);
            Debug.Log(msg.type + " : " + msg.message);

            if (!myId.Equals(msg.userId))
            {
                Log("Our id from the server doesn't match");
            }

            //If we want to disconnect, tell the server before anything else happends
            if (disconnect)
            {
                Log("Sending a leave message");
                Send(new Message(MessageType.DISCONNECT, "Can we leave", myId, clientId).Serialize(), clientSocket);
                disconnect = false;
            }

            //If we have won, tell the server before the incoming message is handled
            if (weWon)
            {
                Log("We won sending to server");
                BoardState board = new BoardState(multiplayerGame.knightPos, multiplayerGame.barbarianPos, multiplayerGame.kingPos);

                Send(new Message(MessageType.GAME_OVER, board.Serialize(), myId, clientId).Serialize(), clientSocket);
                return;
            }

            //Select how to handle the message using the Message Type
            switch (msg.type)
            {
                //We are now connected, we need to tell the server we want to start finding a game
                case MessageType.CONNECT:

                    myId = msg.userId;
                    Log("We've connected and here is our id : " + myId);
                    Send(new Message(MessageType.FIND_GAME, "Please find a game", myId, clientId).Serialize(), clientSocket);

                    break;
                //We are waiting for an opponent
                case MessageType.WAITING_FOR_PLAYER:
                    Log("We are waiting for another player");
                    //We need to sleep for 2 seconds so we dont spam the server
                    System.Threading.Thread.Sleep(2000);
                    Send(new Message(MessageType.WAITING_FOR_PLAYER, "Ok we will wait", myId, clientId).Serialize(), clientSocket);
                    break;
                //An opponent has been found for us and we have to confirm that we are ready
                case MessageType.PLAYER_FOUND:
                    Log("Opponent found");
                    Send(new Message(MessageType.GAME_SETUP, msg.message, myId, clientId).Serialize(), clientSocket);

                    break;
                //We have been sent the game information
                case MessageType.GAME_SETUP:

                    //Get the game data and assign the variables to our local variables
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

                    //Scene move to multiplayer game
                    loadMuliplayerGame = true;

                    //If the scene hasnt finished loading yet
                    while (!sceneLoaded)
                    {

                        //Wasting time

                    }
                    //Stop waiting
                    waiting = false;
                    //If we are first we will start taking out turn, otherwise we will wait
                    if (areWeFirst)
                    {
                        Send(new Message(MessageType.TAKING_OUR_TURN, "We are just taking our turn", myId, clientId).Serialize(), clientSocket);
                    }
                    else
                    {
                        Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "We are waiting for our turn", myId, clientId).Serialize(), clientSocket);
                    }


                    break;
                //We are taking our turn
                case MessageType.TAKING_OUR_TURN:
                    //Sleep for 2 seconds so no server spam
                    System.Threading.Thread.Sleep(2000);
                    //If we have selected to quit, tell the server we are quitting
                    if (quit)
                    {
                        Send(new Message(MessageType.QUIT, "We are quitting", myId, clientId).Serialize(), clientSocket);
                    }
                    //Otherwise take our turn
                    else
                    {
                        //If we have made our move, send the game board over
                        if (nextTurn)
                        {
                            Log("Sending turn over");
                            BoardState board = new BoardState(multiplayerGame.knightPos, multiplayerGame.barbarianPos, multiplayerGame.kingPos);
                            Send(new Message(MessageType.NEXT_TURN, board.Serialize(), myId, clientId).Serialize(), clientSocket);
                            nextTurn = false;
                        }
                        //If not we keep telling the server we are still taking our turn
                        else
                        {
                            Send(new Message(MessageType.TAKING_OUR_TURN, "We are just taking our turn", myId, clientId).Serialize(), clientSocket);
                        }
                    }
                    break;
                //The server didn't change the game data correctly and needs us to send the information again
                case MessageType.SEND_AGAIN:
                    Log("Sending turn over");
                    BoardState board1 = new BoardState(multiplayerGame.knightPos, multiplayerGame.barbarianPos, multiplayerGame.kingPos);
                    Send(new Message(MessageType.NEXT_TURN, board1.Serialize(), myId, clientId).Serialize(), clientSocket);
                    nextTurn = false;
                    break;
                //We are waiting for our turn
                case MessageType.WAITING_FOR_OUR_TURN:
                    //Sleep for 2 seconds before asking the server again otherwise we will spam them
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
                //It is now out turn
                case MessageType.YOUR_TURN:
                    Log("Our turn");
                    //Get the new board state and implement it
                    BoardState boardS = BoardState.Deserialise(msg.message);
                    multiplayerGame.knightPos = new List<Vector2>();
                    multiplayerGame.knightPos = boardS.knightsPos;
                    multiplayerGame.barbarianPos = new List<Vector2>();
                    multiplayerGame.barbarianPos = boardS.barbariansPos;
                    Debug.Log(multiplayerGame.knightPos.Count + " : " + multiplayerGame.barbarianPos.Count);
                    multiplayerGame.kingPos = boardS.kingPos;
                    multiplayerGame.ourTurn = true;
                    movePieces = true;
                    //Tell the server we are not taking out turn
                    Send(new Message(MessageType.TAKING_OUR_TURN, "We are just taking our turn", myId, clientId).Serialize(), clientSocket);

                    break;
                //Confirmation from the server that we can disconnect, and will allow the waiting function to process
                case MessageType.DISCONNECT:
                    Log("We can disconnect");
                    exit = true;
                    break;
                //The opponent disconnected so we can now load the main menu
                case MessageType.OPPONENT_DISCONNECT:
                    loadMenu = true;
                    break;
                //Weve been told the game is over, which means we lost
                case MessageType.GAME_OVER:
                    Log("We lost");
                    //Display the new board state
                    BoardState boardState = BoardState.Deserialise(msg.message);
                    multiplayerGame.knightPos = new List<Vector2>();
                    multiplayerGame.knightPos = boardState.knightsPos;
                    multiplayerGame.barbarianPos = new List<Vector2>();
                    multiplayerGame.barbarianPos = boardState.barbariansPos;
                    Debug.Log(multiplayerGame.knightPos.Count + " : " + multiplayerGame.barbarianPos.Count);
                    multiplayerGame.kingPos = boardState.kingPos;
                    multiplayerGame.ourTurn = true;
                    movePieces = true;
                    //Then display the relevant losing message
                    multiplayerGame.WeLost();

                    break;
                //If the message doesn't have a valid type we will just tell the server we ignored it
                default:
                    Send(new Message(MessageType.IGNORE, "Waiting", myId, clientId).Serialize(), clientSocket);
                    break;
            }
            //Begin receiving new messages from the server now we have processed the last one
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }

    void Update()
    {
        //When we are allowed to exit from waiting, return to the main menu
        if (exit && SceneManager.GetActiveScene().name == "MultiplayerLobby")
        {
            SceneManager.LoadScene("Main Menu");
        }
        //Load the multiplayer game scene
        if (loadMuliplayerGame)
        {
            SceneManager.LoadScene("MultiplayerScene");
        }
        //Move the board pieces
        if (movePieces)
        {
            multiplayerGame.MovePieces();
            movePieces = false;
        }
        //Go back to the main menu
        if (loadMenu)
        {
            SceneManager.LoadScene("Main Menu");
        }
        //Waiting message is set to the string
        if (waiting)
        {
            waitingText.text = waitingString;
        }

    }

    private void FixedUpdate()
    {
        //When waiting rotate the waiting icon around
        if (waiting)
        {
            waitingIcon.transform.Rotate(waitingIcon.transform.rotation.x, waitingIcon.transform.rotation.y, waitingIcon.transform.rotation.z - 5);
        }
    }

    //Thread used to wait for a scene to load
    IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => sceneLoaded == true);
    }

    void OnApplicationQuit()
    {
        //When quiting we want to disconnect from the server
        Debug.Log("Attempting to quit");
        StartCoroutine(TimeToQuit());
        //If we arent allowed to quit then cancel the quit
        if (!allowQuitting)
        {
            Application.CancelQuit();
        }

    }

    IEnumerator TimeToQuit()
    {
        //Say we want to disconnect
        disconnect = true;
        Debug.Log("Waiting for exit");
        //Now wait for the server to respond and allow us to exit
        yield return new WaitUntil(() => exit == true);
        Debug.Log("We can quit");
        //Allow the next quit and retry
        allowQuitting = true;
        Application.Quit();
    }


    //This section of code has not been edited from the Microsoft example
    //It converts a string message to a byte array and then begins sending it
    private static void Send(string text, Socket socket)
    {
        byte[] data = Encoding.ASCII.GetBytes(text);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
    }

    //This section of code has been editied and simplified from the Microsoft example
    //It occurs when the Send function has finished sending the message
    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

}
