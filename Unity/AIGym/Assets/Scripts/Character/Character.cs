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

    public AudioClip tension;
    public AudioClip death;

    public string agentID; // @Refactor, this should not be in Character, or it should be renamed
    
    public static float characterSpeed = 1f;
    public float vGravity = 2f;
    public static float jumpForce = 0.5f;

    public static float viewDistance = 7f;
    public float viewDistanceSqr => viewDistance * viewDistance;
    public float eyeHeight = 1.2f;
    public Vector3 relativeEyePosition;

    public bool IsAlive => _health > 0;
    public bool IsMoving => Math.Abs(_moveDirection.x) > 0.1f || Math.Abs(_moveDirection.z) > 0.1f;
    public bool wasMoving;
    public Vector3 _moveDirection = Vector3.zero;

    public int maxHealth = 100;
   
    public Transform model;

    private CharacterController _characterController;
    private Animator _animator;
    private Vector3? targetLocation = null;
    private bool circleLeft = false;
    private Mood _mood;
    private int _health;
    private int _score = 0 ;

    public HashSet<string> _visitedPointWorthObjects = new HashSet<string>();
    private HashSet<string> _turnedOnSwitches = new HashSet<string>();

    public Color debugColor;

    public Transform _transform; //Cached transform, to prevent Unity safety code

    public struct Mood
    {
        public DateTime lastSet;
        public string value;
    }

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        relativeEyePosition = GetComponentInChildren<Camera>().transform.position - _transform.position;

        //GetComponent<Renderer>().material.SetColor("Pink", new Color(1, 1, 0));
    }

    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            string[] moods =
            {
                "Ouch",
                ":(",
                "That hurts!"
            };
            SetMood(moods[UnityEngine.Random.Range(0, moods.Length)]);
            AudioSource sound = this.gameObject.GetComponent<AudioSource>();
            sound.clip = null;
            if (Health < 30 && Health > 0 && Health % 10 == 0)
            {
                sound.clip = tension;
            }
            else if (Health <= 0)
            {
                UserErrorInfo.ErrorWriter.AddMessage($"Agent {agentID} has died.", true, ErrorType.General);
                sound.clip = death;
            }
            if (sound.clip != null) sound.Play(0);
        }
    }

    public int Score
    {
        get { return _score;  }
        set
        {
            _score = value;
        }
    }

    void Start()
    {
        OnStartEvent?.Invoke(this, gameObject);

        _mood = new Mood
        {
            lastSet = DateTime.Now,
            value = "",
        };
        _health = maxHealth;
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

        if (!IsAlive) return; // otherwise agent-controlled character can still moving
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

                bool enableFireAvoidance = false;
                // disabling the fire-avoidance below, as it conflicts with the way java-agent wants to
                // cobntrol
                if(enableFireAvoidance)
                {
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
                }
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
        if (!IsAlive) return;
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
        if (_characterController.isGrounded && IsAlive)
            _moveDirection.y = jumpForce;
    }
    
    /// <summary>
    /// Rotates the character based on its current rotation.
    /// </summary>
    /// <param name="rotate_degrees">The rotation in degrees</param>
    public void Turn(float rotate_degrees)
    {
        if (!IsAlive) return;
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
    /// Interacts with a switch when in range. Note that the logic below will interact with the FIRST
    /// switch found within the interaction distance, if there is any. The candidates are note sorted
    /// by distance, so make sure that the switches are not placed too close to each other.
    /// </summary>
    public void Interact()
    {
        if (!IsAlive) return;
        Debug.Log("Trying to interact...");
        GameObject[] switches = GameObject.FindGameObjectsWithTag("Switch");
        foreach (GameObject @switch in switches)
        {
            var sensor = @switch.GetComponent<Interactable>();
            if (!sensor.interactable
                || Vector3.Distance(sensor.transform.position, this.transform.position) > Constants.interactionDistance)
                continue;
            Debug.Log($"Interacting with: {sensor.name}, distance {Vector3.Distance(sensor.transform.position, this.transform.position)}");
            sensor.Interact(this);
            return;
        }
        
    }

    /// <summary>
    /// Set the mood of the character to a given string.
    /// </summary>
    /// <param name="value"></param>
    public void SetMood(string value)
    {
        _mood.value = value;
        _mood.lastSet = DateTime.Now;
    }

    /// <summary>
    /// Returns the current mood.   
    /// </summary>
    /// <returns></returns>
    public Mood GetMood() => _mood;

    /*
     * Check if the game-object with the given name (assumed unique) has been visited by this
     * character.
     */ 
    public bool hasBeenVisited(string gameObjectName)
    {
        return _visitedPointWorthObjects.Contains(gameObjectName) ;
    }

    /*
     * Check if the switch with the specified name has received score-bonus for being turned-on. This bonus
     * can only be received once.
     */ 
    public bool hasReceivedTurnedOnBonus(string switchName)
    {
        return _turnedOnSwitches.Contains(switchName);
    }

    public void registerVisitedGameObject(string gobjName)
    {
        //Debug.Log(">>> " + gobjName + " registered: " + hasBeenVisited(gobjName)) ;
        _visitedPointWorthObjects.Add(gobjName);
        //Debug.Log(">>> " + gobjName + " registered: " + hasBeenVisited(gobjName)) ;
    }

    public void registerVisitedTurnedOnSwitch(string switchName)
    {
        _turnedOnSwitches.Add(switchName);
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