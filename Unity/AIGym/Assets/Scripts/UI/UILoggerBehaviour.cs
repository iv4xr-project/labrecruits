/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// The UILoggerBehaviour can be used to log messages to be seen by the user.
/// </summary>
public class UILoggerBehaviour : MonoBehaviour
{
    /// <summary>
    /// TODO: Logs a message to the log window.
    /// </summary>
    public void Log(string msg)
    {
#if false 
        Debug.Log(msg);
#endif
    }

    /// <summary>
    /// TODO: Logs a message together with the remote endpoint to the log window.
    /// </summary>
    public void Log(Socket handler, string msg)
    {
#if false
        try {
            Debug.Log(handler.RemoteEndPoint.ToString() + ": " + msg);
        }
        catch (Exception e)
        {
            if (e.GetType().IsAssignableFrom(typeof(ObjectDisposedException)))
                Log(msg);
            else
                Debug.Log(e.ToString());
        }
#endif
    }

}
