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

        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();

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
<<<<<<< HEAD
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
=======

            Thread clientThread;
            IPEndPoint clientEndpoint;
            User clientData;

            Console.WriteLine("Servidor iniciado. Esperando clientes...");
            while (true)
            {
                c_socket = s_socket.Accept();
                clientSockets.Add(c_socket);
                //clientThread = new Thread(() => clientConnection(c_socket));
                //clientThread.Start();
                _ = Task.Run(() => clientConnection(c_socket));
            }



        }

        //public void clientConnection(Socket client)
        //{
        //    User user;
        //    string alias;
        //    int bytesReceived;
        //     byte[] receivedData;
        //    Message msg;

        //    c_socket.Send(stringToBytes("Welcome to the server"));

        //    try {
        //        byte[] buffer = new byte[1024];
        //        bytesReceived = c_socket.Receive(buffer);

        //        receivedData = getReceivedData(buffer, bytesReceived);

        //        user = (User)JSONSerialization.Deserialize(receivedData, typeof(User));

        //        alias = user.alias;


        //        Console.WriteLine("Cliente conectado, " + alias);
        //        Console.Out.Flush();

        //        while (true)
        //        {
        //            //IMRIME LO QUE HAY EN EL BUFFER ANTESD DE ARRANCAAR EL BUCLE

        //            Console.WriteLine($"Cliente {c_socket.RemoteEndPoint} siendo escuchado");

        //            byte[] bufferMsg = new byte[1024];
        //           bytesReceived = c_socket.Receive(bufferMsg);
        //            Console.WriteLine("DEPURACION bytesReceived: " + byteToString(bufferMsg));

        //            if (bytesReceived == 0)
        //            {
        //                Console.WriteLine($"Cliente {c_socket.RemoteEndPoint} desconectado.");
        //                break; 
        //            }

        //            receivedData = getReceivedData(bufferMsg, bytesReceived);
        //            msg  = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
        //            Console.WriteLine(msg.userFrom + ": " + msg.msg);
        //            Console.Out.Flush();
        //        }
        //    }
        //    catch(SocketException s_e)
        //    {
        //        Console.WriteLine("Client disconnected");
        //    }
        //    finally
        //    {
        //        c_socket.Close(); 
        //    }

        //}

        public async Task clientConnection(Socket client)
        {
            User user;
            string alias;
            byte[] receivedData;
            Message msg;

            Message welcomeMessage = new Message("Welcome to the server", "Server");
            client.Send(JSONSerialization.Serialize(welcomeMessage));


            try
            {

                receivedData = await this.readBuffer(client);

                user = (User)JSONSerialization.Deserialize(receivedData, typeof(User));
                alias = user.alias;

                Console.WriteLine("Cliente conectado, " + alias);

                Console.Out.Flush();

                while (true)
                {
                    receivedData = await this.readBuffer(client);

                    msg = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
                    Console.WriteLine(msg.userFrom + ": " + msg.msg);

                    foreach (Socket sClient in clientSockets)
                    {
                        //imprime el remoteEnpoint del socket

                        if (sClient != client)  // No reenviar al cliente que lo envió
                        {
                            Message message = new Message("--" + msg.userFrom + ": " + msg.msg, "Server");
                            sClient.Send(JSONSerialization.Serialize(message));
                        }
                    }

                    Console.Out.Flush();
                }
            }
            catch (SocketException s_e)
>>>>>>> 97e58e4bb4ec274dfc637f161af7663357229326
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
<<<<<<< HEAD
        }

        private async Task<User> GetUserFromClient(Socket client)
=======
            finally
            {

                Console.WriteLine("Client disconnected");
                client.Close();
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


        public byte[] getReceivedData(byte[] buffer, int bytesReceived)
>>>>>>> 97e58e4bb4ec274dfc637f161af7663357229326
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

