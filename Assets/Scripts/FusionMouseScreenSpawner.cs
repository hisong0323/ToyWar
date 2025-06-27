using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class FusionMouseScreenSpawner : NetworkBehaviour
{
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private float spawnDistance = 10f;

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;          // 서버만 전체 입력 검사

        foreach (var player in Runner.ActivePlayers)
        {
            if (Runner.TryGetInputForPlayer<NetworkInputData>(player, out var data))
            {
                if (data.Buttons.IsSet(NetworkInputData.SpawnButton))
                {
                    Faction faction = player.PlayerId == 1 ? Faction.Player1 : Faction.Player2;

                    var unit = Runner.Spawn(unitPrefab,
                                            data.SpawnPosition,
                                            Quaternion.identity,
                                            player);

                    unit.GetComponent<Unit>().Init(faction);
                }
            }
        }
    }
}
