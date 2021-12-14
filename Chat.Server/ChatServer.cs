using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Chat.Data;

namespace Chat.Server;

public class ChatServer
{
    private string Host { get; init; } = "192.168.115.101:8895";

    private readonly TcpListener _server;
    private readonly BlockingCollection<Socket> _clients;
    private readonly ConcurrentQueue<byte[]> _messagesQueue;

    private Task _connectionsTask;
    private Task _messagesTask;
    private Task _broadcastTask;

    public ChatServer()
    {
        _server = new TcpListener(IPEndPoint.Parse(Host));
        _clients = new BlockingCollection<Socket>();
        _messagesQueue = new ConcurrentQueue<byte[]>();
    }

    public void Start(CancellationToken cancellationToken)
    {
        _connectionsTask = StartReceivingConnections(cancellationToken);
        _messagesTask = StartReceivingMessages(cancellationToken);
        _broadcastTask = StartBroadcastMessages(cancellationToken);
    }

    private async Task StartReceivingConnections(CancellationToken cancellationToken)
    {
        _server.Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("W8 4 connections.");
            var socket = await _server.AcceptSocketAsync(cancellationToken);
            _clients.Add(socket, cancellationToken);
            Console.WriteLine($"Client {socket.RemoteEndPoint} connected.");
        }

        _server.Stop();
    }

    private async Task StartReceivingMessages(CancellationToken cancellationToken)
    {
        await using var db = new MyDbContext();

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100, cancellationToken);

            foreach (var client in _clients)
            {
                var data = new byte[256];
                if (client.Available > 0)
                {
                    client.Receive(data);
                    var t = new byte[] {1, 1, 1, 1, 1};
                    if (data.Take(5).SequenceEqual(t))
                    {
                        var dbMessages = SendDbMessages.SendMessages();
                        var dataForSending = new List<byte>();

                        foreach (var sendMessage in dbMessages)
                        {
                            dataForSending.AddRange(sendMessage);
                        }

                        client.Send(dataForSending.ToArray());
                        await client.DisconnectAsync(false, cancellationToken);
                    }

                    else
                    {
                        var message = DecodeMessage.Decode(data);
                        await db.Messages.AddAsync(message, cancellationToken);
                        await db.SaveChangesAsync(cancellationToken);
                        _messagesQueue.Enqueue(EncodeMessage.Encode(message));
                    }
                }
            }
        }
    }

    private async Task StartBroadcastMessages(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100, cancellationToken);
            while (_messagesQueue.TryDequeue(out var message))
            {
                foreach (var client in _clients)
                {
                    client.Send(message);
                }
            }
        }
    }
}