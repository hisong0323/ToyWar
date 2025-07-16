using Fusion;
using TMPro;
using UnityEngine;

public class ShopUI : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerMoneyText;

    public override void Spawned()
    {
        GameManager.Instance.ChangedMoneyAction += UpdateMoneyUI;
    }

    public void UpdateMoneyUI()
    {
        playerMoneyText.text = $"{GameManager.Instance.PlayerMoney.Get(Runner.LocalPlayer.PlayerId - 1)}$";
    }
}
