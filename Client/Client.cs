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

        Socket c_socket;
        User user;
        public Client(string ip, int port) {
            host = Dns.GetHostEntry(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            c_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Start()
        {
            await c_socket.ConnectAsync(endPoint);

            Console.WriteLine("Conectado al servidor!");

            Console.Write("Ingrese alias: ");
            string alias = Console.ReadLine();
            User user = new User(alias);
            SendObject(user);

            // Iniciar la recepción de mensajes en segundo plano
            _ = Task.Run(() => serverConnection(c_socket));

            // Bucle de envío de mensajes
            while (true)
            {
                Console.WriteLine("Envia: ");
                string msg = Console.ReadLine();
                SendMessage(msg, alias);
            }
        }

        public async Task serverConnection(Socket c_socket)
        {
            try
            {
                Console.WriteLine("Debbug: ESCUCHANDO SERVER");

                while (true)
                {
                    // Buffer para recibir datos del servidor
                    byte[] buffer = new byte[1024];

                    // Esperar los datos recibidos
                    int bytesReceived = await c_socket.ReceiveAsync(buffer, SocketFlags.None);

                    // Si bytesReceived es 0, significa que la conexión se cerró
                    if (bytesReceived == 0)
                    {
                        Console.WriteLine("Servidor ha cerrado la conexión.");
                        break; // Salir del bucle si la conexión se cierra
                    }

                    // Deserializar el mensaje recibido
                    byte[] receivedData = new byte[bytesReceived];
                    Array.Copy(buffer, receivedData, bytesReceived);

                    // Asegurarse de que los datos se están recibiendo y deserializando correctamente
                    Message receivedMessage;
                    try
                    {
                        receivedMessage = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al deserializar el mensaje: " + ex.Message);
                        continue; // Seguir recibiendo mensajes aunque ocurra un error
                    }

                    // Imprimir el mensaje recibido
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
            c_socket.Send(JSONSerialization.Serialize(message));

        }

        public async Task<Message> Received()
        {
            byte[] buffer = new byte[1024];
            int bytesReceived;
            Message msg;

            c_socket.Receive(buffer);
            bytesReceived = await c_socket.ReceiveAsync(buffer, SocketFlags.None);

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);

           return msg = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
            
            
        }

        public void SendObject(object toSend)
        {
            if (c_socket == null || !c_socket.Connected)
            {
                throw new InvalidOperationException("El socket no está conectado.");
            }

            User user = (User)toSend;
            c_socket.Send(JSONSerialization.Serialize(toSend));
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
