using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program()
    {
        static void Main(String[] args)
        {
            Client c = new Client("localHost", 4404);
            c.Start();
            string msg;
            while (true)
            {
                Console.WriteLine("Enter message: ");
                msg = Console.ReadLine();
                if (msg == "exit")
                {
                    break;
                }
                c.Send(msg);
                
                Console.WriteLine("-" + DateTime.Now + ": " + msg);
            }
            Console.ReadKey();
        }
    }
}