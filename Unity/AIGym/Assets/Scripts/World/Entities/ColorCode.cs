using System;

/// <summary>
/// For representing color in RGB. We use this class because it can be serialized
/// to JSON. The default class Color gives an error when serialzed, when the color
/// is other than straight red, gree, or blue.
/// </summary>
public class ColorCode
{
    public float r;
    public float g;
    public float b;

    public ColorCode(float r, float g, float b)
    {
        this.r = r; this.g = g; this.b = b;
    }
}
