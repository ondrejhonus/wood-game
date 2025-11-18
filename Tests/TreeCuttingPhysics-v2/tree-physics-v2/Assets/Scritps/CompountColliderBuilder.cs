using UnityEngine;

[RequireComponent(typeof(TreeChunk))]
public class CompoundColliderBuilder : MonoBehaviour
{
    // Builds a set of CapsuleColliders spaced along the log length to approximate collision
    public void BuildFromVoxel(VoxelLog log)
    {
        // Remove existing colliders (except MeshCollider)
        CapsuleCollider[] existing = GetComponents<CapsuleCollider>();
        foreach (var c in existing) DestroyImmediate(c);

        float acc = 0f;
        int idx = 0;
        foreach (var seg in log.segments)
        {
            float mid = acc + seg * 0.5f;
            GameObject child = new GameObject("capsule_" + idx);
            child.transform.parent = transform;
            child.transform.localPosition = new Vector3(0f, mid, 0f);
            child.transform.localRotation = Quaternion.identity;
            var cap = child.AddComponent<CapsuleCollider>();
            cap.direction = 1; // Y axis
            cap.height = seg + log.radius * 2f;
            cap.radius = log.radius * 0.98f;
            idx++;
            acc += seg;
        }
    }
}