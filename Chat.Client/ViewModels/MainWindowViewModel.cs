using System.Windows.Input;
using Chat.Client.Models;
using Chat.Client.Services;
using Chat.Data.Models;
using ReactiveUI;

namespace Chat.Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand SendMessageCommand { get; }
    public ChatViewModel ChatViewModel { get; }
    private IpConfigService IpConfig { get; }

    public MainWindowViewModel(ChatViewModel chatViewModel, IpConfigService ipConfig)
    {
        ChatViewModel = chatViewModel;
        SendMessageCommand = ReactiveCommand.Create<Message>(SendMessage);
        IpConfig = ipConfig;
    }

    private void SendMessage(Message message)
    {
        var newClient = new ChatClient(message, IpConfig.MySocket);
    }
}