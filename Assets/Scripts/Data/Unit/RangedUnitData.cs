using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitData / RangedUnitData")]
public class RangedUnitData : UnitData
{
    [SerializeField]
    private Projectile projectile;

    public Projectile Projectile => projectile;
}
