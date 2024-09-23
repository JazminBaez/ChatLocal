using System.Net;
using System.Text;
using System.Net.Sockets;
using Serialization;


namespace Server
{
    public class Server
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_socket;
        Socket? s_client;

        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();
        private List<User> usersList = new List<User>();

        public Server(string ip, int port)
        {
            host = Dns.GetHostByName(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            s_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s_socket.Bind(endPoint);
            s_socket.Listen(10);

        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("Esperando conexión de cliente...");
                s_client = s_socket.Accept();
                clientSockets.Add(s_client);
                _ = Task.Run(() => clientConnection(s_client));
            }

        }



        public async Task clientConnection(Socket client)
        {
            try
            {
                SendWelcomeMessage(client);

                var user = await GetUserFromClient(client);
                usersList.Add(user);
                Console.WriteLine($"Cliente conectado: {user.alias}");

                await HandleClientMessages(client, user);
            }
            catch (SocketException s_e)
            {
                Console.WriteLine("Error en la conexión con el cliente: " + s_e.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void SendWelcomeMessage(Socket client)
        {
            Message welcomeMessage = new Message("CHAT DE REDES 2024", "Server");
            client.Send(JSONSerialization.Serialize(welcomeMessage));
        }

        private async Task<User> GetUserFromClient(Socket client)
        {
            byte[] receivedData = await readBuffer(client);
            User user = (User)JSONSerialization.Deserialize(receivedData, typeof(User));
            return user;
        }

        private async Task HandleClientMessages(Socket client, User user)
        {
            while (true)
            {
                byte[] receivedData = await readBuffer(client);
                Message msg = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));

                if (msg.msg.StartsWith("/"))
                {
                    if (await HandleClientCommands(client, msg, user))
                    {
                        break;
                    }
                    continue;
                }

                Console.WriteLine($"{msg.userFrom}: {msg.msg}");
                BroadcastMessageToClients(client, msg);
            }
        }

        private async Task<bool> HandleClientCommands(Socket client, Message msg, User user)
        {
            if (msg.msg.Equals("/quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"{user.alias} se ha desconectado");
                usersList.Remove(user);
                return true;  
            }

            if (msg.msg.Equals("/list", StringComparison.OrdinalIgnoreCase))
            {
                string userList = "Usuarios conectados: " + string.Join(", ", usersList.Select(u => u.alias));
                Message listMessage = new Message(userList, "Server");
                client.Send(JSONSerialization.Serialize(listMessage));
                return false;
            }

            if (msg.msg.Equals("/commands", StringComparison.OrdinalIgnoreCase))
            {
                string availableCommands = "Comandos disponibles: \n/quit ---- cierra la conexion con el servidor, " +
                                                                  "\n /list ---- lista los usuarios conectados, " +
                                                                  "\n/commands --- lista los comandos disponibles";
                Message commandsMessage = new Message(availableCommands, "Server");
                client.Send(JSONSerialization.Serialize(commandsMessage));
                return false;
            }

            return false;
        }

        private void BroadcastMessageToClients(Socket client, Message msg)
        {
            foreach (Socket sClient in clientSockets)
            {
                if (sClient != client)
                {
                    Message broadcastMessage = new Message($"{msg.userFrom}: {msg.msg}", "Server");
                    sClient.Send(JSONSerialization.Serialize(broadcastMessage));
                }
            }
        }


        public async Task<byte[]> readBuffer(Socket client)
        {
            int bytesReceived;
            byte[] buffer = new byte[1024]; 

            bytesReceived = await client.ReceiveAsync(buffer, SocketFlags.None);

            if (bytesReceived == 0)
            {
                Console.WriteLine($"Cliente {client.RemoteEndPoint} desconectado.");
                return null; 
            }

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);

            return receivedData;
        }


        public Message quitCommand(byte[] receivedData)
        {
            return null;
        }   

        public String byteToString(byte[] bytes)
        {
            string msg;
            int endIndex;

            msg = Encoding.ASCII.GetString(bytes);
            endIndex = msg.IndexOf("\0");

            if (endIndex > 0)
            {
                msg = msg.Substring(0, endIndex);
            }

            return msg;
        }

        public byte[] stringToBytes(string msg)
        {
            return Encoding.ASCII.GetBytes(msg);
        }
    }
}
