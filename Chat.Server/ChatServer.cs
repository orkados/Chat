using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Chat.Data;
using Chat.Data.Models;

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
        var length = new byte[2];
        var protocolBody = new byte[4];

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100, cancellationToken);

            foreach (var client in _clients)
            {
                bool flag = true;

                if (client.Available > 0)
                {
                    client.ReceiveBufferSize = 2;
                    client.Receive(length);

                    if (length.Take(2).SequenceEqual(new byte[] {0, 4}))
                    {
                        client.ReceiveBufferSize = 4;
                        client.Receive(protocolBody);
                        foreach (var num in protocolBody)
                        {
                            if (num != 1)
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                        {
                            var dbMessages = SendDbMessages.SendMessages();
                            var dataForSending = new List<byte>();

                            foreach (var sendMessage in dbMessages)
                            {
                                dataForSending.AddRange(sendMessage);
                            }

                            var allMessagesLengthFirstByte = (byte) (dataForSending.Count >> 8);
                            var allMessageLengthSecondByte = (byte) dataForSending.Count;
                            dataForSending.Insert(0, allMessagesLengthFirstByte);
                            dataForSending.Insert(1, allMessageLengthSecondByte);
                            client.Send(dataForSending.ToArray());
                        }
                    }

                    else
                    {
                        client.ReceiveBufferSize = ProceedMessageLength.GetLength(length);
                        var data = new byte[client.ReceiveBufferSize];
                        client.Receive(data);
                        var tmp = data.ToList();
                        tmp.InsertRange(0, length);
                        var message = MessageProcessing.Decode(tmp.ToArray());
                        await db.Messages.AddAsync(message, cancellationToken);
                        await db.SaveChangesAsync(cancellationToken);
                        _messagesQueue.Enqueue(MessageProcessing.Encode(message));
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
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Возникла ошибка при попытке отправить сообщение клиенту: " +
                                          $"{client.LocalEndPoint} | {exception}");
                    }
                }
            }
        }
    }
}