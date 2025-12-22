using UnityEngine;
using System.Collections;

public class InventoryRestocker : MonoBehaviour
{
    public GameObject axePrefab; 
    public ShopItem currentAxeItem;
    public float respawnDelay = 60f;
    private bool isTimerRunning = false;

    void Start()
    {
        // SpawnNewAxe();
    }

    public void SpawnNewAxe()
    {
        GameObject newAxe = Instantiate(axePrefab, transform.position, transform.rotation);
        
        // Link the axe back to this slot
        ShopItem itemScript = newAxe.GetComponent<ShopItem>();
        if (itemScript != null)
        {
            itemScript.mySlot = this;
        }
        isTimerRunning = false;

        currentAxeItem = newAxe.GetComponent<ShopItem>();
        currentAxeItem.mySlot = this;
        newAxe.GetComponent<ObjectGrabbable>().enabled = true;
        newAxe.GetComponent<ShopItem>().enabled = true;
        newAxe.GetComponent<Rigidbody>().isKinematic = false;
        newAxe.GetComponent<ShopItem>().isPurchased = false;
        newAxe.tag = "ShopItem";
        newAxe.SetActive(true);

        GameObject InventoryObject = GameObject.Find("Inventory");
        if (InventoryObject != null)
        {
            newAxe.transform.SetParent(InventoryObject.transform);
        }
    }
    public void StartRespawnTimer()
    {
        if (!isTimerRunning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        isTimerRunning = true;
        yield return new WaitForSeconds(respawnDelay);
        if (currentAxeItem != null && !currentAxeItem.isPurchased)
        {
            Destroy(currentAxeItem.gameObject);
        }
        SpawnNewAxe();
    }
}