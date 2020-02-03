/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using Newtonsoft.Json;
using System;

/// <summary>
/// These are the current actions the agent is able to perform in the gym.
/// </summary>
public enum AgentCommandType
{
    DONOTHING,
    MOVETOWARD,
    INTERACT
}

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(AgentCommandConverter))]
public abstract class Command 
{
    public AgentCommandType cmd;
    public string agentId;
    public string targetId;

    protected static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

    public static Command CreateFrom(string json) => JsonConvert.DeserializeObject<Command>(json);

    public static AgentCommandType AgentCommandType(string input) => (AgentCommandType)Enum.Parse(typeof(AgentCommandType), input);
}

public class AgentCommand<ArgumentType> : Command
{
    /// <summary>
    /// This is the specific argument needed to execute the action
    /// If no argument is needed, this argument is null
    /// </summary>
    public ArgumentType arg;

    public new static AgentCommand<ArgumentType> CreateFrom(string message) => JsonConvert.DeserializeObject<AgentCommand<ArgumentType>>(message, settings);
}

public class AgentCommand : AgentCommand<object>
{
    public new static AgentCommand<object> CreateFrom(string message) => JsonConvert.DeserializeObject<AgentCommand<object>>(message, settings);
}
