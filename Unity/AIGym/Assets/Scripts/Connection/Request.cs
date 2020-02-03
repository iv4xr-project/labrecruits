/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using Newtonsoft.Json;

/// <summary>
/// This is a concrete but generic implementation of Message.
/// Using ArgumentType and ResponseType, the json message will be deserialized accordingly.
/// Maurin 13/12/1029
/// </summary>
/// <typeparam name="ArgumentType">This is the argument type sent from APlib.</typeparam>
/// <typeparam name="ResponseType">This is the response type that APlib expects.</typeparam>
public class Request<ArgumentType, ResponseType> : Message
{
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
    public Request<ArgumentType, ResponseType> WithResponse(ResponseType response)
    {
        SetResponse(response);
        return this;
    }

    /// <summary>
    /// Execute the deserialisation within this class with the right settings.
    /// </summary>
    public new static Request<ArgumentType, ResponseType> CreateFrom(string message) => JsonConvert.DeserializeObject<Request<ArgumentType, ResponseType>>(message, settings);

    /// <summary>
    /// Execute the serialisation within this class.
    /// </summary>
    public override string ToJson() => JsonConvert.SerializeObject(result);
}

/// <summary>
/// Whenever APlib sends a null argument (this is common for a DISCONNECT for example),
/// this class makes it possible to create a request as a Request<object, ResponseType> by simply calling Request<ResponseType>
/// </summary>
public class Request<ResponseType>
{
    public static Request<object, ResponseType> CreateFrom(string message) => Request<object, ResponseType>.CreateFrom(message);
}
