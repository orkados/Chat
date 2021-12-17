namespace Chat.Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ChatViewModel ChatViewModel { get; }
    public MessageManagerViewModel MessageManagerViewModel { get; }

    public MainWindowViewModel(ChatViewModel chatViewModel, MessageManagerViewModel messageManagerViewModel)
    {
        ChatViewModel = chatViewModel;
        MessageManagerViewModel = messageManagerViewModel;
    }
    
}