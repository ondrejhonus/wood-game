using UnityEngine;
using System.Collections.Generic;

public class TreeRenderManager : MonoBehaviour
{
    public static TreeRenderManager instance;

    [Header("Settings")]
    public Transform player;         // player camera
    public float renderDistance = 150f; 
    public float checkInterval = 0.2f; // how often to check

    private class TreeData
    {
        public Transform transform;
        public Renderer[] renderers; // save render of trunk and leaves
        public bool isVisible;
    }

    private List<TreeData> registeredTrees = new List<TreeData>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        StartCoroutine(CheckVisibilityLoop());
    }

    public void RegisterTree(GameObject treeObj)
    {
        TreeData data = new TreeData();
        data.transform = treeObj.transform;
        
        data.renderers = treeObj.GetComponentsInChildren<Renderer>();
        data.isVisible = true;

        registeredTrees.Add(data);
    }

    System.Collections.IEnumerator CheckVisibilityLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(checkInterval);

        while (true)
        {
            if (player == null) 
            {
                yield return wait;
                continue;
            }

            Vector3 playerPos = player.position;
            float distSq = renderDistance * renderDistance;

            for (int i = registeredTrees.Count - 1; i >= 0; i--)
            {
                TreeData tree = registeredTrees[i];

                if (tree.transform == null)
                {
                    registeredTrees.RemoveAt(i);
                    continue;
                }

                float currentDistSq = (tree.transform.position - playerPos).sqrMagnitude;
                bool shouldBeVisible = currentDistSq < distSq;

                if (tree.isVisible != shouldBeVisible)
                {
                    tree.isVisible = shouldBeVisible;
                    foreach (var r in tree.renderers)
                    {
                        if(r != null) r.enabled = shouldBeVisible;
                    }
                }
            }

            yield return wait;
        }
    }
}