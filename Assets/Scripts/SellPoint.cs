using UnityEngine;

public class SellPoint : MonoBehaviour
{
    public PlayerStats playerStats;

    private void OnTriggerEnter(Collider other)
    {
        SellableObject item = other.GetComponent<SellableObject>();
        if (item != null)
        {
            int value = CalculateValue(item);
            playerStats.AddMoney(value, item.transform.position);

            Destroy(other.gameObject); // Remove sold object
        }
    }

    private int CalculateValue(SellableObject item)
    {
        int baseValue;

        switch (item.objectType)
        {
            case "basicLog":
                baseValue = 10;
                break;
            default:
                baseValue = 1;
                break;
        }

        return Mathf.RoundToInt(baseValue * item.transform.localScale.x * (item.transform.localScale.y * 2)); // my custom formula that doesnt make that much sense i just like it
    }
}
