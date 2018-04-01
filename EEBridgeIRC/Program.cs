using System.Net;
using System.Threading;

namespace EEBridgeIrc
{
    internal static class Program
    {
         private static void Main(string[] args)
         {
             var ctx = new SingleThreadSynchronizationContext();
             SynchronizationContext.SetSynchronizationContext(ctx);

             var server = new Server(IPAddress.Any, 23000);
             Server.HostName = "localhost";
             server.Run();

             ctx.RunMessagePump();
         }
    }
}