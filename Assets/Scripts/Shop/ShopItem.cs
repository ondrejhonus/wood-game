using UnityEngine;
using System.Collections;

public class ShopItem : MonoBehaviour
{
    public int price = 50;
    public bool isPurchased = false;
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    public GameObject itemPrefab; // The prefab to respawn

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Call this when the player first picks it up
    public void OnPickedUp()
    {
        if (!isPurchased)
        {
            StartCoroutine(RespawnTimer());
        }
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(10f); // 2 minutses
        Instantiate(itemPrefab, startPosition, startRotation);
        // TODO: link to shop system
    }
}