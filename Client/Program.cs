using Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program()
    {
        static async Task Main(String[] args)
        {
            Client c = new Client("localhost", 4404);

            // Iniciar la conexión
            await c.Start(); // Espera a que termine Start()
        }
    }
}