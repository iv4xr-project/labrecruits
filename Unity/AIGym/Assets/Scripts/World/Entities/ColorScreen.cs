/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary>
/// Adds input colors together and displays it on a panel
/// </summary>
public class ColorScreen : Actuator
{
    private Material panelMaterial;
    private Dictionary<string, Color> mix; // Keep a list of colors that have been added. 

    void Awake()
    {
        panelMaterial = transform.Find("Panel").GetComponent<MeshRenderer>().material;
        mix = new Dictionary<string, Color>();
        if (!isActive) panelMaterial.color = Color.black;
    }

    /// <summary>
    /// Update the color mixture to include a new color
    /// </summary>
    public override void Actuate(object o1, object o2)
    {
        base.Actuate(o1, o2);

        Color color = (Color)o1;
        string buttonID = (string)o2;

        if (mix.ContainsKey(buttonID)) mix.Remove(buttonID);
        else                           mix.Add(buttonID, color);
        
        Vector3 a = Vector3.zero;
        foreach (var k in mix.Keys)
        {
            var c = mix[k];
            a.x += c.r;
            a.y += c.g;
            a.z += c.b;
        }

        panelMaterial.color = new Color(Mathf.Min(a.x,1), Mathf.Min(a.y,1), Mathf.Min(a.z, 1));
    }

    public Color GetColor()
    {
        return panelMaterial.color;
    }
}
