using System.Collections.Generic;
using System.Linq;
using Chat.Data.Models;

namespace Chat.Client.Models;

public static class GetMessagesFromDb
{
    public static List<Message> GetMessages(byte[] data)
    {
        var list = new List<Message>();

        var counter = 0;
        do
        {
            counter++;
            if (data[counter] != 0)
            {
                var lengthMes = data[counter] + 2;
                counter--;
                var tmpArr = data
                    .Skip(counter).Take(lengthMes).ToArray();
                list.Add(DecodeMessage.Decode(tmpArr));
                counter += lengthMes;
            }
            else
            {
                counter = 2049;
            }
        } while (counter < 2048);

        return list;
    }
}