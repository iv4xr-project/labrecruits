/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine.Events;
using System;

/* Welcome to Lab Recruits, an agent testing facility for Aplib agents.
 * 
 * The project consists of a few entities: Lab, Connection and (external) Agents.
 * An external agent application launches the Unity application configures the 
 * environment through a socket connection. Agents can then send actions, which 
 * the Lab will respond to by applying the actions to characters, and returning 
 * a new environment state. For more information on these entities, and setting 
 * up a project, please browse the documentation on: 
 * https://git.science.uu.nl/muscleai/aigym-iv4xr-aplib/-/wikis/home
*/

/// <summary>
/// A Lab is responsible for controlling requests from the agent interface.
/// </summary>
public class Lab : MonoBehaviour
{
    public bool Playing { private set; get; }
    
    [Serializable]
    public class SendEvent : UnityEvent<Socket, Message> { }
    public SendEvent OnSend;

    public List<Socket> clients = new List<Socket>(); // Keep track of all clients.
    private Queue<(Socket, string)> messages = new Queue<(Socket, string)>();

    private World _world;
    private Dictionary<string, AgentController> _agentControllers;
    private Character[] _characters;
    private NavMeshContainer _nav;

    private Delayer delayer; // Delays the execution of a function for a number of frames

    private int gameTick;

    bool reload_level = false;
    public EnvironmentConfig config = new EnvironmentConfig();
    
    void Start()
    {
        _world = FindObjectOfType<World>().GetComponent<World>();
        delayer = gameObject.AddComponent<Delayer>();

        ReadLevelFromArgs(); 
        StartNewLevel();
    }

    /// <summary>
    /// Process a message from the agent application.
    /// </summary>
    /// <remarks>
    /// The agent application may either send application requests, or agent 
    /// requests. The lab will defer agent request to the corresponding agent 
    /// controller. A request will always trigger the lab to respond with an 
    /// answer.
    /// </remarks>
    public void ProcessMessage(Socket client, string message)
    {
        Message msg = Message.CreateFrom(message);

        switch (msg.cmd)
        {
            case RequestType.DISCONNECT:
                var disconnectRequest = RawRequest<bool>.CreateFrom(message);
                UserErrorInfo.ErrorWriter.AddMessage("Client requested a DISCONNECT");
                Respond(client, disconnectRequest, true);
                break;

            case RequestType.PAUSE:
                var pauseRequest = RawRequest<bool>.CreateFrom(message);
                Pause();
                Respond(client, pauseRequest, true);
                break;

            case RequestType.START:
                var startRequest = RawRequest<object, bool>.CreateFrom(message);
                Play();
                Respond(client, startRequest, true);
                break;

            case RequestType.INIT:
                var initRequest = Request<EnvironmentConfig, NavMeshContainer>.CreateFrom(message);
                InitiateNewLevel(initRequest.arg);
                // because of deleting and generating a new navmesh, delay the response by 3 game updates
                delayer.Add(3, () => Respond(client, initRequest, _nav));
                break;

            case RequestType.AGENTCOMMAND:
                var agentRequest = Request<Command, Observation>.CreateFrom(message);
                if (_agentControllers.ContainsKey(agentRequest.arg.agentId))
                {
                    AgentController agentController = _agentControllers[agentRequest.arg.agentId];
                    Observation obs = agentController.ProcessCommand(agentRequest.arg, gameTick, _nav);
                    Respond(client, agentRequest, obs);
                }
                else
                    UserErrorInfo.ErrorWriter.AddMessage("Agent with ID '" + agentRequest.arg.agentId + "' does not exist");
                break;
            default:
                throw new Exception("The incoming message could not be handled!\n" + message);
        }
    }

    //@Todo, remove after implementing our own navmesh!
    public void Update()
    {
        gameTick++;

        if (reload_level)
        {
            delayer.Add(1, StartNewLevel);
            reload_level = false;
        }   
    }

    /// <summary>
    /// Respond to APlib using the socketserver (via the OnSend event)
    /// </summary>
    /// <param name="client">The client socket</param>
    /// <param name="request">The original request</param>
    /// <param name="response">The response</param>
    private void Respond<Argument, Response>(Socket client, RawRequest<Argument, Response> request, Response response) => OnSend.Invoke(client, request.WithResponse(response));

    /// <summary>
    /// Stop processing incoming messages. This keeps all clients waiting for their responses.
    /// </summary>
    public void Pause()
    {
        Playing = false;
    }

    /// <summary>
    /// Add a client to the clients list and send over the initial state.
    /// </summary>
    public void AddClient(Socket client)
    {
        // Keep track of all connected clients.
        clients.Add(client);
#if UNITY_EDITOR
        Debug.Log("A client has connected on " + client.RemoteEndPoint.ToString());
#endif
    }

    /// <summary>
    /// Remove a client from the clients list.
    /// </summary>
    public void RemoveClient(Socket client)
    {
        clients.Remove(client);
#if UNITY_EDITOR
        Debug.Log("A client has disconnected");
#endif
    }

    /// <summary>
    /// Allows the level file to be set from the command line
    /// </summary>
    public void ReadLevelFromArgs()
    {
        foreach (string arg in Environment.GetCommandLineArgs())
        {
            if (!arg.Contains("-level")) continue;

            string[] split = arg.Split(new char[] { '=' });

            if (split.Length != 2) throw new ArgumentException("Optional -level requires a valid path specification, like: -level=Assets/Levels/level1.csv");
            config.level_path = split[1];

            return;
        }
    }

    /// <summary>
    /// Loading a new level will happen after 1 frame delay, to make the navmesh work properly. 
    /// </summary>
    // @Todo, this can be cleared up after having our own navmesh!
    public void InitiateNewLevel(EnvironmentConfig config)
    {
        reload_level = true;

        this.config = config;

        _world.ClearWorld();
        _world.transform.GetComponent<NavMeshHelper>().ClearMesh();

        UserErrorInfo.ErrorWriter.ClearMessages();
    }

    /// <summary>
    /// Load the specified level and set up some things for the environment.
    /// </summary>
    private void StartNewLevel()
    {
        _world.LoadLevel(config);

        _characters = FindObjectsOfType<Character>();
        _agentControllers = new Dictionary<string, AgentController>();

        foreach (var c in _characters)
        {
            c.debugColor = new Color(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), 0.3f);
            _agentControllers.Add(c.agentID, new AgentController(c, new Observation(c.agentID)));
        }

        _nav = _world.transform.GetComponent<NavMeshHelper>().GenerateNavMesh();

        if (_characters.Length > 0)
            GetComponent<CameraBehaviour>().AttachToCharacter(_characters[0]);

        
    }

    /// <summary>
    /// Resume/Start processing incoming messages.
    /// </summary>
    public void Play()
    {
        Playing = true;

        // Process all messages that have been received while being paused.
        while (messages.Count > 0)
        {
            (Socket client, string message) = messages.Dequeue();
            ProcessMessage(client, message);
        }
    }

    /// <summary>
    /// Draw debug cubes for the navmesh nodes that agents can see
    /// </summary>
    public void OnDrawGizmos()
    {
        if (_agentControllers == null) return;

        foreach (var k in _agentControllers.Keys)
        {
            var agent = _agentControllers[k];
            Gizmos.color = agent._Character.debugColor;

            var indices = agent.observation.navMeshIndices;
            if (indices == null) continue;
            for (var i = 0; i < indices.Length; i++)
                Gizmos.DrawCube(_nav.vertices[indices[i]], Constants.CubeSizeSmall);
        }
    }
}
