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
        FindObjectOfType<World>().transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        var triangulation = NavMesh.CalculateTriangulation();

        // Process the mesh
        mesh = MeshHelper.CleanMeshData(triangulation.vertices, triangulation.indices);
        MeshHelper.FillNormals(mesh);
        return new NavMeshContainer(mesh.vertices, mesh.triangles);
    }

    public Mesh getMesh() 
        => mesh;

    public void ClearMesh()
        => mesh?.Clear();

    /// <summary>
    /// Draws the navmesh in the scene editor
    /// </summary>
    private void OnDrawGizmos()
    {
        if (mesh == null || mesh.vertices.Length == 0) return;

        Gizmos.color = new Color(0.8f, 0f, 0f, 1f); //@Todo, create some static debug colors
        Gizmos.DrawWireMesh(mesh);
    }
}
