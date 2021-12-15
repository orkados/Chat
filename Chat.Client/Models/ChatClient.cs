using System;
using System.Net.Sockets;
using Chat.Data.Models;

namespace Chat.Client.Models;

public class ChatClient
{
    public ChatClient(Message message, Socket socket)
    {
        try
        {
            var data = MessageProcessing.Encode(message);
            socket.Send(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}