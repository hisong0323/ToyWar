using Fusion;
using UnityEngine;

public class UnitPlacer : NetworkBehaviour
{
    [SerializeField]
    private Unit unitPrefab;

    private MeshFilter model;

    private UnitData unitData;

    private void Awake()
    {
        model = transform.GetComponentInChildren<MeshFilter>();
    }

    public void Init(UnitData data)
    {
        unitData = data;
        model.sharedMesh = data.Model.GetComponent<MeshFilter>().sharedMesh;
    }

    public override void FixedUpdateNetwork()
    {
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
                    RPC_UnitSpawn(data.SpawnPosition);
                }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_UnitSpawn(Vector3 spawnPosition)
    {
        Unit unit = Runner.Spawn(unitPrefab, spawnPosition, Quaternion.identity, Object.InputAuthority);
        unit.Init(unitData);
        Runner.Despawn(Object);
    }
}
