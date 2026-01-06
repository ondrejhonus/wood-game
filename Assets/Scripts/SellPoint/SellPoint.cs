using UnityEngine;

public class SellPoint : MonoBehaviour
{
    public PlayerStats playerStats;
    private class SoldMarker : MonoBehaviour { } // Prevent double selling by marking sold items

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<SellableObject>(out var item)) return;
        if (other.GetComponent<SoldMarker>() != null) return; // already sold

        other.gameObject.AddComponent<SoldMarker>(); // mark as sold

        int value = CalculateValue(item);
        playerStats.AddMoney(value, item.transform.position);

        Destroy(other.gameObject);
    }

    private int CalculateValue(SellableObject item)
    {
        int baseValue;

        switch (item.objectType)
        {
            case "basicLog":
                baseValue = 3;
                break;
            case "cacti":
                baseValue = 15;
                break;
            case "winterLog":
                baseValue = 36;
                break;
            default:
                baseValue = 1;
                break;
        }

        return Mathf.RoundToInt(baseValue * item.transform.localScale.x * (item.transform.localScale.y)); // my custom formula that doesnt make that much 
                                                                                                            // sense i just like it
    }
}
