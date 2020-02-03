/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using Newtonsoft.Json;
using System;

/// <summary>
/// These are the current RequestTypes that are used in our network.
/// </summary>
public enum RequestType
{
    DISCONNECT,
    PAUSE,
    START,
    INIT,
    AGENTCOMMAND,
    UPDATE_ENVIRONMENT
}

/// <summary>
/// Using a message as super class, the json can first be converted to a Message. 
/// With its RequestType the json can be deserialized as a Request with a certain Argument/Response type.
/// </summary>
public class Message
{
    public RequestType cmd;

    /// <summary>
    /// Deserialize a json message
    /// </summary>
    public static Message CreateFrom(string json) => JsonConvert.DeserializeObject<Message>(json);

    /// <summary>
    /// OnSend will call ToJson. If the message is not a Response/Request (the method is not overwritten) throw an error.
    /// </summary>
    public virtual string ToJson() => throw new NotImplementedException("You cannot send a Message without a response. Please implement the ToJson method.");
}
