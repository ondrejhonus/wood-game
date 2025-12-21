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
        Destroy(currentAxeItem.gameObject);
        SpawnNewAxe();
        // todo: parent to inventory
    }
}