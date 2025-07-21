using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitDatas/UnitDatas")]
public class UnitDatabase : ScriptableObject
{
    [SerializeField]
    private UnitData[] unitDatas;

    public UnitData[] UnitDatas => unitDatas;
}
