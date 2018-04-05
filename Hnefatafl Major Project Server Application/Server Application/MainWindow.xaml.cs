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
        public MainWindow()
        {
            InitializeComponent();
        }
        private static List<string> logQueue = new List<string>();

        private static byte[] buffer = new byte[2048];
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<ClientInfo> clientSockets = new List<ClientInfo>();

        private static List<GameData> gameList = new List<GameData>();
        private static List<Guid> waitingPlayers = new List<Guid>();
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetupServer();
            Thread uiUpdater = new Thread(UIUpdater);
            uiUpdater.Start();
            Thread logManagementThread = new Thread(new ThreadStart(LogManagement));
            logManagementThread.Start();
            Log("Started Log Manager");
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

        private void SetupServer()
        {
            Log("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 7995));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            
            
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }





        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);
            Log(received.ToString());
            byte[] dataBuf = new byte[received];
            Array.Copy(buffer, dataBuf, received);
            string text = Encoding.ASCII.GetString(dataBuf);
            
            //text = StringCipher.Decrypt(text);
            if(text == "")
            {
                return;
            }

            Message message = Message.Deserialize(text);
            Guid clientId = message.clientId;

            if (clientSockets.Exists(x => x.clientId == clientId))
            {
                if (!message.userId.Equals(clientSockets.Find(x => x.clientId == message.clientId).userId))
                {
                    Log("Those IDs do not match!! " + message.userId + " : " + clientSockets.Find(x => x.clientId == clientId).userId);
                }
            }

            GameData gd = gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId);

            //Logic

            switch (message.type)
            {
                case MessageType.CONNECT:
                    Log(clientId.ToString());
                    Guid cid = new Guid(message.message);
                    clientSockets.Add(new ClientInfo(socket, Guid.NewGuid(), cid ));

                    Send(new Message(MessageType.CONNECT, "Welcome to the server", clientSockets.Find(x => x.clientId == cid).userId).Serialize(), socket);

                    Log(message.type + " : " + message.message);

                    break;
                case MessageType.DISCONNECT:
                    Send(new Message(MessageType.DISCONNECT, "You can leave", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);

                    if(gameList.Exists(x => x.player1 == message.userId || x.player2 == message.userId))
                    {
                        gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId).disconnect = true;
                    }

                    if(waitingPlayers.Exists(x => x == message.userId))
                    {
                        waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                    }

                    clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                    break;
                case MessageType.QUIT:
                    if (gameList.Exists(x => x.player1 == message.userId || x.player2 == message.userId))
                    {
                        gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId).disconnect = true;
                    }

                    if (waitingPlayers.Exists(x => x == message.userId))
                    {
                        waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                    }

                    clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                    break;
                case MessageType.FIND_GAME:
                    waitingPlayers.Add(message.userId);
                    Log(message.type + " : " + message.message);
                    Send(new Message(MessageType.WAITING_FOR_PLAYER, "Added you to the queue", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                    break;
                case MessageType.WAITING_FOR_PLAYER:

                    if (waitingPlayers.Count < 2 && !(gameList.Exists(x => x.player1 == message.userId || gameList.Exists(y => y.player2 == message.userId))) && clientSockets.Exists(x => x.clientId == clientId))
                    {
                        Send(new Message(MessageType.WAITING_FOR_PLAYER, "No Players waiting", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                    }
                    else
                    {
                        if (gameList.Exists(x => x.player1 == message.userId || gameList.Exists(y => y.player2 == message.userId)))
                        {
                            if (gd.player1 == message.userId)
                            {
                                gameList[gameList.FindIndex(x => x.player1 == message.userId || x.player2 == message.userId)].p1Ready = true;
                            }
                            else if (gd.player2 == message.userId)
                            {
                                gameList[gameList.FindIndex(x => x.player1 == message.userId || x.player2 == message.userId)].p2Ready = true;

                            }

                            Send(new Message(MessageType.PLAYER_FOUND, gd.gameId.ToString(), message.userId).Serialize(), socket);

                        }
                        if (clientSockets.Exists(x => x.userId == message.userId))
                        {

                        
                        if (!clientSockets.Find(x => x.userId == message.userId).gameMatch)
                        {
                            Guid player1 = message.userId;
                            waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                            Guid player2 = waitingPlayers[0];
                            waitingPlayers.RemoveAt(0);

                            clientSockets.Find(x => x.userId == player2).gameMatch = true;
                            clientSockets.Find(x => x.userId == player1).gameMatch = true;

                            GameData gameD = GameData.CreateGameData(player1, player2);

                            gameD.p1Ready = true;

                            gameList.Add(gameD);

                            Send(new Message(MessageType.PLAYER_FOUND, gameD.gameId.ToString(), message.userId).Serialize(), socket);

                        }
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
                case MessageType.GAME_SETUP:

                    Guid gid = new Guid(message.message);
                    
                    GameData game = gameList.Find(x => x.gameId == gid);
                    Console.WriteLine(game.p1Ready + " " + game.p2Ready);
                    if(game.p1Ready && game.p2Ready)
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

                case MessageType.TAKING_OUR_TURN:
                    if (gd.disconnect)
                    {
                        Send(new Message(MessageType.OPPONENT_DISCONNECT, "Your opponent has dced", message.userId).Serialize(), socket);
                    }
                    else
                    {
                        Send(new Message(MessageType.TAKING_OUR_TURN, "Ok keep going", message.userId).Serialize(), socket);
                    }
                    
                    break;
                case MessageType.WAITING_FOR_OUR_TURN:

                    if (gd.disconnect)
                    {
                        Send(new Message(MessageType.OPPONENT_DISCONNECT, "Your opponent has dced", message.userId).Serialize(), socket);
                        clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                    }
                    else
                    {
                        if (gd.turn == message.userId)
                        {
                            Send(new Message(MessageType.YOUR_TURN, gd.boardState, message.userId).Serialize(), socket);
                        }
                        else
                        {
                            Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "Ok we're still waiting for the other player", message.userId).Serialize(), socket);

                        }
                    }
                    
                    break;
                case MessageType.NEXT_TURN:
                    
                    if(gd.player1 == message.userId)
                    {
                        gameList.Find(x => x.player1 == message.userId).turn = gd.player2;
                    }
                    else
                    {
                        gameList.Find(x => x.player2 == message.userId).turn = gd.player1;
                    }

                    gameList.Find(x => x.gameId == gd.gameId).boardState = message.message;

                    Send(new Message(MessageType.WAITING_FOR_OUR_TURN, "Start waiting for your turn", message.userId).Serialize(), socket);

                    break;
                    
                case MessageType.GAME_OVER:
                    //Still need stuff here
                    break;
                default:
                    Send(new Message(MessageType.IGNORE, "Ignore", clientSockets.Find(x => x.clientId == clientId).userId).Serialize(), socket);
                    break;
            }
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);


        }

        private static void Send(string text, Socket socket)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        private static void Log(string msg)
        {
            logQueue.Add(msg);
        }

        private static string logPath = "log.txt";

        private void LogManager()
        {
            if(logQueue.Count > 0)
            {
                string message = "[" + DateTime.Now + "]: " + logQueue[0] + "\n";
                logQueue.RemoveAt(0);
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        txtLog.Text += message;
                        txtLog.ScrollToEnd();
                    });
                }
                catch (Exception)
                {

                }
                

                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine();
                }


            }
        }

        private void LogManagement()
        {
            if (File.Exists(logPath))
            {
                string newPath = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + "log.txt";
                File.Move(logPath, newPath);
            }
            else
            {
                StreamWriter sw = File.CreateText(logPath);
                sw.Close();
            }

            while (true)
            {
                LogManager();
            }
        }

    }
}
