/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

/// <summary>
/// Abstract class that implements interactions.
/// </summary>
public abstract class Actuator : Stateful
{
    /// Called when a sensor connected to this Actuator is actived.
    public virtual void Actuate()
    {
        isActive = !isActive;
    }

    /// <summary>
    /// Receive a parameter from a sensor when it is activated
    /// </summary>
    /// <param name="o">An object that can be cast to what this actuator expects</param>
    public virtual void Actuate(object o)
    {
        Actuate();
    }

    /// <summary>
    /// Receive a parameter from a sensor when it is activated
    /// </summary>
    /// <param name="o">An object that can be cast to what this actuator expects</param>
    public virtual void Actuate(object o1, object o2)
    {
        Actuate();
    }
}
