using Assets.Script.Server.Commands;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ServerSocket : IDisposable
{
    public class ConnectedClient
    {
        public Socket client;
        public TcpListener server;
        public ServerSocket observer;

        public ConnectedClient(Socket client, TcpListener server, ServerSocket observer)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));

            Thread thread = new Thread(() => this.Lisen());
            thread.Start();
        }

        public bool isConected()
        {
            return server != null && client.Connected;
        }

        public void Lisen()
        {
            while (isConected())
            {
                byte[] buffer = new byte[1024 * 1024];
                var size = client.Receive(buffer);
                if (size > 0)
                {
                    var cmd = CommandBuilder.Instance.deserilize(buffer);
                    observer.sendCommandToOthers(cmd, this);
                }
            }
        }

        public void Send(string message)
        {
            var bites = System.Text.Encoding.UTF8.GetBytes(message);
            Send(bites);
        }

        public void Send(byte[] data)
        {
            if (isConected())
                client.Send(data, data.Length, SocketFlags.None);
        }

        public void Send(Command cmd)
        {
            var data = CommandBuilder.Instance.serilize(cmd);
            Send(data);
        }

        public void Stop()
        {
            server = null;
        }
    }
    private static ServerSocket instance;
    private static ServerSocket Instance => getInstance();
    private TcpListener socket;
    private List<ConnectedClient> clients = new List<ConnectedClient>();
    private Thread lisenerThread = null;

    public static ServerSocket getInstance(int port = 42069, int nrMaxPlayers = 7)
    {
        if (instance == null)
            instance = new ServerSocket(port, nrMaxPlayers);
        return instance;
    }
    private ServerSocket(int port, int nrPlayers = 7)
    {
        socket = new TcpListener(IPAddress.Any, port);
        Start(port, nrPlayers);
    }

    public void Accept(int nrPlayers)
    {
        Socket newSocket;
        Debug.Log("server oppened");
        for (int i = 0; i < nrPlayers; i++)
        {
            newSocket = socket.AcceptSocket();
            Debug.Log(newSocket.RemoteEndPoint.ToString());
            //RoomMenuBehaviour.instance.log(newSocket.RemoteEndPoint.ToString());

            lock (clients)
            {
                var newC = new ConnectedClient(newSocket, socket, this);
                clients.Add(newC);
            }
        }
    }

    public void Start(int port = 42069, int nrPlayers = 7)
    {
        RoomMenuBehaviour.instance.log("Started server on post " + port);

        socket.Start();
        lisenerThread = new Thread(() => this.Accept(nrPlayers));
        lisenerThread.Start();
    }

    public void Stop()
    {
        socket.Stop();
        for (int i = 0; i < clients.Count; i++)
            clients[i].Stop();
        try
        {
            lisenerThread.Abort();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    public void Dispose()
    {
        Stop();
    }

    public void sendCommandToOthers(Command command, ConnectedClient sender)
    {
        foreach (var cl in clients)
            if (cl != sender)
                sender.Send(command);
    }
}
