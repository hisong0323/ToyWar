using Fusion;
using UnityEngine;

public class UnitPlacer : NetworkBehaviour
{
    [SerializeField]
    private Unit unitPrefab;


    public override void FixedUpdateNetwork()
    {
        /*        if (Object.HasStateAuthority)
                    foreach (var player in Runner.ActivePlayers)
                    {
                        if (Runner.TryGetInputForPlayer<NetworkInputData>(player, out var data))
                        {
                            if (data.Buttons.IsSet(NetworkInputData.SpawnButton))
                            {
                                GameManager.Instance.SpendMoney(player, 10);

                                Runner.Spawn(unitPrefab, data.SpawnPosition, Quaternion.identity, player);
                                Runner.Despawn(Object);
                            }
                        }
                    }*/

        if (Object.HasInputAuthority)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, LayerMask.GetMask("Ground")))
            {
                transform.position = hit.point + Vector3.up * 2;
            }
            if (GetInput(out NetworkInputData data))
                if (data.Buttons.IsSet(NetworkInputData.SpawnButton))
                {
                    GameManager.Instance.SpendMoney(Object.InputAuthority, 10);

                    RPC_UnitSpawn(data.SpawnPosition);
                }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_UnitSpawn(Vector3 spawnPosition)
    {
        Runner.Spawn(unitPrefab, spawnPosition, Quaternion.identity, Object.InputAuthority);
        Runner.Despawn(Object);
    }
}
