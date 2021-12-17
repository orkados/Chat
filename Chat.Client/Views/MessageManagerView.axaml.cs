using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chat.Client.Views;

public class MessageManagerView : UserControl
{
    private TextBox _message;
    private Button _sendButton;

    public MessageManagerView()
    {
        InitializeComponent();
        _message = this.FindControl<TextBox>("Message");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SendButtonPressed(object? sender, EventArgs args)
    {
        _message.Clear();
    }
}