using UnityEngine;

public class MeleeAttack : MonoBehaviour, IAttackBehaviour
{
    public void Attack(Unit unit, Unit target)
    {
        target.TakeDamage(unit.UnitData.AttackPower);
    }
}
