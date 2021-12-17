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
        var encodedDate = Encoding.UTF8.GetBytes(message.Date);
        var dateLength = (byte) encodedDate.Length;
        var encodedUsername = Encoding.UTF8.GetBytes(message.Username);
        var usernameLength = (byte) encodedUsername.Length;
        var encodedContent = Encoding.UTF8.GetBytes(message.Content);
        var contentLengthFirstByte = (byte) (encodedContent.Length >> 8);
        var contentLengthSecondByte = (byte) encodedContent.Length;

        encodeMessage.Add(dateLength);
        encodeMessage.AddRange(encodedDate);
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
        var dateLength = encodeMessage[2];
        var usernameLength = encodeMessage[3 + dateLength];
        var contentLength = (encodeMessage[0] << 8) + encodeMessage[1] - usernameLength - dateLength - 6;

        var date = Encoding.UTF8.GetString(encodeMessage
            .Skip(3)
            .Take(dateLength)
            .ToArray());

        var username = Encoding.UTF8.GetString(encodeMessage
            .Skip(4 + dateLength)
            .Take(usernameLength)
            .ToArray());

        var content = Encoding.UTF8.GetString(encodeMessage
            .Skip(6 + usernameLength + dateLength)
            .Take(contentLength)
            .ToArray());

        return new Message
        {
            Username = username,
            Content = content,
            Date = date
        };
    }

    public static List<Message> DecodeAllMessages(byte[] encodeMessage, int offset)
    {
        var decodedMessages = new List<Message>();

        while ((encodeMessage[offset] << 8) + encodeMessage[offset + 1] != 0)
        {
            var dateLength = encodeMessage[offset + 2];
            var usernameLength = encodeMessage[offset + 3 + dateLength];
            var contentLength =
                (encodeMessage[offset] << 8) + encodeMessage[offset + 1] - usernameLength - dateLength - 6;

            var date = Encoding.UTF8.GetString(encodeMessage
                .Skip(offset + 3)
                .Take(dateLength)
                .ToArray());

            var username = Encoding.UTF8.GetString(encodeMessage
                .Skip(offset + 4 + dateLength)
                .Take(usernameLength)
                .ToArray());

            var content = Encoding.UTF8.GetString(encodeMessage
                .Skip(6 + offset + usernameLength + dateLength)
                .Take(contentLength)
                .ToArray());

            decodedMessages.Add(new Message
            {
                Date = date,
                Username = username,
                Content = content
            });

            offset += (encodeMessage[offset] << 8) + encodeMessage[offset + 1];
        }

        return decodedMessages;
    }
}