/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Generates and cleans a navmesh
/// </summary>
public class NavMeshHelper : MonoBehaviour
{
    private Mesh mesh;

    /// <summary>
    /// Generate a navmesh and remove duplicated vertices
    /// </summary>
    public NavMeshContainer GenerateNavMesh()
    {
        // @Temporary, the door colliders need to be disabled for the initial generation of the navmesh
        var doors = FindObjectsOfType<Door>();
        EnableDoorColliders(doors, false);

        FindObjectOfType<World>().transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        var triangulation = NavMesh.CalculateTriangulation();

        EnableDoorColliders(doors, true);

        // Process the mesh
        mesh = MeshHelper.CleanMeshData(triangulation.vertices, triangulation.indices);
        MeshHelper.FillNormals(mesh);
        return new NavMeshContainer(mesh.vertices, mesh.triangles);
    }

    public Mesh getMesh() { return mesh; }

    public void ClearMesh() { if (mesh != null) mesh.Clear(); }

    /// <summary>
    /// Enable or disable colliders that obstruct the navmesh
    /// </summary>
    private void EnableDoorColliders(Door[] doors, bool enabled)
    {
        foreach (var door in doors) 
            door.transform.Find("Door").GetComponent<BoxCollider>().enabled = enabled;
    }

    /// <summary>
    /// Draws the navmesh in the scene editor
    /// </summary>
    private void OnDrawGizmos()
    {
        if (mesh == null || mesh.vertices.Length == 0) return;

        Gizmos.color = new Color(0.44f, 0.96f, 0.99f, 0.1f); //@Todo, create some static debug colors
        Gizmos.DrawWireMesh(mesh);
    }
}
