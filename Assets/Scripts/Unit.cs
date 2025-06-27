using Fusion;
using UnityEngine;

public abstract class Unit : NetworkBehaviour
{
    [SerializeField] protected UnitData _unitData;

    [SerializeField][Networked] private Faction faction { get; set; }

    [Networked] protected TickTimer attackCooldown { get; set; }

    [SerializeField]
    private GameObject targetBase;

    private Collider[] detectionArea;

    private bool canAttack;
    public Faction Faction => faction;

    public void Init(Faction faction)
    {
        this.faction = faction;

        if (this.faction == Faction.Player1)
            targetBase = GameManager.Instance.PlayerBase[1];
        else
            targetBase = GameManager.Instance.PlayerBase[0];
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {

        }
    }

    public override void FixedUpdateNetwork()
    {

        if (!Object.HasStateAuthority)
        {
            return;
        }
        if (attackCooldown.Expired(Runner))
        {
            Debug.Log("공격 가능!");
            canAttack = true;
        }
        FindTarget();
    }

    private void TryAttack()
    {
        if (canAttack)
        {
            attackCooldown = TickTimer.CreateFromSeconds(Runner, _unitData.AttackDelay);
            canAttack = false;
            Debug.Log("공격함!");
        }
        else
        {
            Debug.Log("공격 쿨타임 기다리는중!");
        }
    }

    protected abstract void Attack();

    protected void FindTarget()
    {
        Vector3 centerPosition = transform.forward * _unitData.DetectionCenterOffset;

        detectionArea = Physics.OverlapSphere(transform.position + centerPosition, _unitData.AttackRange, LayerMask.GetMask("Unit"));

        Transform targetTransform = DetectionUnit(detectionArea);

        if (targetTransform != null)
        {
            TryAttack();
        }
        else
        {
            detectionArea = Physics.OverlapSphere(transform.position, _unitData.DetectionRange, LayerMask.GetMask("Unit"));
            targetTransform = DetectionUnit(detectionArea);
        }

        targetTransform = targetTransform != null ? targetTransform : targetBase.transform;

        Chase(targetTransform.position);
    }

    private Transform DetectionUnit(Collider[] detectionArea)
    {
        float minDistance = float.MaxValue;
        Transform targetTransform = null;

        foreach (var unit in detectionArea)
        {
            if (unit.TryGetComponent(out Unit target))
            {
                if (target != this && faction != target.Faction)
                {
                    float distnce = Vector3.Distance(transform.position, target.transform.position);
                    if (distnce < minDistance)
                    {
                        minDistance = distnce;
                        targetTransform = target.transform;
                    }
                }
            }
        }

        return targetTransform;
    }

    private void Chase(Vector3 targetTransform)
    {

        Vector3 newPos = Vector3.MoveTowards(transform.position, targetTransform, _unitData.MoveSpeed * Runner.DeltaTime);

        transform.position = newPos;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _unitData.DetectionCenterOffset, _unitData.DetectionRange);
    }
}
