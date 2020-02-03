/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

/// <summary>
/// Implementation of an animated button.
/// </summary>
public class Button : Sensor
{
    protected Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Trigger()
    {
        base.Trigger();

        // Start the button press animation.
        animator.SetTrigger("Push");
    }

    new void Update()
    {
        base.Update();
    }
}