/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public class Enemy : IHasMood
{
    public float waitTime;
    public static float walkingSpeed;
    public float walkingDistance;
    public float visionDistance;
    public float shortCooldown = 2;
    public int attacksBeforeLongCooldown = 3;
    public float longCooldown = 10;

    private float _timer = 0;
    private int attacks = 0;
    private Animator _animator;
    private NavMeshAgent _agent;
    
    private AgentManager agentManager;
    private Character.Mood mood;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        agentManager = GameObject.FindGameObjectWithTag("AgentManager").GetComponent<AgentManager>();
    }

    // Update is called once per frame
    public void Update()
    {
        _animator.SetBool("isMoving", _agent.velocity.magnitude > 0.2f);

        _timer -= Time.deltaTime;
        if (_timer > 0) return;

        if (!_agent.pathPending && agentManager != null)
            GotoClosestPlayer();
    }

    private void GotoClosestPlayer()
    {
        var closestAgent = agentManager.Where(a => a.IsAlive).Select(a => new { agent = a, distance = Vector2.Distance(From3D(transform.position), From3D(a.transform.position)) })
                                       .OrderBy(a => a.distance).FirstOrDefault();

        if (closestAgent == null || closestAgent.distance > visionDistance)
        {
            _agent.destination = transform.position;
            return;
        }

        if (_agent.destination == transform.position)
        {
            mood.value = "Heal Me..."; //TODO IMood: GetMood, add MoodBubble
            mood.lastSet = DateTime.Now;
        }

        if (closestAgent.distance < 1f)
            DoDamage(closestAgent.agent);
        else
            _agent.destination = closestAgent.agent.transform.position;
            
    }

    private Vector2 From3D(Vector3 p) => new Vector2(p.x, p.z); 

    private void DoDamage(Character toAgent)
    {
        toAgent.Health -= 10;
        _agent.destination = transform.position;
        attacks++;
        if (attacks >= attacksBeforeLongCooldown)
        {
            _timer = longCooldown;
            attacks = 0;
        }
        else _timer = shortCooldown;

        AudioSource sound = this.gameObject.GetComponent<AudioSource>();
        if (toAgent.Health > 30)
            sound.Play(0); //sound effect from https://freesound.org/people/Breviceps/sounds/445983/
    }

    public override Character.Mood GetMood() => mood;
}
