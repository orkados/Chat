using System.Collections.Generic;
using Chat.Data.Models;

namespace Chat.Client.Models;

public static class DecodeDbMessages
{
    public static List<Message> Decode(List<byte[]> encodedMessages)
    {
        var decodedMessages = new List<Message>();
        foreach (var encodeMessage in encodedMessages)
        {
            decodedMessages.Add(MessageProcessing.Decode(encodeMessage));
        }

        return decodedMessages;
    }
}