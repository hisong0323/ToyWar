using Fusion;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private ProjectileData projectileData;

    Unit owner;

    Vector3 targetPosition;

    public void Init(Unit unit, Unit target)
    {
        owner = unit;
        targetPosition = target.transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
            Runner.Despawn(Object);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, projectileData.Speed * Runner.DeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
        {
            if (unit.Faction != owner.Faction)
            {
                unit.TakeDamage(owner.UnitData.AttackPower);
                if (!projectileData.IsPiercing)
                    Runner.Despawn(Object);
            }
        }
    }

}
