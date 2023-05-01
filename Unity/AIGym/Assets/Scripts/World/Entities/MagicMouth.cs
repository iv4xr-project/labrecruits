
using System;
using System.Linq;
using UnityEngine;

public class MagicMouth : MonoBehaviour
{
    private AgentManager agentManager;
    private Character.Mood mood;
    private float _triggerDistance = 4;
    private bool _isMagicMounth = false;
    
    void Start()
    {
        agentManager = GameObject.FindGameObjectWithTag("AgentManager").GetComponent<AgentManager>();

        string name = this.gameObject.name;
        if (name.StartsWith("R") || name.StartsWith("C"))
        {
            _isMagicMounth = true;
        }
        //Debug.Log((">>>> magic mouth " + name));
    }

    public void Update()
    {
        if (! _isMagicMounth) return;
        
        var closestAgent = agentManager.Where(a => a.IsAlive)
            .Select(a => new { agent = a, distance = Vector2.Distance(From3D(transform.position), From3D(a.transform.position)) })
            .OrderBy(a => a.distance).FirstOrDefault();

        if (closestAgent == null || closestAgent.distance > _triggerDistance)
        {
            return;
        }
        closestAgent.agent.SetMood("This is " + this.gameObject.name);
    }
    
    private Vector2 From3D(Vector3 p) => new Vector2(p.x, p.z); 

}
