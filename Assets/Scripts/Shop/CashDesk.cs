using UnityEngine;

public class CashDesk : MonoBehaviour 
{
    public PlayerStats playerStats;
    public AudioSource purchaseSound;

    private void OnTriggerEnter(Collider other)
    {
        ShopItem item = other.GetComponent<ShopItem>();

        if (item != null && !item.isPurchased)
        {
            if (playerStats.GetMoney() >= item.price)
            {
                playerStats.RemoveMoney(item.price, item.transform.position);
                item.isPurchased = true;
                purchaseSound.Play();
            }
            else
            {
                Debug.Log("Not enough money!");
                playerStats.ShowPopup("Not enough $H", item.transform.position, Color.red);
            }
        }
    }

}