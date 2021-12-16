using System;
using System.Collections.Generic;
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

    private static readonly byte[] GetMessagesCommand = {0, 4, 1, 1, 1, 1};
    private byte[] _buffer;


    public ChatViewModel(IpConfigService ipConfig)
    {
        ConnectionSocket = ipConfig.MySocket;
        Messages = new ObservableCollection<Message>();
        _buffer = new byte[16 * 1024];
        Start();
        Refresh();
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
            if (ConnectionSocket.Available > 0)
            {
                Array.Clear(_buffer);
                ConnectionSocket.Receive(_buffer, 2, SocketFlags.None);
                var length = ProceedMessageLength.GetMessageLength(_buffer);

                while (length >= _buffer.Length)
                {
                    _buffer = new byte[_buffer.Length * 2];
                }

                ConnectionSocket.Receive(_buffer, 2, length, SocketFlags.None);

                var message = MessageProcessing.Decode(_buffer);
                Messages.Add(message);
            }
        }
    }

    private void Refresh()
    {
        ConnectionSocket.Send(GetMessagesCommand);
        ConnectionSocket.Receive(_buffer, 4, SocketFlags.None);

        var length = ProceedMessageLength.GetDbMessagesLength(_buffer);

        while (length >= _buffer.Length)
        {
            _buffer = new byte[_buffer.Length * 2];
        }

        ConnectionSocket.Receive(_buffer, 4, length, SocketFlags.None);
        Messages.Clear();
        Messages.AddRange(DbReceiver.GetMessages(_buffer, length));
    }
}