/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

/// <summary>
/// Object which emits light
/// </summary>
public class Lamp : Actuator
{
    public Light lighting;

    void Awake()
    {
        lighting = transform.Find("Lamp").Find("Lightbulb").Find("Point Light").GetComponent<Light>();
        lighting.intensity = isActive ? 1 : 0; //Todo, add parameter to light in level file to set on/off at start
    }

    // Update is called once per frame
    public override void Actuate()
    {
        base.Actuate();
        lighting.intensity = isActive ? 1 : 0;
    }
}
