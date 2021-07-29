using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class NPCObserveableVariant : MonoBehaviour
{
    public bool RandomizeColorOnStart = false;

    [Serializable]
    public struct NPCColors
    {
        public Color Skin;
        public Color Eye;
        public Color Hair;
        public Color Shirt;
        public Color Pants;
        public Color Shoe;
    }

    [JsonProperty]
    [SerializeField]
    private NPCColors color;

    private Material material;
    private Texture2D texture;

    public NPCColors NPCColor
    {
        get => color;
        set
        {
            color = value;
            UpdateColor();
        } 
    }

    private void Start()
    {
        UpdateColor();

        if (RandomizeColorOnStart)
            RandomizeColor();
    }

    public void RandomizeColor()
    {
        color.Skin  = new Color32(0xff, 0xd1, 0x9f, 0xff);
        color.Eye   = new Color32(0x29, 0x29, 0x29, 0xff);
        color.Hair  = UnityEngine.Random.ColorHSV(0.083f, 0.167f, 0.000f, 0.333f);
        color.Shirt = UnityEngine.Random.ColorHSV(0, 1, 0.0f, 0.5f);
        color.Pants = UnityEngine.Random.ColorHSV();
        color.Shoe  = UnityEngine.Random.ColorHSV();

        UpdateColor();
    }

    private void SetUpMaterial()
    {
        if (material)
            return;

        var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        material = new Material(renderer.sharedMaterial);
        renderer.material = material;
    }

    public void UpdateColor()
    {
        SetUpMaterial();

        texture = new Texture2D(1, 32, TextureFormat.RGB24, false, true);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        var pixels = (new IEnumerable<Color>[] { 
            Enumerable.Repeat(color.Skin , 6),
            Enumerable.Repeat(color.Eye  , 5),
            Enumerable.Repeat(color.Hair , 6),
            Enumerable.Repeat(color.Shirt, 6),
            Enumerable.Repeat(color.Pants, 5),
            Enumerable.Repeat(color.Shoe , 4),
        }).SelectMany(s => s).Reverse().ToArray();
        texture.Apply();

        texture.SetPixels(pixels);

        material.mainTexture = texture;
    }
}

[CustomEditor(typeof(NPCObserveableVariant))]
public class NPCObserveableVariantEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var npc = (NPCObserveableVariant)target;
        var s_target = new SerializedObject(target);

        npc.RandomizeColorOnStart = EditorGUILayout.Toggle("Randomize color on start", npc.RandomizeColorOnStart);

        using (new EditorGUI.DisabledGroupScope(npc.RandomizeColorOnStart))
        {
            var color = s_target.FindProperty("color");
            EditorGUILayout.PropertyField(color, true);
            s_target.ApplyModifiedProperties();

            if (GUILayout.Button("Update"))
                npc.UpdateColor();

            if (GUILayout.Button("Randomize"))
                npc.RandomizeColor();
        }
    }
}
