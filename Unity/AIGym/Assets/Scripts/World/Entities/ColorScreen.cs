/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
/// <summary>
/// Adds input colors together and displays it on a panel
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class ColorScreen : MonoBehaviour
{
    private Material panelMaterial;
    private Dictionary<string, Color> mix; // Keep a list of colors that have been added. 

    //[JsonProperty] this BREAKS when the color is not straight red, green, or blue! Strange.
    public Color color { get; private set; } = Color.black;
    [JsonProperty] // using hand-crafted color-code instead:
    public ColorCode colorCode = new ColorCode(0,0,0) ;

    void Awake()
    {
        panelMaterial = transform.Find("Panel").GetComponent<MeshRenderer>().material;
        mix = new Dictionary<string, Color>();
        panelMaterial.color = color;
    }

    public void UpdateScreen(GameObject trigger)
        => UpdateScreen(trigger.GetComponent<Colorized>().GetColor(), trigger.name);

    /// <summary>
    /// Update the color mixture to include a new color
    /// </summary>
    public void UpdateScreen(Color color, string buttonID)
    {
        if (mix.ContainsKey(buttonID))
            mix.Remove(buttonID);
        else
            mix.Add(buttonID, color);

        this.color = Color.black;
        foreach (var k in mix.Keys)
            this.color += mix[k];
        colorCode = new ColorCode(this.color.r, this.color.g, this.color.b);
        panelMaterial.color = this.color;

        GetComponent<APLSynced>()?.APLSync();
    }

    public Color GetColor() 
        => color;
}
