using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket c_socket;
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

        public void Send(string msg)
        {
            byte[] msgBuffer = Encoding.ASCII.GetBytes(msg);
            c_socket.Send(msgBuffer);

        }
      
    }
}
