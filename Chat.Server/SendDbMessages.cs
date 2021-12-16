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
        return messages.Select(MessageProcessing.Encode).ToList();
    }
}