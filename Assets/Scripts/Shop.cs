using Fusion;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    [SerializeField]
    private int tier;

    [SerializeField]
    private UnitDatabase unitDatabase;

    [SerializeField]
    private Unit unitPrefab;

    [SerializeField]
    private Item[] items;

    private UnitData unitData;

    public override void Spawned()
    {
        Setting();
    }

    private void Setting()
    {
        for (int i = 0; i < items.Length; i++)
        {
            int itt = Random.Range(0, unitDatabase.UnitDatas.Length);
            items[i].Init(unitDatabase.UnitDatas[itt]);
            Debug.Log(itt);
        }
    }

    public void Buy(Item item)
    {
        unitData = item.UnitData;

        Debug.Log("±¸¸Å");

        if (GameManager.Instance.SpendMoney(Runner.LocalPlayer, unitData.Price))
        {
            RPC_UnitSpawn();
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_UnitSpawn()
    {
        Unit unit = Runner.Spawn(unitPrefab, transform.position, Quaternion.identity, Runner.LocalPlayer);
        unit.Init(unitData);
    }
}
