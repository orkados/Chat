using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chat.Client.Views;

public class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}