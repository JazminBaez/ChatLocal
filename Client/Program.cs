using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program()
    {
        public void Main()
        {
            Client c = new Client("localHost", 4404);
            c.Start();
            c.Send("hola server");
        }
    }
}