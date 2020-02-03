/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates an object which can be interacted with.
/// </summary>
public class Sensor : Stateful
{
    public List<Actuator> actuators = new List<Actuator>();
    public int MaxInteractCooldown = 20;

    private int CurrentInteractCooldown;

    // TODO: decouple trigger conditions from the sensor base class.
    public virtual void Update()
    {
        if(CurrentInteractCooldown > 0) CurrentInteractCooldown--;
    }

    // TODO: create an event system where actuators can substribe to events and sensors can emit events.
    /// <summary>
    /// Actuates the connected <see cref="Actuator"/> objects.
    /// </summary>
    public virtual void Trigger()
    {
        CurrentInteractCooldown = MaxInteractCooldown;
        isActive = !isActive;
        foreach (Actuator actuator in actuators)
            actuator.Actuate();
    }

    /// <summary>
    /// Activates the connected objects with optional data.
    /// </summary>
    /// <param name="o">A generic object that can be casted to something useful in an Actuator</param>
    public virtual void Trigger(object o)
    {
        CurrentInteractCooldown = MaxInteractCooldown;
        isActive = !isActive;
        foreach (Actuator actuator in actuators)
            actuator.Actuate(o);
    }

    /// <summary>
    /// Activates the connected objects with optional data.
    /// </summary>
    /// <param name="o">A generic object that can be casted to something useful in an Actuator</param>
    public virtual void Trigger(object o1, object o2)
    {
        CurrentInteractCooldown = MaxInteractCooldown;
        isActive = !isActive;
        foreach (Actuator actuator in actuators)
            actuator.Actuate(o1, o2);
        
    }

    public void AddActuator(Actuator a)
    {
        actuators.Add(a);
        connectedObjects.Add(a.name);
    }

    internal void RemoveActuator(Actuator a)
    {
        connectedObjects.Remove(a.name);
        actuators.Remove(a);
    }
}
