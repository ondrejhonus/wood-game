using UnityEngine;
using TMPro; // For TextMeshPro

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public int money = 0;
    public int health = 100;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI healthText;

    [Header("Popup")]
    public GameObject popupPrefab; // Assign a prefab in Inspector
    public Transform popupParent;  // UI canvas for popups

    private void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount, Vector3 worldPosition)
    {
        money += amount;
        UpdateUI();
        ShowPopup("+" + amount + "$H", worldPosition, Color.green);
    }

    public void RemoveMoney(int amount, Vector3 worldPosition)
    {
        AddMoney(-amount, worldPosition);
    }

    public void TakeDamage(int amount, Vector3 worldPosition)
    {
        health -= amount;
        if (health < 0) health = 0;
        UpdateUI();
        ShowPopup("-" + amount + " HP", worldPosition, Color.red);
    }

    private void UpdateUI()
    {
        moneyText.text = money.ToString() + " $H";
        healthText.text = health.ToString() + " HP";
    }

    public void ShowPopup(string text, Vector3 worldPosition, Color color)
    {
        // Convert world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // make popup
        GameObject popup = Instantiate(popupPrefab, popupParent);
        popup.transform.position = screenPos;

        // set text to popup
        TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();
        popupText.text = text;
        popupText.color = color;

        // remove after 1 second
        Destroy(popup, 1f);
    }

    // Show popup for one frame only, so it appears as long as the player looks at the object
        public void ShowPopupOneFrame(string text, Vector3 worldPosition, Color color)
    {
        // Convert world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // make popup
        GameObject popup = Instantiate(popupPrefab, popupParent);
        popup.transform.position = screenPos;

        // set text to popup
        TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();
        popupText.text = text;
        popupText.color = color;

        // remove after 1 frame
        Destroy(popup, Time.deltaTime * 2); // multiplied by 2 to ensure it lasts the full frame
    }

    // Basic getters and setters for saving/loading
    public int GetMoney()
    {
        return money;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetMoney(int amount)
    {
        money = amount;
        UpdateUI();
    }

    public void SetHealth(int amount)
    {
        health = amount;
        UpdateUI();
    }
}