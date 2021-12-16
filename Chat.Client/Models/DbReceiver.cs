using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Chat.Data.Models;

namespace Chat.Client.Models;

public class DbReceiver
{
    public static List<Message> GetMessages(byte[] data, int mesCount)
    {
        var messages = new List<Message>();


        messages.AddRange(MessageProcessing.DecodeAllMessages(data, 4));

        return messages;
    }
}