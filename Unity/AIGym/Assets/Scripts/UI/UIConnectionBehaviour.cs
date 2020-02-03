/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIConnectionBehaviour : MonoBehaviour
{
    // Assets to manipulate.
    public Image indicator;
    public Text description;

    // Default color and text, visible when we are not listening or connected.
    public Color defaultColor;
    public string defaultText;

    // Listening color and text, visible when we are listening and not connected.
    public Color listeningColor;
    public string listeningText;

    // Connected color and text, visible when we are connected.
    public Color connectedColor;
    public string connectedText;

    // Are we listening on a port?
    public bool Listening {
        get
        {
            return listening;
        }
        private set
        {
            listening = value;
            if (value)
            {
                indicator.color = listeningColor;
                description.text = listeningText;
            }
            else
            {
                indicator.color = defaultColor;
                description.text = defaultText;
            }
        }
    }

    private bool listening;

    // Are we connected to any client?
    public bool Connected
    {
        get
        {
            return connected;
        }
        private set
        {
            connected = value;
            if (value)
            {
                indicator.color = connectedColor;
                description.text = connectedText;
            }
            else
            {
                indicator.color = listeningColor;
                description.text = listeningText;
            }
        }
    }

    private bool connected;

    // Keep track of all socket connections and each endpoint.
    private readonly List<IPEndPoint> endPoints = new List<IPEndPoint>();
    private readonly List<Socket> connections = new List<Socket>();

    void Start()
    {
        // Set initial color and text.
        indicator.color = defaultColor;
        description.text = defaultText;
    }

    /// <summary>
    /// Keep track of a listener on a specific <see cref="IPEndPoint"/>.
    /// </summary>
    /// <param name="endPoint">Local <see cref="IPEndPoint"/> to keep track of.</param>
    public void AddListener(IPEndPoint endPoint)
    {
        endPoints.Add(endPoint);
        if (!Listening)
            Listening = true;
    }

    /// <summary>
    /// Lose track of a listener on a specific <see cref="IPEndPoint"/>.
    /// </summary>
    /// <param name="endPoint">Local <see cref="IPEndPoint"/> to lose track of.</param>
    public void RemoveListener(IPEndPoint endPoint)
    {
        endPoints.Remove(endPoint);
        if (Listening && endPoints.Count == 0)
            Listening = false;
    }

    /// <summary>
    /// Keep track of a <see cref="Socket"/> connection.
    /// </summary>
    /// <param name="endPoint"><see cref="Socket"/> to keep track of.</param>
    public void AddConnection(Socket socket)
    {
        connections.Add(socket);
        if (!Connected)
            Connected = true;
    }

    /// <summary>
    /// Lose track of a <see cref="Socket"/> connection.
    /// </summary>
    /// <param name="endPoint"><see cref="Socket"/> to lose track of.</param>
    public void RemoveConnection(Socket socket)
    {
        connections.Remove(socket);
        if (Connected && connections.Count == 0) 
            Connected = false;
    }
}
