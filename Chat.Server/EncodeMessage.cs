using System;
using System.Collections.Generic;
using System.Text;
using Chat.Data.Models;

namespace Chat.Server;

public static class EncodeMessage
{
    public static byte[] Encode(Message message)
    {
        if (message.Content.Length > ushort.MaxValue)
        {
            throw new Exception("String longer than 65034 symbols");
        }

        var encode = new List<byte>();
        var lengthUsernameToBytes = (byte) message.Username.Length;
        var usernameToBytes = Encoding.UTF8.GetBytes(message.Username);
        var lengthContentToBytes = (byte) message.Content.Length;
        var contentToBytes = Encoding.UTF8.GetBytes(message.Content);

        encode.Add(lengthUsernameToBytes);
        encode.AddRange(usernameToBytes);
        encode.Add(lengthContentToBytes);
        encode.AddRange(contentToBytes);

        var firstByte = (byte) (encode.Count >> 8);
        var secondByte = (byte) encode.Count;
        encode.Insert(0, firstByte);
        encode.Insert(1, secondByte);
        return encode.ToArray();
    }
}