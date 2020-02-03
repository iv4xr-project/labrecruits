/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Construct and manipulate mesh data.
/// </summary>
public class MeshHelper
{
    /// <summary>
    /// Construct a mesh from the minimum components
    /// </summary>
    public static Mesh FromComponents(Vector3[] vertices, int[] indices)
    {
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = indices;
        return m;
    }

    /// <summary>
    /// Add a set of normals to a mesh.
    /// </summary>
    public static void FillNormals(Mesh mesh)
    {
        var normals = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
            normals[i] = Vector3.up;

        mesh.normals = normals;
    }

    /// <summary>
    /// Clean a mesh by removing duplicate vertices
    /// </summary>
    public static Mesh CleanMeshData(Vector3[] vertices, int[] indices)
    {
#if false // @Incomplete, This code detects duplicate triangles, but this is not an issue right now.
        HashSet<(Vector3, Vector3, Vector3)> triangles = new HashSet<(Vector3, Vector3, Vector3)>();
        for (int i = 0; i < indices.Length; i += 3) {
            var tri = (vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]]);
            if (!triangles.Add(tri)) {

            }
        }
#endif
        return RemoveDuplicateVertices(vertices, indices);
    }
    /// <summary>
    /// Removes duplicate vertices, and updates the indices to match it.
    /// </summary>
    private static Mesh RemoveDuplicateVertices(Vector3[] vertices, int[] indices)
    {
        var map = new Dictionary<Vector3, int>();
        var unique = new List<Vector3>();
        var changes = new List<int>(); // Holds the new index for each vertex in the original array.

        Vector3 v;
        int new_index;

        // For each vertex, 
        //    increment index counter and add it, if it is unique
        //    look up the index of the first vertex occurence, otherwise
        for (int i = 0; i < vertices.Length; i++) {
            v = vertices[i];
            if (!map.ContainsKey(v)) {
                map.Add(v, i);
                unique.Add(v);
                
                new_index = unique.Count - 1;
            }
            else {
                new_index = changes[map[v]];
            }

            changes.Add(new_index);
        }
        vertices = unique.ToArray();
        
        // Copy the updated indices over
        int[] new_indices = new int[indices.Length];
        for (int i = 0; i < indices.Length; i++) new_indices[i] = changes[indices[i]];

        indices  = new_indices;

        var m = new Mesh();
        m.vertices = vertices;
        m.triangles = indices;
        return m;
    }
}