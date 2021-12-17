using System.Windows.Input;
using Chat.Client.Models;
using Chat.Client.Services;
using Chat.Data.Models;
using ReactiveUI;

namespace Chat.Client.ViewModels;

public class MessageManagerViewModel : ViewModelBase
{
    public ICommand SendMessageCommand { get; }

    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    private IpConfigService IpConfig { get; }
    private string _content;

    public MessageManagerViewModel(IpConfigService ipConfig)
    {
        SendMessageCommand = ReactiveCommand.Create<Message>(SendMessage);
        IpConfig = ipConfig;
    }

    private void SendMessage(Message message)
    {
        var newClient = new ChatClient(message, IpConfig.MySocket);
        Content = "";
    }
}