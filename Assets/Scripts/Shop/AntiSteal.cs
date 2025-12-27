using UnityEngine;

public class AntiSteal : MonoBehaviour
{
    public PlayerStats playerStats;
    private void OnTriggerExit(Collider other)
    {
        ShopItem item = other.GetComponent<ShopItem>();

        if (item != null && !item.isPurchased)
        {
            if (item != null && !item.isPurchased)
            {
                playerStats.ShowPopup("You shall not steal!", item.transform.position, Color.red);
                Destroy(other.gameObject);
            }
        }
    }
}
