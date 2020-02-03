/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Entity,
    Dynamic,
    Interactive
}

// A basic representation of a game object to be used by the agent.
public class Entity
{
    public EntityType type = EntityType.Entity;
    public string tag;
    public string id;
    public Vector3 position;
    public string property = "";

    public Entity(GameObject context)
    {
        id = context.name;
        position = context.transform.position;
        tag = context.tag;

        if (tag.Equals("ColorScreen"))
        {
            Color c = context.GetComponent<ColorScreen>().GetColor();
            this.property = $"CS {c.r} {c.g} {c.b}";
        }    
    }

    public void AddProperty(string property)
    {
        this.property = property;
    }
}

// A representation of a dynamic entity: basically an entity with a velocity.
public class DynamicEntity : Entity
{
    public Vector3 velocity;

    public DynamicEntity(GameObject context) : base(context)
    {
        this.velocity = context.GetComponent<Rigidbody>().velocity;
    }
}

// An entity with a boolean state, and a bounding box for interactions
public class InteractiveEntity : Entity
{
    public bool isActive;
    public List<string> connectedObjects; // @refactor this into one time only
    public Vector3 center; // Bounding box position in world space
    public Vector3 extents; // Half box size 

    public InteractiveEntity(GameObject context, Stateful s) : base(context)
    {
        this.isActive = s.isActive;
        this.connectedObjects = s.connectedObjects;
        this.extents = s.interactiveBounds.extents;
        this.center = s.interactiveBounds.center;
        this.type = EntityType.Interactive;
    }
}
