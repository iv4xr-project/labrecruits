/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IResponse {
    string GetResponseString();
}

/// <summary>
/// This is a concrete but generic implementation of Message.
/// Using ArgumentType and ResponseType, the json message will be deserialized accordingly.
/// Maurin 13/12/1029
/// </summary>
/// <typeparam name="ArgumentType">This is the argument type sent from APlib.</typeparam>
/// <typeparam name="ResponseType">This is the response type that APlib expects.</typeparam>
public class RawRequest<ArgumentType, ResponseType> : Message
{
    public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new APLSyncedConverter() }
    };

    public ArgumentType arg;
    public ResponseType result;

    /// <summary>
    /// These settings must be used to avoid null cast exceptions.
    /// </summary>
    private static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

    /// <summary>
    /// The result variable will be used by the SocketServer to send back a message.
    /// </summary>
    public void SetResponse(ResponseType response) => this.result = response;

    /// <summary>
    /// This method makes the use of request easier. For example:
    /// Respond(disconnectRequest.WithResponse(true));
    /// </summary>
    /// <returns>The same instance of the request, but with the result set.</returns>
    public RawRequest<ArgumentType, ResponseType> WithResponse(ResponseType response)
    {
        SetResponse(response);
        return this;
    }

    /// <summary>
    /// Execute the deserialisation within this class with the right settings.
    /// </summary>
    public new static RawRequest<ArgumentType, ResponseType> CreateFrom(string message) => JsonConvert.DeserializeObject<RawRequest<ArgumentType, ResponseType>>(message, settings);

    /// <summary>
    /// Execute the serialisation within this class.
    /// </summary>
    public override string ToJson()
    {
        Debug.Log(result);
        return JsonConvert.SerializeObject(result, serializerSettings);
    }
}

/// <summary>
/// Whenever APlib sends a null argument (this is common for a DISCONNECT for example),
/// this class makes it possible to create a request as a Request<object, ResponseType> by simply calling Request<ResponseType>
/// </summary>
public class RawRequest<ResponseType>
{
    public static RawRequest<object, ResponseType> CreateFrom(string message) => RawRequest<object, ResponseType>.CreateFrom(message);
}

/// <summary>
/// Special kind of request that wraps around IAPLSerializable classes.
/// </summary>
/// <typeparam name="ArgumentType"></typeparam>
/// <typeparam name="ResponseType"></typeparam>
public class Request<ArgumentType, ResponseType> : RawRequest<ArgumentType, ResponseType> where ResponseType : IAPLSerializable
{
    /// <summary>
    /// Execute the serialisation within this class.
    /// </summary>
    public override string ToJson() => result.APLSerialize().ToString();
}

public interface IAPLSerializable
{
    JObject APLSerialize();
}

