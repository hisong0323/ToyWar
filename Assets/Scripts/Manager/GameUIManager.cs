using Fusion;
using TMPro;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerMoney;

    public override void FixedUpdateNetwork()
    {
        playerMoney.text = $"{GameManager.Instance.PlayerMoney.Get(Runner.LocalPlayer.PlayerId - 1)}¿ø";
    }
}
