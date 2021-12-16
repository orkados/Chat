using System.Collections.Generic;
using Chat.Data.Models;

namespace Chat.Client.Models;

public static class DbReceiver
{
    public static List<Message> GetMessages(byte[] data, int mesCount)
    {
        var messages = new List<Message>();
        messages.AddRange(MessageProcessing.DecodeAllMessages(data, 4));
        return messages;
    }
}