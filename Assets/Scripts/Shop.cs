using Fusion;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    [SerializeField]
    private UnitDatas unitDatas;

    [SerializeField]
    private UnitPlacer unitPlacer;

    public void RandomUnitData(int tier)
    {
        //Debug.Log(unitDatas.TierUnitDatas[tier - 1].UnitDatas[0].UnitName);

        if (Runner.LocalPlayer == Runner.LocalPlayer)
        {
            Debug.Log("소환 실행");
            RPC_UnitPlacerSpawn(Runner.LocalPlayer);
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_UnitPlacerSpawn(PlayerRef player)
    {
        Runner.Spawn(unitPlacer, transform.position, Quaternion.identity, player);
    }
}
