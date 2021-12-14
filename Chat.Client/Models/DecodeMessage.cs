using System.Linq;
using System.Text;
using Chat.Data.Models;

namespace Chat.Client.Models;

public static class DecodeMessage
{
    public static Message Decode(byte[] encodeMessage)
    {
        var username = Encoding.UTF8.GetString(encodeMessage
            .Skip(3)
            .Take(encodeMessage[2])
            .ToArray());
        var content = Encoding.UTF8.GetString(encodeMessage
            .Skip(4 + encodeMessage[2])
            .Take(encodeMessage[3 + encodeMessage[2]])
            .ToArray());
        return new Message
        {
            Username = username,
            Content = content
        };
    }
}