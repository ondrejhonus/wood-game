using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple radial extrusion mesh builder for a straight log along local +Y.
/// Produces a tubular mesh with end caps.
/// </summary>
public static class LogMeshBuilder
{
    // resolution around circle
    private const int RADIAL_SEGMENTS = 16;

    /// <summary>
    /// Build mesh for a VoxelLog. The mesh is centered so that pivot at bottom (y=0).
    /// </summary>
    public static Mesh BuildMesh(VoxelLog log)
    {
        float length = log.TotalLength;
        int rings = log.segments.Count + 1; // vertices rings = segments count + 1
        float radius = log.radius;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        // Compute ring Y positions
        List<float> ringY = new List<float>();
        float acc = 0f;
        ringY.Add(0f);
        foreach (var s in log.segments)
        {
            acc += s;
            ringY.Add(acc);
        }

        // Generate vertices
        for (int r = 0; r < ringY.Count; r++)
        {
            float y = ringY[r];
            for (int i = 0; i < RADIAL_SEGMENTS; i++)
            {
                float ang = (i / (float)RADIAL_SEGMENTS) * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(ang) * radius, y, Mathf.Sin(ang) * radius);
                verts.Add(p);
                normals.Add(new Vector3(p.x, 0f, p.z).normalized);
                uvs.Add(new Vector2(i / (float)RADIAL_SEGMENTS, y / length));
            }
        }

        int ringsCount = ringY.Count;
        // side tris
        for (int r = 0; r < ringsCount - 1; r++)
        {
            for (int i = 0; i < RADIAL_SEGMENTS; i++)
            {
                int ni = (i + 1) % RADIAL_SEGMENTS;
                int a = r * RADIAL_SEGMENTS + i;
                int b = r * RADIAL_SEGMENTS + ni;
                int c = (r + 1) * RADIAL_SEGMENTS + i;
                int d = (r + 1) * RADIAL_SEGMENTS + ni;

                // two tris: a,c,b and b,c,d
                tris.Add(a);
                tris.Add(c);
                tris.Add(b);

                tris.Add(b);
                tris.Add(c);
                tris.Add(d);
            }
        }

        // Caps: bottom and top
        int bottomCenterIndex = verts.Count;
        verts.Add(new Vector3(0f, 0f, 0f));
        normals.Add(Vector3.down);
        uvs.Add(Vector2.zero);

        for (int i = 0; i < RADIAL_SEGMENTS; i++)
        {
            int next = (i + 1) % RADIAL_SEGMENTS;
            tris.Add(bottomCenterIndex);
            tris.Add(next);
            tris.Add(i);
        }

        int topCenterIndex = verts.Count;
        verts.Add(new Vector3(0f, length, 0f));
        normals.Add(Vector3.up);
        uvs.Add(Vector2.zero);

        int topRingStart = (ringsCount - 1) * RADIAL_SEGMENTS;
        for (int i = 0; i < RADIAL_SEGMENTS; i++)
        {
            int next = (i + 1) % RADIAL_SEGMENTS;
            tris.Add(topCenterIndex);
            tris.Add(topRingStart + i);
            tris.Add(topRingStart + next);
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        // smooth normals if desired
        return mesh;
    }
}
