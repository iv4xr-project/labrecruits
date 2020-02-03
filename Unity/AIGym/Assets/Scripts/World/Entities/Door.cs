/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using UnityEngine;

/// <summary>
/// Implementation of a door object.
/// </summary>
public class Door : Actuator
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Actuate()
    {
        base.Actuate();

        // Flip Opened animation parameter
        animator.SetBool("Opened", !animator.GetBool("Opened"));
        StartCoroutine(AnimatePosition(0.1f, 1f, animator.GetBool("Opened")));
    }

    /// <summary>
    /// Coroutine that moves the local x value of the child door
    /// We do this to re-adjust the location of the door during the animation so it will fit better.
    /// </summary>
    IEnumerator AnimatePosition(float distance, float time, bool open)
    {
        Transform child = transform.GetChild(0);
        float moved = 0f;
        float direction = open ? -1f : 1f;
        distance = Mathf.Abs(distance);
        while (moved < distance)
        {
            float step = (distance / time) * Time.deltaTime;
            if (moved + step >= distance)
            {
                child.localPosition += new Vector3((distance-moved)*direction, 0, 0);
                break;
            }
            child.localPosition += new Vector3(step*direction, 0, 0);
            moved += step;
            yield return null;
        }
    }
}
