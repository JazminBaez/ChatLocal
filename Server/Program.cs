using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
         static void Main(String[] args)
        {
            Server server = new Server("localHost", 57343);
            server.Start();
            Console.ReadKey();
        }
    }
}