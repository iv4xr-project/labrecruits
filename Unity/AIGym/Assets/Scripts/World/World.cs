/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// World builds a level from a given CSV file. 
/// </summary>
/// <remarks>
/// The World defines what objects correspond to a symbol in a level file, and
/// their behaviour. If you want to create your own level, please see the 
/// specifications of a level definition file at:
/// https://git.science.uu.nl/muscleai/aigym-iv4xr-aplib/-/wikis/Defining-a-level
/// </remarks>
public class World : MonoBehaviour
{
    public bool wireBreaking = false;
    public UIViewBehaviour _UIViewBehaviour;

    public enum EntityGroup { Fixed, Interactives, Agents, Dynamic }

    [Serializable]
    public struct Placeable
    {
        public string symbol;
        public EntityGroup group;
        public GameObject[] prefabs;
    }

    public Placeable[] placeables;
    
    private readonly Dictionary<string, Placeable> placeablesMap = new Dictionary<string, Placeable>();
    private Dictionary<EntityGroup, GameObject> sceneHierarchyNodes;

    private HashSet<string> agentIDs = new HashSet<string>();

    private readonly static char ID_SEPERATOR = '^';
    private readonly static char ROT_SEPERATOR = '>';
    private readonly static char COL_SEPERATOR = '#';
    
    private WireBuilder _wireBuilder;

    private void Awake()
    {
        foreach (Placeable placeable in placeables)
            placeablesMap[placeable.symbol] = placeable;

        _wireBuilder = GetComponent<WireBuilder>();
    }

    /// <summary>
    /// Call UpdateHazard for every dynamic object in the dynamic node of the scene hierarchy
    /// </summary>
    /// <param name="delta">Time delta</param>
    public void UpdateHazards()
    {
        foreach (Transform t in sceneHierarchyNodes[EntityGroup.Dynamic].transform)
        {
            t.gameObject.GetComponent<Hazard>().UpdateHazard();
        }
    }

    /// <summary>
    /// Spawn prefabs from the placeables array according to the layout provided by level file.
    /// </summary>
    /// <param name="path">Path to CSV file in the Resources folder</param>
    public void LoadLevel(EnvironmentConfig config)
    {
        PreBuildConfiguration(config);
        BuildLevel(config.level_path);
        PostBuildConfiguration(config);
    }

    private void PreBuildConfiguration(EnvironmentConfig config)
    {
        // seed
        Random.InitState(config.seed);

        // movement
        Character.characterSpeed = config.agent_speed;
        Character.jumpForce = config.jump_force;
        Character.viewDistance = config.view_distance;
        NPC.walkingSpeed = config.npc_speed;

        FireHazard.SPREAD_SPEED = config.fire_spread;
        // setting the light intensity here
        RenderSettings.ambientIntensity = config.light_intensity;
    }

    private void BuildLevel(string path)
    {
        Debug.Log($"Loading level: {path}");
        if (path == "")
            return;

        UserErrorInfo.ErrorWriter.AddMessage($"Loading level '{Path.GetFileNameWithoutExtension(path)}'", false);
        if (transform.childCount > 0)
            ClearWorld();

        // Extract the objects from the text file
        var text = Utils.LoadText(path);
        if (text == null)
        {
            UserErrorInfo.ErrorWriter.AddMessage($"Level not found: {path}");
            return;
        }

        string[] sheets = SplitSheets(text);
        Debug.Log($"Found {sheets.Length} sheets.");
        bool hasLinks = sheets.Length > 1 && IsNoFloor(sheets[0]);

        List<List<(string, Vector3, EntityGroup)>> floors = (hasLinks) ?
            ExtractSpawnableObjects(sheets.Skip(1).ToArray()) :
            ExtractSpawnableObjects(sheets.ToArray());


        // Spawn the objects in the correct position and organize them in the scene hierarchy
        sceneHierarchyNodes = new Dictionary<EntityGroup, GameObject> {
            { EntityGroup.Agents, Utils.InstantiateEmpty("Agents", transform) },
            { EntityGroup.Dynamic, Utils.InstantiateEmpty("Dynamic", transform) }
        };

        for (int i = 0; i < floors.Count; i++)
        {
            AddNewFloorInSceneHierarchy(i);
            foreach (var o in floors[i])
                PlaceObject(o.Item1, new Vector3(o.Item2.x, i, o.Item2.z), sceneHierarchyNodes[o.Item3].transform);
        }

        if (hasLinks)
            ConnectLinks(sheets[0]);

        UserErrorInfo.ErrorWriter.AddMessage("Finished loading level", false);
        UserErrorInfo.ErrorWriter.FlushErrorLog();
    }

    private void PostBuildConfiguration(EnvironmentConfig config)
    {
#warning TODO: Do we still need post build?
        // lamp brightness
        /*
        foreach (var lamp in FindObjectsOfType<Lamp>())
            lamp.lighting.intensity = config.light_intensity;
        // remove links
        foreach (var a in config.remove_links)
        {
            var sensor = GameObject.Find(a.object1).GetComponent<Sensor>();
            var actuator = GameObject.Find(a.object2).GetComponent<Actuator>();
            if (sensor != null && actuator != null)
                sensor.RemoveActuator(actuator);
        }

        // add links
        foreach (var a in config.add_links)
        {
            var sensor = GameObject.Find(a.object1).GetComponent<Sensor>();
            var actuator = GameObject.Find(a.object2).GetComponent<Actuator>();
            if (sensor != null && actuator != null)
                sensor.AddActuator(actuator);
        }*/
    }

    /// <summary>
    /// Extract a list of spawnable objects from the text file
    /// </summary>
    public List<List<(string, Vector3, EntityGroup)>> ExtractSpawnableObjects(string[] floors)
    {
        if (floors.Length == 0) throw new ArgumentException("Expected a floor, but instead got an empty list!");
        int levelWidth = floors[0].Split(new[] { "\r\n", "\n" }, 2, StringSplitOptions.RemoveEmptyEntries)[0].Split(',').Length;
        Debug.Log($"Level width: {levelWidth}");

        //Add a list for all objects in a single floor
        var floorSpawnables = new List<List<(string, Vector3, EntityGroup)>>();
        for (int i = 0; i < floors.Length; i++)
        {
            int index = 0;

            var s = from row in SplitRows(floors[i])
                    from cell in row.Split(',')
                    let p = index++
                    let position = new Vector3(p % levelWidth, 0, (int)Math.Floor((double)p / levelWidth))
                    from obj in cell.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                    let symbol = obj.Split(new char[] { ID_SEPERATOR, ROT_SEPERATOR, COL_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries)[0]
                    let placeableGroup = placeablesMap.ContainsKey(symbol) ? placeablesMap[symbol].@group : placeablesMap["f"].@group
                    select (obj, position, placeableGroup);
            floorSpawnables.Add(s.ToList());
        }

        return floorSpawnables;
    }

    /// <summary>
    /// Spawn the object and parse and apply any additional parameters
    /// </summary>
    /// <param name="obj">The object string along with parameters</param>
    private void PlaceObject(string obj, Vector3 position, Transform parent)
    {
        string[] obj_id = obj.Split(ID_SEPERATOR);
        string[] color = obj_id[0].Split(COL_SEPERATOR);
        string[] orientation = color[0].Split(ROT_SEPERATOR);
        string tile = orientation[0];

        if (!placeablesMap.ContainsKey(tile))
        {
            UserErrorInfo.ErrorWriter.AddMessage($"Invalid symbol '{tile}' at position '{position}'");
            return;
        }

        Placeable p = placeablesMap[tile];

        if (p.prefabs.Length == 0)
        {
            UserErrorInfo.ErrorWriter.AddMessage($"No prefabs assigned for object '{tile}'");
            return;
        }

        GameObject prefab = p.prefabs[Random.Range(0, p.prefabs.Length)];
        GameObject instance = Instantiate(prefab, position, Quaternion.identity, parent);

        if (tile == "cb")
        {
            if (color.Length < 2) UserErrorInfo.ErrorWriter.AddMessage($"The ColorButton '{obj_id}' requires a color parameter");
            instance.GetComponent<Colorized>().SetColor(Utils.colorFromHexString(color[1]));
        }

        if (orientation.Length > 1)
            RotateObject(instance, orientation[1]);

        if (obj_id.Length > 1)
        {
            string id = obj_id[1];
            if (p.group == EntityGroup.Agents)
                AssignAgentID(instance, id, obj, position);
            else
                instance.name = id;
        }
        else if (ObjectIDRequired(p.group))
        {
            UserErrorInfo.ErrorWriter.AddWorldMessage($"No ID specified for '{tile}', required for {p.group}.", obj, position);
            return;
        }
    }

    /// <summary>
    /// Connects sensors with actuators, and npcs with walk patterns
    /// </summary>
    private void ConnectLinks(string table)
    {
        var rows = SplitRows(table);
        for (int j = 0; j < rows.Length; j++)
        {
            var cells = rows[j].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string linkID = cells[0];
            //Debug.Log(">>> " + linkID);

            GameObject link = GameObject.Find(linkID);

            if (!link)
            {
                UserErrorInfo.ErrorWriter.AddMessage($"Invalid sensor id '{linkID}'. The object was not found in the level. Line: {j}");
                continue;
            }

            // WP: adding this logic for the link/switch. When the switch is interacted-to (when the event
            // onIteract is invoked), this logic will cause its state to be toggled.
            // Note that in this this will fire the switch's onToggle event.
            if(link.GetComponent<Interactable>() && link.GetComponent<Toggleable>())
            {
                link.GetComponent<Interactable>().onInteract?.AddListener(_ => link.GetComponent<Toggleable>().ToggleState());
            }
            else
            {
                UserErrorInfo.ErrorWriter.AddMessage($"Something is wrong with '{linkID}': it has no Interactable or no Toggable component. Line: {j}");
                continue;
            }

            // this cannot be the case, since cells[0] exists:
            // if (cells.Length == 0)
            //    UserErrorInfo.ErrorWriter.AddMessage($"The sensor '{linkID}' does not have any objects connected to it. Line: {j}");

            for (var i = 1; i < cells.Length; i++)
            {
                GameObject connector = GameObject.Find(cells[i]);

                if (!connector)
                {
                    UserErrorInfo.ErrorWriter.AddMessage($"Invalid connected object id '{cells[i]}'. The object was not found in the level. Line: {j} Column: {i}");
                    continue;
                }
                if (link.GetComponent<Interactable>())
                {
                    //TODO Apply pseudo-random seed
                    bool broken = wireBreaking && Random.Range(0, 2) == 0;

                    if (!broken)
                    {
                        // Add the door logic. When the link/switch is interacted to (so, in turn the switch will
                        // fire onTogle event), we will also toggle this door (which connected to the switch):
                        if (connector?.GetComponent<Toggleable>())
                            link.GetComponent<Toggleable>()?.onToggle?.AddListener(_ => connector.GetComponent<Toggleable>().ToggleState());
                        //if (connector?.GetComponent<Toggleable>())
                        //    link.GetComponent<Interactable>()?.onInteract?.AddListener(_ => connector.GetComponent<Toggleable>().ToggleState());

                        if (connector?.GetComponent<ColorScreen>())
                            link.GetComponent<Interactable>()?.onInteract?.AddListener(connector.GetComponent<ColorScreen>().UpdateScreen);

                        // Handle switches
                        // WP: not sure if this is needed. I don't think the link/switch will ever receive
                        // an onToggle(s) event, unless it has been made listener of another switch.
                        // In this case, when the other switch is interacted, it will auto-trigger an
                        // onToggle(s) event, where s is the state of the switch. This logic below would
                        // then propagates the effect to all doors connected to the link.
                        // But so far we don't actually have a switch connected to anorther switch...
                        //link.GetComponent<Toggleable>()?.onToggle.AddListener(connector.GetComponent<Toggleable>().SetState);
                    }

                    GameObject wire = _wireBuilder.CreateWire(link.transform, connector.transform, 0.125f, 0.25f, broken);
                    wire.transform.SetParent(link.transform);
                }

                if (link.GetComponent<NPC>())
                    link.GetComponent<NPC>().points.Add(connector.transform.position);
            }
        }
    }


    /// <summary>
    /// Rotate an object in four possible directions
    /// </summary>
    /// <param name="s">The direction (north, east, south, west) to rotate the object to.</param>
    private void RotateObject(GameObject instance, string s)
    {
        Vector3 rotation = Vector3.zero;
        switch (s[0])
        {
            case 'w': return;
            case 's': rotation.y += 90; break;
            case 'e': rotation.y += 180; break;
            case 'n': rotation.y += 270; break;
            default: UserErrorInfo.ErrorWriter.AddMessage($"Invalid rotation parameter {s[0]}."); break;
        }
        instance.transform.rotation = Quaternion.Euler(rotation);
    }

    /// <summary>
    /// Assign the id to an agent's character, and prevent duplicate names
    /// </summary>
    private void AssignAgentID(GameObject instance, string id, string obj, Vector3 position)
    {
        if (agentIDs.Contains(id))
        {
            UserErrorInfo.ErrorWriter.AddWorldMessage($"Agent id '{id}' was not unique. Index: {position}", obj, position);
            return;
        }

        agentIDs.Add(id);

        instance.name = "Agent " + id;

        var character = instance.GetComponent<Character>();
        character.agentID = id;
    }

    /// <summary>
    /// Destroy all child objects of a previous world
    /// </summary>
    public void ClearWorld()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        _wireBuilder.StopAllCoroutines();
        agentIDs.Clear();
    }

    /// <summary>
    /// Add new base nodes in the scene hierarchy for a new floor.
    /// </summary>
    /// <param name="i">The floor number</param>
    private void AddNewFloorInSceneHierarchy(int i)
    {
        var floorObject = Utils.InstantiateEmpty("Floor " + i, this.transform);
        foreach (EntityGroup c in Enum.GetValues(typeof(EntityGroup)))
            if (IsFloorLocalTile(c)) sceneHierarchyNodes[c] = Utils.InstantiateEmpty(c.ToString(), floorObject.transform);
    }

    /// <summary>
    /// Function that retrieves a list of all the hazards (all the objects belonging to the EntitiyGroup.Dynamic). Used for whiskers.
    /// </summary>
    public FireHazard[] GetHazards()
    {
       // List<GameObject> hazards = new List<GameObject>();

        //GameObject hazardsParent = sceneHierarchyNodes[EntityGroup.Dynamic];
        return GameObject.FindObjectsOfType<FireHazard>();

        //foreach (Transform hazard in hazardsParent.transform)
        //    hazards.Add(hazard.gameObject);

        //return hazards.AsReadOnly();
    }

    /// <summary>
    /// Check if there are linkable objects in the level
    /// </summary>
    /// <param name="levelData">The level data</param>
    /// <returns></returns>
    private bool IsNoFloor(string levelData)
    {
        string check = levelData.Split(',')[0];

        foreach (Placeable placeable in placeables)
            if (check == placeable.symbol || check == "") return false;

        return true;
    }
    /// <summary>
    /// Whether an object is part of a floor, or should be placed at the root
    /// </summary>
    private bool IsFloorLocalTile(EntityGroup e)
    {
        return !(e == EntityGroup.Agents);
    }

    private bool ObjectIDRequired(EntityGroup e)
    {
        return (e == EntityGroup.Agents || e == EntityGroup.Interactives);
    }

    private string[] SplitRows(string text)
    {
        return text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    public string[] SplitSheets(string text)
    {
        return text.Split(new[] { "|", "|\r\n", "|\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
}