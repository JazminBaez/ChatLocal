using Serialization;
using System.Net.Sockets;
using System.Net;

usingusing System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Serialization;

namespace Server
{
    public class Server
    {
        private IPHostEntry host;
        private IPAddress ipAdd;
        private IPEndPoint endPoint;

        private Socket s_socket;
        private List<Socket> clientSockets = new List<Socket>();
        private List<User> usersList = new List<User>();

        public Server(string ip, int port)
        {
            host = Dns.GetHostEntry(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            s_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s_socket.Bind(endPoint);
            s_socket.Listen(10);
        }

        public void Start()
        {
            Console.WriteLine("Servidor iniciado. Esperando clientes...");
            while (true)
            {
                Socket clientSocket = s_socket.Accept();
                clientSockets.Add(clientSocket);
                _ = Task.Run(() => HandleClientConnection(clientSocket));
            }
        }

        private async Task HandleClientConnection(Socket client)
        {
            User user;
            Message welcomeMessage = new Message("Bienvenido al servidor", "Server");
            client.Send(JSONSerialization.Serialize(welcomeMessage));

            try
            {
                user = await GetUserFromClient(client);
                Console.WriteLine("Cliente conectado: " + user.alias);
                usersList.Add(user);

                await HandleClientMessages(client, user);
            }
            catch (SocketException)
            {
                Console.WriteLine("Error en la conexión con el cliente.");
            }
            finally
            {
                Console.WriteLine("Cliente desconectado");
                client.Close();
            }
        }

        private async Task<User> GetUserFromClient(Socket client)
        {
            byte[] receivedData = await ReadBuffer(client);
            return (User)JSONSerialization.Deserialize(receivedData, typeof(User));
        }

        private async Task HandleClientMessages(Socket client, User user)
        {
            while (true)
            {
                byte[] receivedData = await ReadBuffer(client);
                if (receivedData == null) break; // Si el cliente se desconecta

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
                string availableCommands = "Comandos disponibles: \n/quit ---- cierra la conexión con el servidor, " +
                                           "\n/list ---- lista los usuarios conectados, " +
                                           "\n/commands ---- lista los comandos disponibles";
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

        private async Task<byte[]> ReadBuffer(Socket client)
        {
            byte[] buffer = new byte[1024];
            int bytesReceived = await client.ReceiveAsync(buffer, SocketFlags.None);

            if (bytesReceived == 0)
            {
                Console.WriteLine($"Cliente {client.RemoteEndPoint} desconectado.");
                return null;
            }

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);
            return receivedData;
        }
    }
}

