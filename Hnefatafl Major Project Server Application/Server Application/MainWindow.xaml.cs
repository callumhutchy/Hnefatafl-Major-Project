using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
        //Some of the server connection code is editied code from the above link.
        //It will be noted where this code is.



        public MainWindow()
        {
            InitializeComponent();
        }
        //Queue to hold messages that need printing to the log view
        private static List<string> logQueue = new List<string>();

        //A section of memory to reuse when receiving messages, instead of reallocating every time a message is received
        private static byte[] buffer = new byte[2048];

        //The main server socket, will be used to accept connections in TCP
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //A list of all the clients that are connected to the server, this counts all game players and waiting players
        private static List<ClientInfo> clientSockets = new List<ClientInfo>();

        //A list of all the information of the games currently player
        private static List<GameData> gameList = new List<GameData>();
        //A list of the currently waiting players
        private static List<Guid> waitingPlayers = new List<Guid>();

        //If the server started
        static bool serverStarted = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!serverStarted)
            {
                SetupServer();
                Thread uiUpdater = new Thread(UIUpdater);
                uiUpdater.Start();
                Thread logManagementThread = new Thread(new ThreadStart(LogManagement));
                logManagementThread.Start();
                Log("Started Log Manager");
                serverStarted = true;
            }
            else
            {
                serverSocket.Close();
            }
            
        }
        
        private void UIUpdater()
        {
            while (true)
            {
                try
                {
                    this.Dispatcher.Invoke(() => {
                        lblClients.Content = "Clients Connected: " + clientSockets.Count();
                        lblWaiting.Content = "Waiting For Players: " + waitingPlayers.Count();
                    });
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
                
            }
        }

        //Starts receiving connections
        private static void SetupServer()
        {
            Log("Setting up server...");
            //This code was left in from the Microsoft Example
            //This binds the server socket to the local IP address and on the port defined 7995
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 7995));
            //The Server will now listen for incoming connections will a backlog of 5, if the server gets spammed with connections the server will be able to keep reference to at least 5
            //Realistically it will process the connections instantly so there is no real need for a backlog
            serverSocket.Listen(5);
            //When the server receives a connection is starts accepting data from it
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        //This function is from the Microsoft example, it has been stripped down and only the functionality needed was left
        //It is used to 
        private static void AcceptCallback(IAsyncResult ar)
        {
            //Create a new socket after receiving all the connection info
            Socket socket = serverSocket.EndAccept(ar);
            //Now listen to what the socket wants to send
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            //After handling the new connection we go back to accepting new connections.
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }


        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //We stop receiving the data and insert it into a byte array.
                Socket socket = (Socket)ar.AsyncState;
                int received = socket.EndReceive(ar);
                byte[] dataBuf = new byte[received];
                Array.Copy(buffer, dataBuf, received);
                //Convert the byte array into a string for easier processing
                string text = Encoding.ASCII.GetString(dataBuf);
                //If the string is empty, we need to stop processing this message and just return
                if (text == "")
                {
                    return;
                }

                //The message received should have been sent from a machine that understands the message class, so we can deserialise the message into an instance of that
                Message message = Message.Deserialize(text);
                //Now get the client if from the message
                Guid clientId = message.clientId;

                //If we already have a client with that client id connected they will have been assigned a user id
                //Here we check to see if our stored version of the user id is the same as the one they have provided
                if (clientSockets.Exists(x => x.clientId == clientId))
                {
                    if (!message.userId.Equals(clientSockets.Find(x => x.clientId == message.clientId).userId))
                    {
                        Log("Those IDs do not match!! " + message.userId + " : " + clientSockets.Find(x => x.clientId == clientId).userId);
                    }
                }

                //Have a quick check to see if there is a game with this user in, if there is we will process this variable later
                GameData gd = gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId);

                //Logic

                switch (message.type)
                {
                    //When a client is connecting to the game
                    case MessageType.CONNECT:
                        Log(clientId.ToString());

                        //We provided get their client id
                        Guid cid = new Guid(message.message);
                        //Create a user id for the client
                        Guid newUserId = Guid.NewGuid();

                        //Create a reference to this client
                        clientSockets.Add(new ClientInfo(socket, newUserId, cid));

                        //Tell the client they are now connected and what their user id is, this will need to be provided when they send a message for authentication.
                        Send(new Message(MessageType.CONNECT, "Welcome to the server", newUserId).Serialize(), socket);

                        Log(message.type + " : " + message.message);

                        break;
                    //When a player wants to disconnect from the game
                    case MessageType.DISCONNECT:
                        //If the client is connected to the server they can leave
                        if (clientSockets.Exists(x => x.clientId == clientId))
                        {
                            Send(new Message(MessageType.DISCONNECT, "You can leave", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                        }

                        //If the client was in a game they we need to change their game reference to say they have disconnected
                        if (gameList.Exists(x => x.player1 == message.userId || x.player2 == message.userId))
                        {

                            gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId).disconnect = true;
                        }

                        //If the client was waiting for a game, we can just remove them from the waiting players list
                        if (waitingPlayers.Exists(x => x == message.userId))
                        {
                            waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                        }

                        //We now just remove the client from out reference list
                        clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                        break;
                    //When a player wants to quit a game
                    case MessageType.QUIT:

                        //First find the game reference and set a disconnect flag
                        if (gameList.Exists(x => x.player1 == message.userId || x.player2 == message.userId))
                        {
                            gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId).disconnect = true;
                        }

                        //If the player is in the waiting queue remove them
                        if (waitingPlayers.Exists(x => x == message.userId))
                        {
                            waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                        }

                        //Remove their connection completely
                        clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                        break;
                    //When a player wants to start finding a game
                    case MessageType.FIND_GAME:
                        //Add the player to the waiting queue
                        waitingPlayers.Add(message.userId);
                        Log(message.type + " : " + message.message);
                        //Tell them theyve been added to the queue and will be matched up
                        Send(new Message(MessageType.WAITING_FOR_PLAYER, "Added you to the queue", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                        break;
                    //While a players is waiting for an opponent
                    case MessageType.WAITING_FOR_PLAYER:
                        //If the number of players in the queue is less than 2 we need to wait for more players
                        if (waitingPlayers.Count < 2 && !(gameList.Exists(x => x.player1 == message.userId || gameList.Exists(y => y.player2 == message.userId))) && clientSockets.Exists(x => x.clientId == clientId))
                        {
                            Send(new Message(MessageType.WAITING_FOR_PLAYER, "No Players waiting", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                        }
                        //If there are more than 2, we can start creating a game
                        else
                        {
                            //If there is already a game setup, because the opponent contacted the server first
                            if (gameList.Exists(x => x.player1 == message.userId || gameList.Exists(y => y.player2 == message.userId)))
                            {
                                //Set whether we are ready
                                if (gd.player1 == message.userId)
                                {
                                    gameList[gameList.FindIndex(x => x.player1 == message.userId || x.player2 == message.userId)].p1Ready = true;
                                }
                                else if (gd.player2 == message.userId)
                                {
                                    gameList[gameList.FindIndex(x => x.player1 == message.userId || x.player2 == message.userId)].p2Ready = true;

                                }
                                //Say we have found a player
                                Send(new Message(MessageType.PLAYER_FOUND, gd.gameId.ToString(), message.userId).Serialize(), socket);

                            }

                            //Quick check to make sure user is connected
                            if (clientSockets.Exists(x => x.userId == message.userId))
                            {

                                //If the user has already found a game
                                if (!clientSockets.Find(x => x.userId == message.userId).gameMatch)
                                {
                                    //Start setting up a game
                                    //Player 1 will be us
                                    Guid player1 = message.userId;
                                    //Now remove us from the queue
                                    waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                                    //Player 2 will be from the front of the waiting queue
                                    Guid player2 = waitingPlayers[0];
                                    //Now remove them
                                    waitingPlayers.RemoveAt(0);

                                    //Set both clients to have found a game so they don't get put in any other game
                                    clientSockets.Find(x => x.userId == player2).gameMatch = true;
                                    clientSockets.Find(x => x.userId == player1).gameMatch = true;

                                    //Create a new game reference, with the two players
                                    GameData gameD = GameData.CreateGameData(player1, player2);

                                    //Say that we are ready and wait for the opponened
                                    gameD.p1Ready = true;

                                    //Store the reference to the game
                                    gameList.Add(gameD);

                                    Send(new Message(MessageType.PLAYER_FOUND, gameD.gameId.ToString(), message.userId).Serialize(), socket);

                                }
                                //We have already been added to a game so set us up
                                else if (clientSockets.Find(x => x.userId == message.userId).gameMatch)
                                {
                                    GameData gameD = gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId);
                                    Guid opponent;
                                    bool p1 = false;
                                    if (gameD.player1 == message.userId)
                                    {
                                        opponent = gameD.player2;
                                        p1 = true;
                                    }
                                    else
                                    {
                                        opponent = gameD.player1;
                                    }

                                    if (p1)
                                    {
                                        gameList[gameList.FindIndex(x => x.player1 == message.userId)].p1Ready = true;
                                    }
                                    else
                                    {
                                        gameList[gameList.FindIndex(x => x.player2 == message.userId)].p2Ready = true;
                                    }

                                    Send(new Message(MessageType.PLAYER_FOUND, gameD.gameId.ToString(), message.userId).Serialize(), socket);
                                }
                            }


                        }

                        break;
                    //When two players have been found and a game has to be setup
                    case MessageType.GAME_SETUP:
                        //An id is generated for the game
                        Guid gid = new Guid(message.message);

                        //
                        GameData game = gameList.Find(x => x.gameId == gid);
                        Log(game.p1Ready + " " + game.p2Ready);
                        if (game.p1Ready && game.p2Ready)
                        {
                            game.SetPieces();
                            game.SetFirst();
                            Send(new Message(MessageType.GAME_SETUP, game.Serialize(), clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                        }
                        else
                        {
                            Send(new Message(MessageType.PLAYER_FOUND, game.gameId.ToString(), message.userId).Serialize(), socket);
                        }
                        break;
                    //While a player is still on their turn
                    case MessageType.TAKING_OUR_TURN:
                        //If the opponent has disconnected then this client has to be disconnected
                        if (gd.disconnect)
                        {
                            Send(new Message(MessageType.OPPONENT_DISCONNECT, "Your opponent has dced", message.userId).Serialize(), socket);
                        }
                        //Other wise the server and client just keep polling until the turn has been taken
                        else
                        {
                            Send(new Message(MessageType.TAKING_OUR_TURN, "Ok keep going", message.userId).Serialize(), socket);
                        }
                        break;
                    //When a player is waiting for the turn
                    case MessageType.WAITING_FOR_OUR_TURN:

                        //If there was a disconnect, tell the client that it needs to disconnect because the opponent left
                        //Then remove it from the connected clients list
                        if (gd.disconnect)
                        {
                            Send(new Message(MessageType.OPPONENT_DISCONNECT, "Your opponent has dced", message.userId).Serialize(), socket);
                            clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                        }
                        //Other wise if the game is still on, we need to check if it's out turn
                        else
                        {
                            //If the turn id has now been set to ours
                            if (gd.turn == message.userId)
                            {
                                //If their isnt a winner it is just out turn
                                if (gd.winner == Guid.Empty)
                                {
                                    Send(new Message(MessageType.YOUR_TURN, gd.boardState, message.userId).Serialize(), socket);
                                }
                                //If there is a winner then a message to say the game is over
                                else
                                {
                                    //A quick check to just make sure the winner was us
                                    if (gd.winner != message.userId)
                                    {
                                        Send(new Message(MessageType.GAME_OVER, gd.boardState, message.userId).Serialize(), socket);
                                    }
                                }

                            }
                            //If the turn player isn't the message client then they need to keep waiting.
                            else
                            {
                                Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "Ok we're still waiting for the other player", message.userId).Serialize(), socket);
                            }
                        }

                        break;
                    //When the turn is passed
                    case MessageType.NEXT_TURN:

                        //Set the new board state to the one received
                        gameList.Find(x => x.gameId == gd.gameId).boardState = message.message;

                        //We find which player the current message was from, then tell the to start waiting for their turn 
                        if (gd.player1 == message.userId)
                        {
                            gameList.Find(x => x.player1 == message.userId).turn = gd.player2;

                            Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "Start waiting for your turn", message.userId).Serialize(), socket);
                        }
                        else if (gd.player2 == message.userId)
                        {
                            gameList.Find(x => x.player2 == message.userId).turn = gd.player1;
                            Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "Start waiting for your turn", message.userId).Serialize(), socket);
                        }
                        else
                        {
                            Log("Didn't get the message correctly, please send again");
                            Send(new Message(MessageType.SEND_AGAIN, "Please Send again", message.userId).Serialize(), socket);
                        }


                        break;
                    //When a player has won the game
                    case MessageType.GAME_OVER:
                        Log("Game has ended " + gd.gameId.ToString());
                        Log("Winner is " + message.userId);

                        //We find the game and set the winner
                        gameList.Find(x => x.gameId == gd.gameId).winner = message.userId;

                        //We not change the turn so the other player will access the game data
                        if (gd.player1 == message.userId)
                        {
                            gameList.Find(x => x.player1 == message.userId).turn = gd.player2;
                        }
                        else
                        {
                            gameList.Find(x => x.player2 == message.userId).turn = gd.player1;
                        }
                        break;

                    //If the message doesn't match then we will reply to the client with an ignore message
                    default:
                        Send(new Message(MessageType.IGNORE, "Ignore", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                        break;
                }

                //After processing the last message, its time to start receiving a new message
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }


        }

        //This remains exactly the same as the Microsoft code, except for some name changes
        //It is used to start sending the data to the client.
        private static void Send(string text, Socket socket)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

        }

        //This is a modified version of some Microsoft code, some unnecessary code about collecting the bytes sent was removed.
        //Once all the bytes of a message have been sent, this function is called and the send is ended.
        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        //A simple function that can be called anywhere in the program and a message will be added to the end of the log queue
        private static void Log(string msg)
        {
            logQueue.Add(msg);
        }

        //The file location of the log file
        private static string logPath = "log.txt";

        //Handles the messages in the log queue and outputs them.
        private void LogManager()
        {
            //If there is a message in the log queue
            if (logQueue.Count > 0)
            {
                //Preprend the message at the front of the queue with the time
                string message = "[" + DateTime.Now + "]: " + logQueue[0] + "\n";
                //Remove the message from the front of the queue
                logQueue.RemoveAt(0);
                try
                {
                    //Output the message to the log box
                    this.Dispatcher.Invoke(() =>
                    {
                        txtLog.Text += message;
                        txtLog.ScrollToEnd();
                    });
                }
                catch (Exception)
                {

                }

                //Also write the message to the log text file
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine();
                }


            }
        }

        //The log processing thread
        private void LogManagement()
        {
            //When the thread is started at the beginning of runtime we need to check if there is an existing log file
            //If there is then we need to rename it, we prepend the name with the Date and time and then move it to the new path
            if (File.Exists(logPath))
            {
                string newPath = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + "log.txt";
                File.Move(logPath, newPath);
            }
            else
            {
                //If there isn't an existing log file, we create a new one at the desired path
                StreamWriter sw = File.CreateText(logPath);
                sw.Close();
            }

            //Once the log file has been sorted we now just constantly loop through the log manager and process the log queue
            while (true)
            {
                LogManager();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

    }
}
