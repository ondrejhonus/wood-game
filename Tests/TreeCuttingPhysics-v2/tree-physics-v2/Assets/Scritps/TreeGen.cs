using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns a straight trunk made of N equal segments.
/// </summary>
public class TreeGenerator : MonoBehaviour
{
    public Material logMaterial;
    public int segments = 8;
    public float segmentLength = 0.6f;
    public float radius = 0.5f;

    void Start()
    {
        CreateSimpleTrunk(transform.position, transform.rotation);
    }

    public void CreateSimpleTrunk(Vector3 pos, Quaternion rot)
    {
        List<float> segs = new List<float>();
        for (int i = 0; i < segments; i++) segs.Add(segmentLength);
        VoxelLog log = new VoxelLog(segs, radius);

        TreeChunk chunk = TreeChunk.CreateFromVoxelLog(log, logMaterial, "Trunk");
        chunk.transform.position = pos;
        chunk.transform.rotation = rot;
        // Optional: parent to this generator for scene cleanliness
        chunk.transform.parent = transform;
    }
}
