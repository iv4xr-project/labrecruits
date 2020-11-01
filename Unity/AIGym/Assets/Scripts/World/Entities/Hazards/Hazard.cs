/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    Rigidbody rb;

    public virtual void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Handle hazard functionality
    /// </summary>
    /// <param name="delta">Time passed</param>
    public abstract void UpdateHazard();

    /// <summary>
    /// Duplicates and returns the attached gameobject
    /// </summary>
    /// <returns></returns>
    public GameObject Duplicate()
    {
        return Instantiate(gameObject, gameObject.transform.parent);
    }

    /// <summary>
    /// Move if allowed by sweeptest
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Move(Vector3 direction, float speed)
    {
        Vector3 movement = direction.normalized * speed;
        
        RaycastHit hit;
        if (!rb.SweepTest(direction, out hit, 0.02f) /*|| Passable(hit.collider.gameObject.tag)*/)
        {
            rb.MovePosition(transform.localPosition + movement);
        }
    }

    /// <summary>
    /// Interact with all buttons within the given radius
    /// </summary>
    public void Interact(float radius)
    {
#warning TODO: What is hazard interact?:
        /*
        Collider[] colliders = Physics.OverlapSphere(transform.localPosition, radius);
        foreach(Collider c in colliders)
        {
            // If c is a button, trigger it.
            c.gameObject.GetComponent<Button>()?.Trigger();
        }*/
    }
}
