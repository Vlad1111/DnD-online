using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UiServerBehaviour : MonoBehaviour
{
    ServerSocket server = ServerSocket.Instance;
    public Transform parent;
    public Text ip;
    public Button button;

    public void toggleMenu()
    {
        parent.gameObject.SetActive(!parent.gameObject.activeSelf);
    }
    public void connect()
    {
        int port = 42069;
        while (true)
        {
            try
            {
                server.Start(port, 20);
            }
            catch(SocketException)
            {
                port = (int)(2000 + Random.value * 4000);
                continue;
            }
            break;
        }
        ip.text = "Created on ip: " + GENERAL.GetLocalIPAddress() + ":" + port;
        button.gameObject.SetActive(false);
    }
}
