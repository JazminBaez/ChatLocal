using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Serialization;
using System.Diagnostics;


namespace Server
{
    public class Server
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_socket;
        Socket? c_socket;

        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();

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

            client.Send(stringToBytes("Welcome to the server"));

            try
            {
               
                receivedData = this.readBuffer(client);

                user = (User)JSONSerialization.Deserialize(receivedData, typeof(User));
                alias = user.alias;

                Console.WriteLine("Cliente conectado, " + alias);
                Console.Out.Flush();

                while (true)
                {
                    receivedData = this.readBuffer(client);
                    msg = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
                    Console.WriteLine(msg.userFrom + ": " + msg.msg);
                    Console.Out.Flush();
                }
            }
            catch (SocketException s_e)
            {
                Console.WriteLine("Client disconnected");
            }
            finally
            {
                client.Close();
            }
        }

        public byte[] readBuffer(Socket client)
        {
            int bytesReceived;
            byte[] receivedData;

            byte[] buffer = new byte[1024];
            bytesReceived = await client.ReceiveAsync(buffer, SocketFlags.None);

            if (bytesReceived == 0)
            {
                Console.WriteLine($"Cliente {client.RemoteEndPoint} desconectado.");
                return null;
            }

            return receivedData = getReceivedData(buffer, bytesReceived);

        }

        public byte[] getReceivedData(byte[] buffer, int bytesReceived)
        {

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);
            return receivedData;
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
