using UnityEngine;
using System.Collections.Generic;

public class TreeChunk : MonoBehaviour
{
    public Transform staticPart;
    public Transform fallingPart;

    private Rigidbody rb;
    private bool hasFallen = false;

    void Awake()
    {
        rb = fallingPart.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Called when a LogSegment is hit
    public void Cut(LogSegment hitSegment)
    {
        if (hasFallen) return;

        // 1. Separate segments into below and above the cut
        List<Transform> above = new List<Transform>();
        List<Transform> below = new List<Transform>();

        foreach (Transform seg in staticPart)
        {
            LogSegment ls = seg.GetComponent<LogSegment>();

            if (ls.index > hitSegment.index)
                above.Add(seg);
            else
                below.Add(seg);
        }

        if (above.Count == 0)
            return;

        // 2. Move the "above" pieces to FallingPart
        foreach (Transform seg in above)
        {
            seg.SetParent(fallingPart);
        }

        // 3. Drop the falling part
        rb.isKinematic = false;
        hasFallen = true;

        // 4. After falling stops, turn it into a new chunk
        StartCoroutine(DetachWhenStopped());
    }

    private System.Collections.IEnumerator DetachWhenStopped()
    {
        // Wait until falling chunk stops moving
        while (rb.linearVelocity.magnitude > 0.1f)
            yield return null;

        CreateNewChunkFromFallingPart();
    }

    private void CreateNewChunkFromFallingPart()
    {
        if (fallingPart.childCount == 0) return;

        // Create a new TreeChunk at the current position
        GameObject newObj = Instantiate(gameObject,
            fallingPart.position, fallingPart.rotation);

        TreeChunk newChunk = newObj.GetComponent<TreeChunk>();

        // Clear the new chunk's default segments
        for (int i = newChunk.staticPart.childCount - 1; i >= 0; i--)
            Destroy(newChunk.staticPart.GetChild(i).gameObject);
        for (int i = newChunk.fallingPart.childCount - 1; i >= 0; i--)
            Destroy(newChunk.fallingPart.GetChild(i).gameObject);

        // Move falling segments into the new chunk's static part
        foreach (Transform seg in fallingPart)
        {
            seg.SetParent(newChunk.staticPart);
        }

        // Reset physics
        newChunk.fallingPart.GetComponent<Rigidbody>().isKinematic = true;
        newChunk.hasFallen = false;

        // Clear fallingPart in old chunk
        for (int i = fallingPart.childCount - 1; i >= 0; i--)
        {
            Destroy(fallingPart.GetChild(i).gameObject);
        }
    }
}
