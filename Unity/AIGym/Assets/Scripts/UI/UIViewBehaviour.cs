/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewBehaviour : MonoBehaviour
{
    public World _world;

    public GameObject gameController;
    public GameObject togglePrefab;

    // Keep track of our agent ui instances
    private readonly Dictionary<GameObject, GameObject> agents = new Dictionary<GameObject, GameObject>();
    
    void Start()
    {
        Character.OnStartEvent += AddEventHandler;
        Character.OnDestroyEvent += RemoveEventHandler;
    }

     void OnDestroy()
    {
        Character.OnStartEvent -= AddEventHandler;
        Character.OnDestroyEvent -= RemoveEventHandler;
    }

    /// <summary>
    /// Display a new agent in the UIView list
    /// </summary>
    private void AddEventHandler(object sender, GameObject agent)
    {
        // Create an UI instance and link it to the given agent object
        GameObject toggle = Instantiate(togglePrefab, gameObject.transform, true);
        UIToggleBehaviour toggleBehaviour = toggle.GetComponent<UIToggleBehaviour>();
        toggleBehaviour.cameraBehaviour = gameController.GetComponent<CameraBehaviour>();
        toggleBehaviour.character = agent.GetComponent<Character>();
        Text txt = toggle.GetComponentInChildren<Text>();
        txt.text = agent.name;
        // txt.fontSize = 24;
        // Keep track of the newly created UI instance
        agents.Add(agent, toggle);
    }

    /// <summary>
    /// Remove an excisting agent from the UIView list
    /// </summary>
    private void RemoveEventHandler(object sender, GameObject agent)
    {
        // Destroy the UI instance
        Destroy(agents[agent]);

        // Remove the agent reference
        agents.Remove(agent);
    }
}
