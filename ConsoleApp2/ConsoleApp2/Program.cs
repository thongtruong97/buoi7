using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Net.NetworkInformation;

namespace ConsoleApp2
{
    class Broadcast
    {
         static void Main(string[] args)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep1 = new IPEndPoint(IPAddress.Broadcast,
            9050);
            IPEndPoint iep2 = new
            IPEndPoint(IPAddress.Parse("192.168.1.254"), 9050);
            string hostname = Dns.GetHostName();
            int timer = 0;
            while(true)
            {
                byte[] data = Encoding.ASCII.GetBytes(hostname);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                sock.SendTo(data, iep1);
                sock.SendTo(data, iep2);
                sock.Close();
                Thread.Sleep(1000);
            }
           
        }

    }
}
