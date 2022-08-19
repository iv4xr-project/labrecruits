/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Collections;

public class AgentManager : MonoBehaviour, IEnumerable<Character>
{
    private SortedList<string, Character> agents = new SortedList<string, Character>(); //Sorted for printing in order
    private AgentInfoLogger logger = new AgentInfoLogger(); //set to null for no logging
    private static string indent = "  "; //string that represents a single indentation

    private void Update()
    {
        if (agents.Count == 0) return; //no use in logging just a timestamp

        logger?.Log($"Timestamp {DateTime.Now:HH:mm:ss:fff}");
        foreach(var agentPair in agents)
        {
            if(agentPair.Value == null) continue; //agent is destroyed
            logger?.Log($"{indent}Info about agent {agentPair.Key}");
            logger?.Log($"{indent}{indent}Location: {agentPair.Value.gameObject.transform.position}");
            logger?.Log($"{indent}{indent}Score: {agentPair.Value.Score}");
            logger?.Log($"{indent}{indent}Health: {agentPair.Value.Health}");
        }
    }

    public void AddAgent(Character agent)
    {
        agents.Add(agent.agentID, agent);
    }

    public void Reset()
    {
        logger?.Flush();
        agents = new SortedList<string, Character>();
    }

    public IEnumerator<Character> GetEnumerator()
    {
        return agents.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
