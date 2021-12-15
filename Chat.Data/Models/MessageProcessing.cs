using System.Text;

namespace Chat.Data.Models;

public static class MessageProcessing
{
    public static byte[] Encode(Message message)
    {
        if (message.Content.Length > ushort.MaxValue)
        {
            throw new Exception("String longer than 65534 symbols");
        }

        var encodeMessage = new List<byte>();
        var usernameLength = (byte) message.Username.Length;
        var encodedUsername = Encoding.UTF8.GetBytes(message.Username);
        var contentLengthFirstByte = (byte) (message.Content.Length >> 8);
        var contentLengthSecondByte = (byte) message.Content.Length;
        var encodedContent = Encoding.UTF8.GetBytes(message.Content);

        encodeMessage.Add(usernameLength);
        encodeMessage.AddRange(encodedUsername);
        encodeMessage.Add(contentLengthFirstByte);
        encodeMessage.Add(contentLengthSecondByte);
        encodeMessage.AddRange(encodedContent);

        var firstByteMessageLength = (byte) ((encodeMessage.Count + 2) >> 8);
        var secondByteMessageLength = (byte) (encodeMessage.Count + 2);
        encodeMessage.Insert(0, firstByteMessageLength);
        encodeMessage.Insert(1, secondByteMessageLength);

        return encodeMessage.ToArray();
    }

    public static Message Decode(byte[] encodeMessage)
    {
        var usernameLength = encodeMessage[2];
        var contentLength = (encodeMessage[0] << 8) + encodeMessage[1] - usernameLength - 5;

        var username = Encoding.UTF8.GetString(encodeMessage
            .Skip(3)
            .Take(usernameLength)
            .ToArray());

        var content = Encoding.UTF8.GetString(encodeMessage
            .Skip(5 + usernameLength)
            .Take(contentLength)
            .ToArray());

        return new Message
        {
            Username = username,
            Content = content
        };
    }
}