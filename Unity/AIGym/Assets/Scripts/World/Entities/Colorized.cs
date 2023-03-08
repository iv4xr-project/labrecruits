/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A button with a color 
/// </summary>
[RequireComponent(typeof(Interactable))]
[JsonObject(MemberSerialization.OptIn)]
public class Colorized : MonoBehaviour
{
    // [JsonProperty] this BREAKS when the color is not straight red, green, or blue! Strange.
    private Color color;
    [JsonProperty] // using hand-crafted color-code instead:
    public ColorCode colorCode ;

    public void SetColor (Color c)
    {
        this.color = c;
        colorCode = new ColorCode(c.r, c.g, c.b);
        transform.Find("Button").GetComponent<MeshRenderer>().material.color = c;
        //Debug.Log(">>> setting color :" + c);
    }

    public Color GetColor()
    {
        return color;
    }
}
