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

    private void ShowPopup(string text, Vector3 worldPosition, Color color)
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
}
