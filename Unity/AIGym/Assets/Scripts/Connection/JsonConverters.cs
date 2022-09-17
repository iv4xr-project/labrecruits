/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;


// From: https://stackoverflow.com/questions/20995865/deserializing-json-to-abstract-class
public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(Command).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}

public class AgentCommandConverter : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Command));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject command = JObject.Load(reader);
        switch (Command.AgentCommandType(command["cmd"].Value<string>()))
        {
            case AgentCommandType.DONOTHING:
                return JsonConvert.DeserializeObject<AgentCommand>(command.ToString(), SpecifiedSubclassConversion);
            case AgentCommandType.INTERACT:
                return JsonConvert.DeserializeObject<AgentCommand>(command.ToString(), SpecifiedSubclassConversion);
            case AgentCommandType.MOVETOWARD:
                return JsonConvert.DeserializeObject<AgentCommand<Tuple<Vector3, bool>>>(command.ToString(), SpecifiedSubclassConversion);
            default:
                throw new Exception();
        }
        throw new NotImplementedException();
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
}
