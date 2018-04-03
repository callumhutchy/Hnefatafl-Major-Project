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
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<string> logQueue = new List<string>();

        private static List<Tuple<Message, Socket>> SendQueue = new List<Tuple<Message, Socket>>();

        private static List<Tuple<Message, Socket>> RecieveQueue = new List<Tuple<Message, Socket>>();




        private static List<SocketIdentifier> clients = new List<SocketIdentifier>();

        string logPath = "log.txt";

        bool serverStarted = false;
        
        static int port = 7995;
        byte[] byteData = new byte[1024];

        public MainWindow()
        {
            InitializeComponent();
            Thread logManagementThread = new Thread(new ThreadStart(LogManagement));
            logManagementThread.Start();
            Log("Started Log Manager");

        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {

            if (!serverStarted)
            {
                Thread serverListenThread = new Thread(new ThreadStart(ServerListen));
              
                serverStarted = true;

                Log("Starting Server");
                serverListenThread.Start();
               
            }
        }

        public static ManualResetEvent allDone = new ManualResetEvent(false);


        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Log("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static Message ServerLogic(Message msg, Socket client)
        {
            MessageType type = msg.type;
            String message = msg.message;
            Guid id = msg.gameID;


            Message response = new Message(MessageType.IGNORE, "Keep alive");

            switch (type)
            {

                case MessageType.CONNECT:
                    Guid newID = Guid.NewGuid();
                    clients.Add(new SocketIdentifier(client, newID));
                    response = new Message(MessageType.CONNECT, "Welcome to the server", newID);
                    Log("Client connected, number of clients online: " + clients.Count());
                    break;
                case MessageType.DISCONNECT:
                    Log(message + " " + id.ToString());
                    clients.RemoveAll(x => x.userID == id);
                    Send(client, new Message(MessageType.DISCONNECT, "You can go").Serialize());

                    break;
                case MessageType.WAITING_FOR_PLAYER:
                    break;
                case MessageType.PLAYER_FOUND:
                    break;
                case MessageType.FIND_GAME:

                    break;

                default:
                    response = new Message(MessageType.DISCONNECT, "Error");
                    break;
            }

            return response;


        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            


            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    //Log("Read "+ content.Length+" bytes from socket. \n Data : " +   content);

                    Message msg = Message.Deserialize(content);
                    Log(msg.type.ToString() + ": " + msg.message);

                    
                    // Echo the data back to the client.  
                    Message response = ServerLogic(msg, handler);


                    Send(handler, response.Serialize());
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Log("Sent " + bytesSent + " bytes to client.");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Log(string msg)
        {
            logQueue.Add(msg);
        }

        private void LogManager()
        {
            if (logQueue.Count > 0)
            {
                string message = "[" + DateTime.Now + "]: " + logQueue[0] + "\n";
                logQueue.RemoveAt(0);
                this.Dispatcher.Invoke(() =>
                {
                    txtLog.Text += message;
                    txtLog.ScrollToEnd();

                });


                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(message);
                }
            }



        }

        //Thread Methods
        private void LogManagement()
        {

            if (File.Exists(logPath))
            {
                string newPath = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + " log.txt";
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

        private void ServerListen()
        {
            //StartListening();
            while (true)
            {
                lblClients.Content = "Clients: " + clients.Count();
            }
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
