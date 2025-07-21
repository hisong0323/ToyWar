using Fusion;
using UnityEngine;

public class Item : NetworkBehaviour
{
    private MeshFilter model;

    private UnitData unitData;
    public UnitData UnitData => unitData;

    private void Awake()
    {
        model = transform.GetComponentInChildren<MeshFilter>();
    }

    public void Init(UnitData unitData)
    {
        this.unitData = unitData;
        model.sharedMesh = unitData.Model.GetComponent<MeshFilter>().sharedMesh;
    }
}

