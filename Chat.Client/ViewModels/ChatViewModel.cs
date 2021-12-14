using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Client.Models;
using Chat.Client.Services;
using Chat.Data.Models;
using DynamicData;

namespace Chat.Client.ViewModels;

public class ChatViewModel : ViewModelBase
{
    public ObservableCollection<Message> Messages { get; set; }
    private static Socket ConnectionSocket { get; set; }

    private Task _receiveNewMessagesTask;


    public ChatViewModel(IpConfigService ipConfig)
    {
        ConnectionSocket = ipConfig.MySocket;
        Messages = new ObservableCollection<Message>();
        Refresh();
        Start();
    }

    private void Start()
    {
        _receiveNewMessagesTask = ClientListener();
    }

    private async Task ClientListener()
    {
        while (true)
        {
            await Task.Delay(100);
            var data = new byte[256];
            if (ConnectionSocket.Available > 0)
            {
                ConnectionSocket.Receive(data);
                var message = DecodeMessage.Decode(data);
                Messages.Add(message);
            }
        }
    }

    private void Refresh()
    {
        var data = new byte[2048];
        ConnectionSocket.Send(new byte[] {1, 1, 1, 1, 1, 1});
        ConnectionSocket.Receive(data);
        Messages.Clear();
        Messages.AddRange(GetMessagesFromDb.GetMessages(data));
    }
}