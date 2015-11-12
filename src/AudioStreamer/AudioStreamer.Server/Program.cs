using System;
using XSockets.Core.Common.Socket;
using XSockets.Plugin.Framework;

namespace AudioStreamer.Server
{
    /// <summary>
    /// Just starts a XSockets server self-hosted.
    /// This project references the AudioStreamer.Modules project which have all logic
    /// So you can just refernce that module if you host in a worker-role, top-shelf (service) etc.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = Composable.GetExport<IXSocketServerContainer>())
            {
                container.Start();
                Console.ReadLine();
            }
        }
    }
}
