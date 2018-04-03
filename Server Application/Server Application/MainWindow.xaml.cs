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


        private static byte[] buffer = new byte[1024];
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<ClientInfo> clientSockets = new List<ClientInfo>();

        private static List<GameData> gameList = new List<GameData>();
        private static List<Guid> waitingPlayers = new List<Guid>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetupServer();
        }

        private void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 7995));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            clientSockets.Add(new ClientInfo(socket, Guid.NewGuid()));
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);
            byte[] dataBuf = new byte[received];
            Array.Copy(buffer, dataBuf, received);
            string text = Encoding.ASCII.GetString(dataBuf);
            Message message = Message.Deserialize(text);

            if(!message.userId.Equals(clientSockets.Find(x => x.connection == socket).userId))
            {
                Console.WriteLine("Those IDs do not match!!");
            }


            Console.WriteLine(message.type + " : " + message.message);

            //Logic

            switch (message.type)
            {
                case MessageType.CONNECT:
                    
                    Send(new Message(MessageType.CONNECT, "Welcome to the server", clientSockets.Find(x => x.connection == socket).userId).Serialize(), socket);
                    break;
                case MessageType.DISCONNECT:
                    Send(new Message(MessageType.DISCONNECT, "You can leave", clientSockets.Find(x => x.connection == socket).userId).Serialize(), socket);
                    clientSockets.RemoveAll(x => x.userId.Equals(message.userId));
                    break;
                case MessageType.FIND_GAME:
                    waitingPlayers.Add(message.userId);
                    Send(new Message(MessageType.WAITING_FOR_PLAYER, "Added you to the queue", clientSockets.Find(x => x.connection == socket).userId).Serialize(), socket);
                    break;
                case MessageType.WAITING_FOR_PLAYER:
                    if(waitingPlayers.Count < 2)
                    {
                        Send(new Message(MessageType.WAITING_FOR_PLAYER, "No Players waiting", clientSockets.Find(x => x.connection == socket).userId).Serialize(), socket);
                    }
                    else
                    {
                        if (!clientSockets.Find(x => x.userId == message.userId).gameMatch)
                        {
                            Guid player1 = message.userId;
                            waitingPlayers.RemoveAt(waitingPlayers.FindIndex(x => x == message.userId));
                            Guid player2 = waitingPlayers[0];
                            waitingPlayers.RemoveAt(0);

                            clientSockets.Find(x => x.userId == player2).gameMatch = true;
                            clientSockets.Find(x => x.userId == player1).gameMatch = true;

                            gameList.Add(new GameData(player1, player2));

                            Send(new Message(MessageType.PLAYER_FOUND, "We have found a game against : " + player2.ToString(), player1).Serialize(), socket);

                        }
                        else
                        {
                            GameData game = gameList.Find(x => x.player1 == message.userId || x.player2 == message.userId);
                            Guid opponent;
                            if(game.player1 == message.userId) {
                                opponent = game.player2;
                            }
                            else
                            {
                                opponent = game.player1;
                            }

                            Send(new Message(MessageType.PLAYER_FOUND, "We have found a game against : " + opponent.ToString(), message.userId).Serialize(), socket);
                        }
                    }

                    break;
                case MessageType.GAME_SETUP:
                    Send(new Message(MessageType.IGNORE, "Ignore", clientSockets.Find(x => x.connection == socket).userId).Serialize(), socket);

                    break;
                case MessageType.FIRST_TURN:

                    break;
                case MessageType.YOUR_TURN:

                    break;
                case MessageType.GAME_OVER:

                    break;
                default:
                    Send(new Message(MessageType.IGNORE, "Ignore", clientSockets.Find(x => x.connection == socket).userId).Serialize(), socket);
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

    }
}
