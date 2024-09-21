using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        public void Start()
        {
            c_socket.Connect(endPoint);
        }

        public void SendMessage(string msg, string userFrom)
        {
            Message message = new Message(msg, userFrom);
            string jsonString = JsonSerializer.Serialize(message);
            Console.Write("DEPUERACION: SENDMESSAGE : " + jsonString);






            c_socket.Send(JSONSerialization.Serialize(message));

        }

        public String Received()
        {
            byte[] buffer = new byte[1024];
            c_socket.Receive(buffer);
            return byteToString(buffer);
        }

        public void SendObject(object toSend)
        {
            User user = (User)toSend;
            c_socket.Send(JSONSerialization.Serialize(toSend));
            string jsonString = JsonSerializer.Serialize(toSend);

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
