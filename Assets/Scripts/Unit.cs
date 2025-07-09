using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Unit : NetworkBehaviour
{

    [SerializeField] private Faction faction;

    [Networked] private TickTimer attackCooldown { get; set; }

    [Networked] private TickTimer spawnDelay { get; set; }

    [SerializeField]
    private UnitData _unitData;

    [SerializeField]
    private Slider hpBar;

    private Unit targetBase;

    private Unit target;

    private Collider[] detectionArea = new Collider[50];

    private float targetDistance;

    private bool canAttack = true;

    private int unitLayerMask;

    private IAttackBehaviour attackBehaviour;

    private int currentHP;
    public Faction Faction => faction;
    public UnitData UnitData => _unitData;

    public void Init(Faction faction)
    {
        this.faction = faction;

        if (this.faction == Faction.Player1)
            targetBase = GameManager.Instance.PlayerBase[1];
        else
            targetBase = GameManager.Instance.PlayerBase[0];
    }

    private void Awake()
    {
        unitLayerMask = LayerMask.GetMask("Unit");
    }

    private void Start()
    {
        switch (_unitData.UnitType)
        {
            case UnitType.Melee:
                attackBehaviour = new MeleeAttack();
                break;
            case UnitType.Ranged:
                attackBehaviour = new RangedAttack((RangedUnitData)_unitData);
                break;
        }
    }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            spawnDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);
            currentHP = _unitData.MaxHP;
            UpdateHPBar();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || !spawnDelay.Expired(Runner))
        {
            return;
        }

        DetectionUnit();

        if (IsInAttackRange())
        {
            TryAttack();
        }
        else if (_unitData.MoveSpeed > 0)
        {
            Chase();
        }

        if (attackCooldown.Expired(Runner) && canAttack == false)
        {
            canAttack = true;
        }
    }

    private bool IsInAttackRange()
    {
        if (target == null)
            return false;

        return targetDistance <= _unitData.AttackRange;
    }

    private void TryAttack()
    {
        if (canAttack)
        {
            attackCooldown = TickTimer.CreateFromSeconds(Runner, _unitData.AttackDelay);
            canAttack = false;
            attackBehaviour.Attack(this, target);
        }
    }

    private void UpdateHPBar()
    {
        if (hpBar == null)
            return;

        hpBar.value = (float)currentHP / _unitData.MaxHP;
    }

    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority)
            return;

        currentHP -= damage;
        UpdateHPBar();
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Runner.Despawn(Object);
    }

    private void DetectionUnit()
    {
        float minDistance = float.MaxValue;
        int unitCount = Physics.OverlapSphereNonAlloc(transform.position, _unitData.DetectionRange, detectionArea, unitLayerMask);

        for (int i = 0; i < unitCount; i++)
        {
            if (detectionArea[i].TryGetComponent(out Unit targetUnit))
            {
                if (targetUnit != this && faction != targetUnit.Faction)
                {
                    float distnce = Vector3.Distance(transform.position, detectionArea[i].ClosestPoint(transform.position));
                    if (distnce < minDistance)
                    {
                        minDistance = distnce;
                        target = targetUnit;
                    }
                }
            }
        }
        targetDistance = minDistance;
    }

    private void Chase()
    {
        if (target != null)
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _unitData.MoveSpeed * Runner.DeltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, targetBase.transform.position, _unitData.MoveSpeed * Runner.DeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        /* if (other.gameObject.CompareTag("Ground"))
         {
             _collder.isTrigger = false;
             Collider[] spawnArea = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Unit"));

             foreach (var unit in spawnArea)
             {
                 if (unit.TryGetComponent(out Rigidbody target) || unit != this)
                 {
                     Vector3 direction = target.transform.position - transform.position;
                     direction.y = 0;
                     float distance = Mathf.Max(direction.magnitude, 1f);
                     direction.Normalize();
                     target.AddForce(direction * (4 / distance), ForceMode.Impulse);
                     Debug.Log("밀치기!!");
                 }
             }
         }*/
    }

    private void OnDrawGizmosSelected()
    {
        //감지 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _unitData.DetectionRange);

        //공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _unitData.AttackRange);
    }
}
