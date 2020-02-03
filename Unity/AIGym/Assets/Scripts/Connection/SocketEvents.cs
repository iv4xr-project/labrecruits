/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Declaring these calsses exposes lifecycle events to the event system.
/// Note that these socket events use ThreadSafeUnityEvents!
/// </summary>

[Serializable]
public class SocketListenEvent : ThreadSafeUnityEvent<IPEndPoint> { }

[Serializable]
public class SocketConnectionEvent : ThreadSafeUnityEvent<Socket> { }

[Serializable]
public class SocketMessageEvent : ThreadSafeUnityEvent<Socket, string> { }

[Serializable]
public class SocketErrorEvent : ThreadSafeUnityEvent<string, bool, ErrorType> { }
