using Fusion;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IAttackBehaviour
{
    RangedUnitData unitData;

    public RangedAttack(RangedUnitData unitData)
    {
        this.unitData = unitData;
    }

    public void Attack(Unit unit, Unit target)
    {
        Projectile projectile = unit.Runner.Spawn(unitData.Projectile, transform.position);
        projectile.Init(unit, target);
    }
}
