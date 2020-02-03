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

// @Incomplete, this custom converter is not used right now, but would help us with serializing/deserializing Commands and Entities.
// However, this requires a custom converter in aplib as well, which is harder.
class PolymorphicJsonConverter : JsonConverter {

    public override bool CanWrite => true;
    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(Entity).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        JObject item = JObject.Load(reader);

        var eType = typeof(Entity);

        switch (item["entityType"].Value<string>())
        {
            case "InteractiveEntity":
                eType = typeof(InteractiveEntity);
                break;

            case "DynamicEntity":
                eType = typeof(DynamicEntity);
                break;
            default:
                break;
        }
        
        serializer.Populate(item.CreateReader(), eType);
        return eType;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JObject o = JObject.FromObject(value);
        if (value is InteractiveEntity)
        {
            //o.AddFirst(new JProperty("type", new JValue("InteractiveEntity")));
        }
        else if (value is DynamicEntity)
        {
            //o.AddFirst(new JProperty("type", new JValue("DynamicEntity")));
        }

        o.WriteTo(writer);
    }

}

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
