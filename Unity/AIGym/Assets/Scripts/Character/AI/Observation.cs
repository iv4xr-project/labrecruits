/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course ©Copyright
Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Collects the observed gamestate from an agent's character, to be sent to an external
/// agent that requests it.
/// </summary>

public struct SerializedAgent
{
    public string id;
    public Vector3 position;
    public Vector3 velocity;
    public bool didNothing;
    public int health;
    public int score;
    public string mood;
}

public struct SerializedMeta
{
    public int tick;
    public DateTime time;
}

public class Observation : IAPLSerializable
{
    public SerializedMeta meta;
    public SerializedAgent agent;
    public HashSet<APLSynced> objects = new HashSet<APLSynced>();

    public int[] navMeshIndices;

    private static readonly HashSet<string> ignored = new HashSet<string> { "Wall", "Floor", "Player", "Wire"};

    public Observation(string agentID) {
        agent.id = agentID;
    }

    /// <summary>
    /// Records the current state of the world from the perspective of a Character
    /// </summary>
    public void Observe(Character character, int gameTick, NavMeshContainer nav, AgentCommandType usedAction) {
        objects.Clear();

        meta.tick = gameTick;
        meta.time = DateTime.Now;

        agent.position = character.transform.position;
        agent.didNothing = usedAction == AgentCommandType.DONOTHING;
        agent.health = character.Health;
        agent.score = character.Score;
        agent.mood = character.GetMood().value;
        
        AddAllVisibleObjects(character);

        navMeshIndices = GetAllVisibleVertexIndices(character, nav);
    }

    public static Observation FromCharacter(string agentID, Character character, int gameTick, NavMeshContainer nav, AgentCommandType usedAction)
    {
        Observation obs = new Observation(agentID);
        obs.Observe(character, gameTick, nav, usedAction);
        return obs;
    }

    /// <summary>
    /// Returns the indices of vertices that are visible from a character's eye height. 
    /// </summary>
    /// <param name="character">The character as a reference point</param>
    public int[] GetAllVisibleVertexIndices(Character character, NavMeshContainer nav) {
        Vector3 eyePosition = agent.position + character.relativeEyePosition;
        var colliders = GetNearbyBoundsColliders(eyePosition, character.viewDistanceSqr);
        
        List<int> visibleVertexIndices = new List<int>();
        for (int i = 0; i < nav.vertices.Length; i++) {
            Vector3 v = nav.vertices[i];
            // This will cause side effect!!
            // v.y += Constants.epsilon;
            Vector3 diff = (v - eyePosition) + new Vector3(0, Constants.epsilon, 0);
            float distance = diff.magnitude;

            //Debug.Log(">>> vertex: " + v);

            if (distance >= Character.viewDistance)     continue; // If the vertex is outside the character's vision radius
            // If the vertex is covered by a collider (door or colorscreen), it is not visible.
            // By "covered" it means if e.g. it is inside/engulfed by a collider, so a agent can't possibly
            // move to it anyway:
            if (colliders.Any(c => c.Contains(v)))                  continue; 
            // If the vertex is behind a wall/floor/wall/door, it is not visible:
            if (Physics.Raycast(eyePosition, diff, distance, (1 << 9) | (1 << 10) | (1 << 11))) continue; 

            //Debug.Log(">>> " + v + " is visible");
            visibleVertexIndices.Add(i);
        }

        return visibleVertexIndices.ToArray();
    }

    /// <summary>
    /// Return the bounds of nearby Colliders, for the purpose of identifying 
    /// navigation nodes which are not covered (being inside) by these colliders.
    /// </summary>
    /// <param name="eyePosition">The position of the character</param>
    /// <param name="viewDistanceSqr">The squared distance the character can see</param>
    /// <returns>A list of colliders that are in range</returns>
    private List<Bounds> GetNearbyBoundsColliders(Vector3 eyePosition, float viewDistanceSqr) {

        // Note first that we can ignore walls and floors for this calculation because the nav-mesh
        // is generated as such that they won't be covered by floors and walls.
        // Also note that we have marked "Decorations" objects as "wall".

        // Current there are only two types of game objects that can cover/engulf a nav-node,
        // namely: door and colorscreen. Note that an open-door might by chance cover a nav-node
        // that is directly underneath it.

        // @Incomplete, compute these references once. Also, maybe there is a better way to capture all Colliders that can block parts of the navmesh.
        // var a = GameObject.FindObjectsOfType<Door>();
        // var b = GameObject.FindGameObjectsWithTag("Dynamic").Union(GameObject.FindGameObjectsWithTag("Decoration")).ToArray();
        var b = GameObject.FindGameObjectsWithTag("Door").Union(GameObject.FindGameObjectsWithTag("ColorScreen")).ToArray();

        var colliders = new List<Bounds>();
#warning TODO: Fix door collider observation:
        /*
        for (int i = 0; i < a.Length; i++) {
            var o = a[i].GetComponentInChildren<Collider>();
            if (o.enabled && (o.transform.position - eyePosition).sqrMagnitude <= viewDistanceSqr)
                colliders.Add(o.bounds);
        }*/

        for (int i = 0; i < b.Length; i++) {
            var o = b[i].GetComponent<Collider>();
            // WP: adding a guard o != null if b[i] turns out not to be a collider
            if (o!= null && o.enabled && (o.transform.position - eyePosition).sqrMagnitude <= viewDistanceSqr)
                colliders.Add(o.bounds);
        }

        return colliders;
    }

    /// <summary>
    /// Adds all visible GameObjects in the <see cref="Character"/>'s vision to the approriate entity list.
    /// </summary>
    /// <param name="radius">The radius of the vision the <see cref="Character"/> has.</param>
    /// <returns>A <see cref="List{GameObject}"/> of all visible <see cref="GameObject"/>s.</returns>
    public void AddAllVisibleObjects(Character character) {
        Vector3 eye = agent.position + character.relativeEyePosition; //Translates player position to player view.
        Collider[] hitColliders = Physics.OverlapSphere(eye, Character.viewDistance); // @Todo, see if we can use layer mask to strip out walls/floors

        foreach (Collider hitCollider in hitColliders) {
            GameObject o = Utils.GetFirstObjectWithTag(hitCollider.gameObject);
            
            if (ignored.Contains(o.tag))          continue; // Object needs to be interesting
            if (!IsObjectVisibleToPlayer(o, eye)) continue; // Object can't be blocked

            // Get relevant APLSynced scripts
            var synced = new List<APLSynced>();
            o.GetComponentsInParent(false, synced); 
            o.GetComponentsInChildren(false, synced);
            objects.UnionWith(synced);
        }
    }

    /// <summary>
    /// Checks if a ray between the character and an object is unobstructed
    /// </summary>
    /// <param name="o"></param>
    /// <param name="eyePosition"></param>
    public bool IsObjectVisibleToPlayer(GameObject o, Vector3 eyePosition)
    {
        o.transform.TryGetComponent(out BoxCollider b);
        if (b == null) b = o.transform.GetComponentInChildren<BoxCollider>();

        // @Todo: Shoot multiple rays from object to the eye in order for more precision.
        // Note: we elevate the position of o a bit, to make it a bit easier for the external
        // agent to spot it.
        Vector3 startPosition = b.bounds.center + new Vector3(0,0.2f,0); // Position of object boxcollider; b.center * b.transform.localScale.x; // + Vector3.up; (1,0,1) + modeloffset 
        Vector3 toEye = eyePosition - startPosition; //Calculate the direction of the character.

        if (Physics.Raycast(startPosition, toEye.normalized, out RaycastHit hit, toEye.magnitude))
        {
            GameObject otherObject = Utils.GetFirstObjectWithTag(hit.collider.gameObject);

            if (otherObject.tag == "Player" && otherObject.name == "Agent " + agent.id)
            {
                Debug.Log(">>> can see obj-name:" + otherObject.name + ", agent.id: " + agent.id);
                return true;  // Player can see object
            }
            if (otherObject.GetInstanceID() != o.GetInstanceID()) return false; // Vision is blocked by another object
        }
        

        //Shouldn't happen, this means we couldn't find the character model
#if false
        Debug.Log("Could not solve ray for:" + o.name);
        Debug.Log("original location: " + b.transform.position);
        Debug.Log("Box offset       : " + box_offset.ToString("F4"));
        Debug.Log("Eye    location  : " + eyePosition);
        Debug.Log("Final start loc  : " + startPosition);
#endif
        return false; //unnecessary later
    }

    /// <summary>
    /// Serialize this entire object into a Json string, making it ready to be sent.
    /// </summary>
    public JObject APLSerialize() {
        var objectsList = objects.Select(x => x.APLSerialize()).ToList();
        return JObject.FromObject(new {
            meta,
            agent,
            objectsList,
        });
    }
}   