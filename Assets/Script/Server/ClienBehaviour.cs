using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using Assets.Script.Server.Commands;
using Assets.Script.Server;

public class ClienBehaviour : MonoBehaviour
{
    public static ClienBehaviour instace;
    private void Awake()
    {
        instace = this;
    }
    TcpClient client;

    public void connect(string ip, int port)
    {
        try
        {

            if (client != null && client.Connected)
            {
                client.Close();
            }
        }
        catch(Exception ex)
        {
            UiLogBehaviour.instance.addMesage("Error: Could not connect to server");
            Debug.LogWarning(ex);
            return;
        }
        client = new TcpClient(ip, port);
        if (client.Connected)
        {
            Debug.Log("connected");
            Thread thread = new Thread(this.Lisen);
            thread.Start();
            Send(CommandBuilder.Instance.connected());
        }
        else
            Debug.LogWarning("not connected");
    }

    public bool isConnected()
    {
        return client != null && client.Connected;
    }

    public void Lisen()
    {
        try
        {
            while (isConnected())
            {
                byte[] data = new byte[1024 * 1024];
                String responseData = String.Empty;
                NetworkStream stream = client.GetStream();

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length); //(**This receives the data using the byte method**)
                                                                 //responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes); //(**This converts it to string**)
                                                                 //Debug.Log("From server: " + responseData);
                var cmd = CommandBuilder.Instance.deserilize(data);

                //CommandInterpretor.Instance.doCommand(cmd);
                WordBehaviour.instance.addCommand(cmd);
            }
        }
        catch(SocketException ex)
        {
            Debug.LogWarning(ex);
        }
    }
    public void Send(byte[] data)
    {
        if(isConnected())
            client.Client.Send(data);
    }
    public void Send(string[] mesages)
    {

    }

    public void Send(string message)
    {
        if (client.Connected)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            Send(buffer);
        }
    }

    public void Send(Command cmd)
    {
        var data = CommandBuilder.Instance.serilize(cmd);
        Send(data);
    }

    public void connect(UiButtonDataDTO data)
    {
        connect(data.str, data.nr);
    }

    public void connect(InputField field)
    {
        string[] ips = field.text.Split(':');
        if(ips.Length == 2)
        {
            int port = 0;
            if(int.TryParse(ips[1], out port))
            {
                connect(ips[0], port);
            }
        }
    }
}
