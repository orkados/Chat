using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Chat.Data.Models;

namespace Chat.Client.Models;

public class ConvertToMessage : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        return new Message()
        {
            Username = values[0] as string ?? string.Empty,
            Content = values[1] as string ?? string.Empty
        };
    }
}