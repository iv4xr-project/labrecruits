/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for interactive objects, which stores their (binary) state.
/// </summary>
public class Stateful : MonoBehaviour
{
    public bool isActive;
    public Bounds interactiveBounds; // Bounding box for allowing interactions.
    public List<string> connectedObjects; // Names of connected objects.

    private void Awake()
    {
        var box = GetComponent<BoxCollider>();
        if (box == null) box = GetComponentInChildren<BoxCollider>();
        interactiveBounds = box.bounds;
        interactiveBounds.Expand(1f); // The interaction range needs to be larger than the collison box.
        connectedObjects = new List<string>();
    }
}
