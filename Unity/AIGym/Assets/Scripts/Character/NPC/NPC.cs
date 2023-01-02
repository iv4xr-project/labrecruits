/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public float waitTime;
    public static float walkingSpeed;
    public float walkingDistance;

    private float _timer;
    private Animator _animator;
    private NavMeshAgent _agent;
    private CameraBehaviour _cameraBehavior;
    public Transform _transform; //Cached transform, to prevent Unity safety code

    public List<Vector3> points;
    private int destPoint = 0;

    private void Awake()
    {
        _transform = transform;
        _cameraBehavior = FindObjectOfType<CameraBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    public void Update()
    {
        _animator.SetBool("isMoving", _agent.velocity.magnitude > 0.2f);
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }

    private void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Count == 0)
            return;

        // Sometimes the CameraBehavior class cannot make an NPC that cross floors visible
        // when it enters the same floor as the current camera floor (due to the incomplete
        // logic there). This should force the monster to become visible:
        Utils.SetVisibility(this.gameObject, this.GetFloor() <= _cameraBehavior.cameraFloor);    

        // Set the agent to go to the currently selected destination.
        _agent.destination = points[destPoint];

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Count;
    }

    /// <summary>
    /// Returns the floor the agent is on, based on its Y coordinate.
    /// </summary>
    public int GetFloor()
    {
        return (int) Math.Round(this._transform.position.y - 0.3); //@todo, what is 0.3 based on?
    }
}
