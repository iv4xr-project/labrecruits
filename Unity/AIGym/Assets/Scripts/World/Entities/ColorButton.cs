/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

/// <summary>
/// A button with a color 
/// </summary>
public class ColorButton : Button
{
    private Color color;

    public void SetColor (Color c)
    {
        this.color = c;
        transform.Find("Button").GetComponent<MeshRenderer>().material.color = c;
    }

    public Color GetColor()
    {
        return color;
    }

    /// <summary>
    /// Pass the color component to the actuators that listen to this.
    /// </summary>
    public override void Trigger()
    {
        animator.SetTrigger("Push");
        base.Trigger(color, name);
    }
}
