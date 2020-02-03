/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of an animated lever.
/// </summary>
public class Lever : Sensor
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Trigger()
    {
        base.Trigger();

        // Toggle between up and down.
        animator.SetBool("Up", !animator.GetBool("Up"));
    }

    new void Update()
    {
        base.Update();
    }
}
