using System.Net.Sockets;

namespace Chat.Client.Services;

public class IpConfigService
{
    public Socket MySocket { get; set; }


    public IpConfigService()
    {
        MySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        MySocket.Connect("192.168.115.60", 8895);
    }
}