/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

/// <summary>
/// A collection of helper functions
/// </summary>
public class Utils
{
    /// <summary>
    /// Instantiate an object with a name;
    /// </summary>
    public static GameObject InstantiateEmpty(string name, Transform parent = null) {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        return go;
    }

    /// <summary>
    /// Returns this object, or the first parent object that contains a tag
    /// </summary>
    public static GameObject GetFirstObjectWithTag(GameObject go) {
        var goParent = go;
        var tag = go.tag;

        while (tag == "Untagged") {
            var t = goParent.transform.parent;
            if (t == null || t.gameObject.tag == "World") {
                Debug.Log($"Unable to find tag in parent tree of object '{go.name }'");
                goParent = go;
                break; //Reached too far in the scene tree, revert back to the base untagged object.
            }
            
            goParent = t.gameObject;
            tag = goParent.tag;
        }
        return goParent;
    }

    /// <summary>
    /// Convert a hexadecimal string to Color.
    /// </summary>
    /// <param name="hex">The hex color, with optional alpha</param>
    public static Color colorFromHexString(string hex)
    {
        var flag = System.Globalization.NumberStyles.HexNumber;
        byte r = byte.Parse(hex.Substring(0, 2), flag);
        byte g = byte.Parse(hex.Substring(2, 2), flag);
        byte b = byte.Parse(hex.Substring(4, 2), flag);
        byte a = (hex.Length != 8) ? (byte)255 : byte.Parse(hex.Substring(6, 2), flag);
        
        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// Load text file from Resources and copy the entire content
    /// </summary>
    /// <param name="filePath">Path relative from Resources folder, _without file extension_</param>
    public static string LoadText(string filePath)
    {
        var t = (Resources.Load(filePath, typeof(TextAsset)) as TextAsset);
        if (t != null)
            return t.text;

        if (!System.IO.File.Exists(filePath))
        {
            UserErrorInfo.ErrorWriter.AddMessage($"Could not load file at path:{filePath}");

            return null;
        }
     
        return System.IO.File.ReadAllText(filePath);
    }
}