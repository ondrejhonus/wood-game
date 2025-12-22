using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ShopItem : MonoBehaviour
{
    public int price = 50;
    public bool isPurchased = false;

    private Vector3 startPosition;
    private Quaternion startRotation;
    public GameObject itemPrefab; // The prefab to respawn
    public InventoryRestocker mySlot; // Reference to the slot that spawned this item

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (isPurchased)
        {
            // add tag "Axe" to the object
            gameObject.tag = "Axe";
        }
    }

    // Call this when the player first picks it up
    public void OnPickedUp()
    {
        if (!isPurchased)
        {
            mySlot.StartRespawnTimer();
        }
    }
}