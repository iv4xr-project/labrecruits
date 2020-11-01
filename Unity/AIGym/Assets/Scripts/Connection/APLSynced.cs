using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class APLSynced : MonoBehaviour, IAPLSerializable
{
    private static readonly JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    });

    public List<MonoBehaviour> components = new List<MonoBehaviour>();

    public bool syncBoxColliders = true;

    [HideInInspector]
    public DateTime lastTouched;

    [Layer]
    public int[] LayerIgnore;

    public JObject APLSerialize()
    {
        var data = JObject.FromObject(new SerializedGameObject(gameObject, lastTouched), serializer);

        if (syncBoxColliders)
            data["colliders"] = JArray.FromObject(gameObject
                .GetComponentsInChildren<BoxCollider>()
                .Where(x => x.enabled && !LayerIgnore.Any(l => l == x.gameObject.layer))
                .Select(x => new SerializedBoxCollider(x)));

        foreach (var item in components)
            data[item.GetType().Name] = JObject.FromObject(item, serializer);

        return data;
    }

    public void APLSync()
    {
        lastTouched = DateTime.Now;
        Debug.Log(APLSerialize().ToString());
    }
}

public class APLSyncedConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
        => objectType == typeof(APLSynced);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        => throw new NotImplementedException();

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var synced = value as APLSynced;
        synced.APLSerialize().WriteTo(writer);
    }

    public override bool CanRead => false;
}

public struct SerializedTransform
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public SerializedTransform(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation.eulerAngles;
        scale    = transform.lossyScale;
    }
}

public class SerializedGameObject
{
    public string name;
    public string tag;
    public int id;
    public SerializedTransform transform;
    public DateTime lastTouched;

    public SerializedGameObject(GameObject gameObject, DateTime lastTouched)
    {
        name = gameObject.name;
        tag = gameObject.tag;
        id = gameObject.GetInstanceID();
        transform = new SerializedTransform(gameObject.transform);
    }
}

public struct SerializedBoxCollider
{
    public Vector3 center;
    public Vector3 size;

    public SerializedBoxCollider(BoxCollider collider)
    {
        center = collider.transform.TransformPoint(collider.center);
        size = collider.transform.TransformVector(collider.size);
    }
}

/// <summary>
/// Attribute to select a single layer.
/// </summary>
public class LayerAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerAttributeEditor : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);
        int index = prop.intValue;
        if (index > 31)
        {
            Debug.Log("CustomPropertyDrawer, layer index is to high '" + index + "', is set to 31.");
            index = 31;
        }
        else if (index < 0)
        {
            Debug.Log("CustomPropertyDrawer, layer index is to low '" + index + "', is set to 0");
            index = 0;
        }
        prop.intValue = EditorGUI.LayerField(pos, label, index);
        EditorGUI.EndProperty();
    }
}
#endif