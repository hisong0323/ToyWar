using UnityEngine;

[System.Serializable]
public class TierUnitData
{
    [SerializeField]
    private int tier;

    public UnitData[] UnitDatas;
}

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitDatas/UnitDatas")]
public class UnitDatas : ScriptableObject
{
    [SerializeField]
    private TierUnitData[] tierUnitDatas;

    public TierUnitData[] TierUnitDatas => tierUnitDatas;   
}
