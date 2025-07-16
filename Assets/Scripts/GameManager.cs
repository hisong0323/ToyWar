using System;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private Unit[] playerBase;

    [Networked, OnChangedRender(nameof(ChangedMoney)), Capacity(2)] private NetworkArray<int> playerMoney => default;

    public Action ChangedMoneyAction;

    private TickTimer timer;

    public static GameManager Instance { get; private set; }

    public Unit[] PlayerBase => playerBase;
    public NetworkArray<int> PlayerMoney => playerMoney;

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        timer = TickTimer.CreateFromSeconds(Runner, 0.1f);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (timer.Expired(Runner))
        {
            timer = TickTimer.CreateFromSeconds(Runner, 0.1f);
            foreach (var player in Runner.ActivePlayers)
            {
                GainMoney(player, 1);
            }
        }
    }

    private void ChangedMoney()
    {
        ChangedMoneyAction();
    }

    public void GainMoney(PlayerRef player, int value)
    {
        int index = player.PlayerId - 1;
        playerMoney.Set(index, playerMoney.Get(index) + value);
    }

    public void SpendMoney(PlayerRef player, int value)
    {
        int index = player.PlayerId - 1;
        playerMoney.Set(index, playerMoney.Get(index) - value);
    }
}
