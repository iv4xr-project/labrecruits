/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Character class for character functionality.
/// </summary>
public class Character : MonoBehaviour
{
    public static EventHandler<GameObject> OnStartEvent;
    public static EventHandler<GameObject> OnDestroyEvent;

    public string agentID; // @Refactor, this should not be in Character, or it should be renamed
    
    public static float characterSpeed = 1f;
    public float vGravity = 2f;
    public static float jumpForce = 0.5f;

    public static float viewDistance = 7f;
    public float viewDistanceSqr => viewDistance * viewDistance;
    public float eyeHeight = 1.2f;
    public Vector3 relativeEyePosition;

    public bool IsMoving => Math.Abs(_moveDirection.x) > 0.1f || Math.Abs(_moveDirection.z) > 0.1f;
    public bool wasMoving;
    public Vector3 _moveDirection = Vector3.zero;

    public Transform model;

    private CharacterController _characterController;
    private Animator _animator;
    private Vector3? targetLocation = null;
    private bool circleLeft = false;

    public Color debugColor;

    public Transform _transform; //Cached transform, to prevent Unity safety code

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        relativeEyePosition = GetComponentInChildren<Camera>().transform.position - _transform.position;
    }

    void Start()
    {
        OnStartEvent?.Invoke(this, gameObject);
    }

    void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this, gameObject);
    }


    void Update()
    {
        _animator.SetBool("isMoving", wasMoving);
        // it can take aplib multiple updates to respond, so as long there is a targetLocation, the agent should animate (otherwise you will notice hickups in the animation)
        wasMoving = targetLocation.HasValue;
    }

    private void FixedUpdate()
    {    
        
        if (targetLocation.HasValue)
        {
            //check if we have already reached the target
            Vector3 diff = targetLocation.Value - _transform.position;

            if (diff.sqrMagnitude < 0.1)
            {
                targetLocation = null; // Target is reached
            } 
            else
            {
                // Important: Whiskers code depends on this being normalized, because it will add to the x and z.

                _moveDirection = new Vector3(diff.x, 0, diff.z).normalized;


                #region whiskers:
                World world = FindObjectOfType<World>().GetComponent<World>();

                float hazardRadius = 0.2f;
                float charRadius = 0.3f;
                float whiskersLength = 2f;
                float deathDist = hazardRadius + charRadius;
                float preferredDist = deathDist + whiskersLength;

                foreach (FireHazard hazard in world.GetHazards())
                {
                    Vector3 hazDiff = hazard.transform.position - transform.position;

                    if (hazDiff.magnitude < preferredDist)
                    {
                        Vector3 normalizedRunDirection = (-hazDiff).normalized;

                        float runMultiplier = 1f;
                        float clampDist = hazDiff.magnitude - deathDist;
                        runMultiplier /= Math.Max(clampDist * clampDist, 0.04f);

                        _moveDirection.x += normalizedRunDirection.x * runMultiplier;
                        _moveDirection.z += normalizedRunDirection.z * runMultiplier;

                        Vector3 circleAround = Quaternion.AngleAxis(circleLeft ? 90 : -90, Vector3.up) * normalizedRunDirection;

                        circleAround *= runMultiplier;

                        _moveDirection.x += circleAround.x;
                        _moveDirection.z += circleAround.z;
                    }
                }
                #endregion

                if (_moveDirection.x != 0 || _moveDirection.z != 0)
                    model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(_moveDirection, Vector3.up), 10f * Time.deltaTime);
            }
        }


        if (!_characterController.isGrounded)
            _moveDirection.y -= 1f;

        if (_characterController.isGrounded && _moveDirection.y <= 0f)
            _moveDirection.y = -.01f;

        _moveDirection.Normalize();

        _moveDirection *= characterSpeed;

        _characterController.Move(_moveDirection);
        _moveDirection.x = 0f;
        _moveDirection.z = 0f;
    }
    
    /// <summary>
    /// Starts moving the character in a given direction
    /// </summary>
    /// <param name="direction">The Vector3 direction to move towards</param>
    public void Move(Vector3 direction)
    {
        direction = direction.normalized;
        model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(direction, Vector3.up), 10f * Time.deltaTime);
        _moveDirection.x = characterSpeed * direction.x;
        _moveDirection.z = characterSpeed * direction.z;
        wasMoving = true;
    }

    public void SwitchCircleDirection()
    {
        this.circleLeft = !this.circleLeft;
    }

    /// <summary>
    /// This method will allow the agent to set a target location where the agent wants to move to, This will
    /// only move the agentin a straight line to the location.
    /// </summary>
    /// <param name="goal"></param>
    public void SetWantedTarget(Vector3 target)
    {
        targetLocation = target;
    }
    
    public void MoveJump()
    {
        if (_characterController.isGrounded)
            _moveDirection.y = jumpForce;
    }
    
    /// <summary>
    /// Rotates the character based on its current rotation.
    /// </summary>
    /// <param name="rotate_degrees">The rotation in degrees</param>
    public void Turn(float rotate_degrees)
    {
        _transform.eulerAngles += new Vector3(0, rotate_degrees, 0);
    }

    /// <summary>
    /// Returns the floor the agent is on, based on its Y coordinate.
    /// </summary>
    public int GetFloor()
    {
        return (int)Math.Round(_transform.position.y - 0.3); //@todo, what is 0.3 based on?
    }

    
    /// <summary>
    /// Interacts with a switch when in range.
    /// </summary>
    public void Interact()
    {
        GameObject[] switches = GameObject.FindGameObjectsWithTag("Switch");
        foreach (GameObject @switch in switches)
        {
            var sensor = @switch.GetComponent<Sensor>();
            if (!sensor.interactiveBounds.Contains(transform.position)) continue;
            sensor.Trigger();
            return;
        }
        
    }

    /// <summary>
    /// These methods should not be included in the binary
    /// </summary>
#if (UNITY_EDITOR) 
    public void OnDrawGizmos()
    {
        UnityEditor.Handles.color = new Color(1, 1, 1, 0.2f);
        UnityEditor.Handles.DrawWireDisc(_transform.position, Vector3.up, viewDistance); // Draw view distance circle
    }
#endif
}