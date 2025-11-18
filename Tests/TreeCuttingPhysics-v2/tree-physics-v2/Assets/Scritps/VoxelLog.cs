using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logical voxel/segment representation of a straight log.
/// Each segment has a length. The full log lies along transform.up (local y).
/// </summary>
[Serializable]
public class VoxelLog
{
    // length of each segment in world units (can be uniform)
    public List<float> segments = new List<float>();

    // radius of the log
    public float radius = 0.5f;

    public VoxelLog() { }

    public VoxelLog(IEnumerable<float> segs, float radius)
    {
        this.segments = new List<float>(segs);
        this.radius = radius;
    }

    public float TotalLength
    {
        get
        {
            float s = 0f;
            foreach (var v in segments) s += v;
            return s;
        }
    }

    /// <summary>
    /// Split at a segment index+offset (distance along that segment).
    /// Returns (lowerLog, upperLog). If cutAtDistance <= 0 or >= TotalLength then returns null for one side.
    /// </summary>
    public (VoxelLog lower, VoxelLog upper) SplitAtDistance(float cutDistance)
    {
        if (cutDistance <= 0f) return (null, Clone());
        if (cutDistance >= TotalLength) return (Clone(), null);

        List<float> lower = new List<float>();
        List<float> upper = new List<float>();

        float acc = 0f;
        for (int i = 0; i < segments.Count; i++)
        {
            float seg = segments[i];
            if (acc + seg < cutDistance - 1e-6f)
            {
                // whole segment in lower
                lower.Add(seg);
            }
            else if (acc >= cutDistance + 1e-6f)
            {
                // whole segment in upper
                upper.Add(seg);
            }
            else
            {
                // cutting inside this segment
                float cutInSegment = Mathf.Clamp(cutDistance - acc, 0f, seg);
                float lowerPart = cutInSegment;
                float upperPart = seg - cutInSegment;

                if (lowerPart > 1e-6f) lower.Add(lowerPart);
                if (upperPart > 1e-6f) upper.Add(upperPart);
            }

            acc += seg;
        }

        VoxelLog L = lower.Count > 0 ? new VoxelLog(lower, radius) : null;
        VoxelLog U = upper.Count > 0 ? new VoxelLog(upper, radius) : null;
        return (L, U);
    }

    public VoxelLog Clone()
    {
        return new VoxelLog(new List<float>(segments), radius);
    }

    /// <summary>
    /// Get world-space point (local Y) position along the log at localDistance from bottom (0 = bottom).
    /// </summary>
    public float LocalYAtDistance(float distance)
    {
        return Mathf.Clamp(distance, 0f, TotalLength);
    }
}
