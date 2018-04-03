using System;
using System.Net;
using System.Threading;

namespace EEBridgeIrc
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server(IPAddress.Any, 23000, "localhost");
                server.Start();

            Console.WriteLine("EEBridgeIRC started.");
            Console.ReadLine();
        }
    }
}