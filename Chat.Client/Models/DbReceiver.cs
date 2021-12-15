using System.Collections.Generic;
using System.Linq;
using Chat.Data.Models;

namespace Chat.Client.Models;

public class DbReceiver
{
    public static List<Message> GetMessages(byte[] data, int mesCount)
    {
        var messages = new List<Message>();
        var counter = 0;
        var lengthMes = ProceedMessageLength.GetLength(data);

        do
        {
            var tmpArr = data
                .Skip(counter)
                .Take(lengthMes)
                .ToArray();
            messages.Add(MessageProcessing.Decode(tmpArr));
            counter += lengthMes;

            if (counter >= mesCount)
            {
                break;
            }

            lengthMes = ProceedMessageLength.GetLength(data.Skip(counter).ToArray());
        } while (counter < mesCount);

        return messages;
    }
}