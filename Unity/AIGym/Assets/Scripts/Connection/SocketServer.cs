/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// State object for reading client data asynchronously using worker threads.
/// </summary>
public class StateObject
{
    public Socket socket; //Client socket.
    public const int BufferSize = 1024; //Size of receive buffer.
    public byte[] buffer = new byte[BufferSize]; //Receive buffer.
    public StringBuilder sb = new StringBuilder();
}

/// <summary>
/// Socket server object used to establish a connection with clients and to communicate to connected clients.
/// </summary>
public class SocketServer : MonoBehaviour
{
    // Socket settings
    public string localhost = "127.0.0.1";
    public int port = 8053;
    public int backlog = 32;

    // store endpoint so we can end the listener at any exception
    IPAddress localHostIP;
    IPEndPoint localEndPoint;

    // Exposes lifecycle events to the event system.
    public SocketListenEvent onListenStart;
    public SocketListenEvent onListenEnd;
    public SocketConnectionEvent onConnectionStart;
    public SocketConnectionEvent onConnectionEnd;
    public SocketMessageEvent onMessage;
    public SocketErrorEvent onError;

    // Defines if the server is listening for new connections or not.
    private bool _listening = false;
    public bool IsListening 
    { 
        get 
        { 
            return _listening; 
        } 
        private set 
        {
            _listening = value;
            if (value)
                onListenStart.Invoke(localEndPoint);
            else
                onListenEnd.Invoke(localEndPoint);
        } 
    }

    // Thread signal to manually block and unblock the listener thread.
    private readonly ManualResetEvent mre = new ManualResetEvent(false);

    // Keep track of all socket connections so we can eventually close them.
    private readonly List<Socket> connections = new List<Socket>();

    /// <summary>
    /// Initialize the Dispatcher
    /// </summary>
    void Awake()
    {
        //must be initialized from the main thread.
        // initializing the dispatcher in its own Awake will cause an overflow.
        Dispatcher.Init();
    }

    /// <summary>
    /// Create an IPAdress and IPEndpoint using the adress and port from the Editor
    /// </summary>
    void Start()
    {
        // Create a local endpoint object containing an ipAddress and port.
        localHostIP = IPAddress.Parse(localhost);
        localEndPoint = new IPEndPoint(localHostIP, port);

        // start listening on a saperate thread
        new Thread(new ThreadStart(Listen)) { IsBackground = true }.Start();
    }

    /// <summary>
    /// Close the server
    /// </summary>
    void OnDisable()
    {
        Close();
    }

    /// <summary>
    /// Start listening for incoming connections.
    /// </summary>
    private void Listen()
    {
        // Create a TCP/IP socket. This socket will be listening for incoming connections.
        Socket listener = null;
        
        try
        {
            listener = new Socket(localHostIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(backlog);

            IsListening = true;

            while (IsListening)
            {
                //Set the event to nonsignaled (blocking) state.
                mre.Reset();

                //Start an asynchronous socket to listen for connections.
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                //Wait until a connection is made before continuing.
                mre.WaitOne();
            }
        }
        catch (Exception)
        {
            onError.Invoke("A connection error occured.", true, ErrorType.ConnectionError);
        }
        finally
        {
            if (listener != null && listener.IsBound)
                listener.Close();
        }
    }

    /// <summary>
    /// Closes existing connections and stops listening for more.
    /// </summary>
    public void Close()
    {
        // Terminate the listening thread.
        IsListening = false;
        mre.Set();

        // Shutdown each socket connection.
        foreach (Socket connection in connections.ToArray())
            Close(connection);
    }

    /// <summary>
    /// Closes given socket connection.
    /// </summary>
    /// <param name="handler">Socket to close.</param>
    private void Close(Socket handler)
    {
        // Remove the connection from the list
        connections.Remove(handler);

        // Invoke onConnectionEnd event on the main thread.
        onConnectionEnd.Invoke(handler);

        // Cannot shutdown the connection with APlib if the connection was already 'forcibly' lost..
        if (handler.Connected)
            handler.Shutdown(SocketShutdown.Both);
     
        handler.Close();
    }

    /// <summary>
    /// Send a <see cref="string"/> to another client.
    /// </summary>
    /// <param name="handler">Socket handling the request.</param>
    /// <param name="msg">Message that will be converted to json</param>
    private void Send(Socket handler, Message msg)
    {
        string json = msg.ToJson(); // msg.ToJsonUtf8();

#if false
        Debug.Log("SEND BACK:" + json);
#endif
        // Convert the string data to byte data using ASCII encoding.
        byte[] data = Encoding.ASCII.GetBytes(json + '\n');

        try
        {
            // Begin sending the data to the remote device.
            handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), handler);
        }
        catch (Exception)
        {
            onError.Invoke("Client disconnected forcefully", true, ErrorType.ConnectionError);
        }
    }

    /// <summary>
    /// End an asychronous accept operation and start listening for data.
    /// </summary>
    /// <param name="ar"></param>
    private void AcceptCallback(IAsyncResult ar)
    {
        // Signal the listener thread to continue.
        mre.Set();

        // Retrieve the listener socket from the asynchronous state object.
        Socket listener = (Socket)ar.AsyncState;

        try
        {
            // Accept the incoming connection and
            // create a socket that handles the client requests.
            Socket handler = listener.EndAccept(ar);

            // Keep track of the established connection.
            connections.Add(handler);

            // Invoke OnConnectionStart event on the main thread.
            onConnectionStart.Invoke(handler);

            // Create the state object.
            StateObject state = new StateObject() { socket = handler };

        
            // Begin to asynchronously receive data from the connected client.
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception)
        {
            onError.Invoke("An error occured while trying to connect to a client.", true, ErrorType.ConnectionError);
            onConnectionEnd.Invoke(listener);
        }
    }

    /// <summary>
    /// End an asychronous receive operation and start listening for more data.
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.socket;

        try
        {
            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            // Check if we received data
            if (bytesRead > 0)
            {
                // Write the data from the receive buffer to the string builder object.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check if all data is received.
                if (Array.IndexOf(state.buffer, (byte)'\n') > -1)
                {
                    // Invoke OnMessage event on the main thread.
                    onMessage.Invoke(handler, state.sb.ToString());

                    // Clear the string builder object for new incoming messages.
                    state.sb.Clear();
                }

                // Begin receiving more data from the connected client.
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                // 0 size result received, connection was closed by the other party.
                Close(handler);
            }
        }
        catch(Exception)
        {
            onError.Invoke("Client disconnected forcefully", true, ErrorType.ConnectionError);
            onConnectionEnd.Invoke(handler);
        }
    }

    /// <summary>
    /// Ends an asychronous send operation.
    /// </summary>
    /// <param name="ar"></param>
    private void SendCallback(IAsyncResult ar)
    {
        // Retrieve the handler socket from the asynchronous state object.
        Socket handler = (Socket)ar.AsyncState;

        try
        {
            // Complete sending the data to the remote device.
            handler.EndSend(ar);
        }
        catch (Exception)
        {
            onError.Invoke("Client disconnected forcefully", true, ErrorType.ConnectionError);
            onConnectionEnd.Invoke(handler);
        }
    }
}
