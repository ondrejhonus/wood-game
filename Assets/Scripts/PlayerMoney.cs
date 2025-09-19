using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public int money = 0;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        UpdateMoneyText();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public int GetMoney()
    {
        return money;
    }


    private void UpdateMoneyText()
    {
        moneyText.text = money + " $H";
    }
}