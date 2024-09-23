using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Serialization;
using Server;

namespace Client
{
    public class Client
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_client;
        private User user { get; set; }

        public Client(string ip, int port) {
            host = Dns.GetHostEntry(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            s_client = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Start()
        {
            await s_client.ConnectAsync(endPoint);
            Console.WriteLine("Conectado al servidor!");

            await HandleUserAlias();

            _ = Task.Run(() => serverConnection(s_client));

            await HandleUserMessages();
        }

      

        private async Task HandleUserAlias()
        {
            Console.Write("Ingrese alias: ");
            string alias = Console.ReadLine();
            User user = new User(alias);
            this.user = user;
            SendObject(user);
        }

        private async Task HandleUserMessages()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                SendMessage(msg, user.alias); 
            }
        }


        public async Task serverConnection(Socket s_client)
        {
            try
            {
                while (true)
                {
                    
                    byte[] buffer = new byte[1024];

                    int bytesReceived = await s_client.ReceiveAsync(buffer, SocketFlags.None);

                    if (bytesReceived == 0)
                    {
                        Console.WriteLine("Servidor ha cerrado la conexión.");
                        break; 
                    }

                    byte[] receivedData = new byte[bytesReceived];
                    Array.Copy(buffer, receivedData, bytesReceived);

                    
                    Message receivedMessage;
                    try
                    {
                        receivedMessage = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
         
                        Console.WriteLine(">>>" + receivedMessage.msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al deserializar el mensaje: " + ex.Message);
                        continue; 
                    }

             
                }
            }
            catch (SocketException s_e)
            {
                Console.WriteLine("Error de conexión: " + s_e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error general: " + e.Message);
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

        public void SendMessage(string msg, string userFrom)
        {
            Message message = new Message(msg, userFrom);
            string jsonString = JsonSerializer.Serialize(message);
            s_client.Send(JSONSerialization.Serialize(message));

        }

        public async Task<Message> Received()
        {
            byte[] buffer = new byte[1024];
            int bytesReceived;
            Message msg;

            s_client.Receive(buffer);
            bytesReceived = await s_client.ReceiveAsync(buffer, SocketFlags.None);

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);

           return msg = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
            
            
        }

        public void SendObject(object toSend)
        {
            if (s_client == null || !s_client.Connected)
            {
                throw new InvalidOperationException("El socket no está conectado.");
            }

            User user = (User)toSend;
            s_client.Send(JSONSerialization.Serialize(toSend));
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
