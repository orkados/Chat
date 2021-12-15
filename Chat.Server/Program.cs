using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Data;

namespace Chat.Server;

public static class Program
{
    private static async Task Main()
    {
        await using var db = new MyDbContext();

        var cancellationSource = new CancellationTokenSource();
        var server = new ChatServer();

        server.Start(cancellationSource.Token);

        Console.ReadLine();
        cancellationSource.Cancel();
    }
}