using System.Collections.Generic;
using System.Linq;
using Chat.Data;
using Chat.Data.Models;

namespace Chat.Server;

public static class SendDbMessages
{
    public static List<byte[]> SendMessages()
    {
        using var db = new MyDbContext();

        var messages = db.Messages.ToList();
        var encodeMessages = new List<byte[]>();
        foreach (var message in messages)
        {
            encodeMessages.Add(MessageProcessing.Encode(message));
        }

        return encodeMessages;
    }
}