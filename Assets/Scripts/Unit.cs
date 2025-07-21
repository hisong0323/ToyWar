using Fusion;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.NetworkBehaviour;

public class Unit : NetworkBehaviour
{
    [Networked] private TickTimer attackCooldown { get; set; }

    [Networked] private TickTimer spawnDelay { get; set; }

    [SerializeField]
    private Faction faction;

    [SerializeField]
    private UnitData _unitData;

    [SerializeField]
    private Slider hpBar;

    [Networked, OnChangedRender(nameof(UpdateHPBar))] private int currentHP { get; set; }

    private Unit targetBase;

    private Unit target;

    private Rigidbody _rigidbody;

    private Collider _collider;

    private Collider[] detectionArea = new Collider[50];

    private float targetDistance;

    private bool canAttack = true;

    private int unitLayerMask;

    private IAttackBehaviour attackBehaviour;

    public Faction Faction => faction;
    public UnitData UnitData => _unitData;

    public void Init(UnitData data)
    {
        _unitData = data;

        if (Object.InputAuthority.PlayerId == 1)
        {
            targetBase = GameManager.Instance.PlayerBase[1];
            faction = Faction.Player1;
        }
        else
        {
            targetBase = GameManager.Instance.PlayerBase[0];
            faction = Faction.Player2;
        }
    }

    private void Awake()
    {
        unitLayerMask = LayerMask.GetMask("Unit");
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
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
        currentHP = _unitData.MaxHP;
        Collider[] spawnArea = Physics.OverlapSphere(transform.position, 1.2f, unitLayerMask);

        foreach (var unit in spawnArea)
        {
            if (unit != this)
            {
                if (unit.TryGetComponent(out Rigidbody targetRigidbody))
                {
                    Vector3 direction = targetRigidbody.transform.position - transform.position;
                    direction.y = 0;
                    direction.Normalize();
                    targetRigidbody.AddForce(direction * 7, ForceMode.Impulse);
                }
            }
        }
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
            {
                Debug.Log("누른");
                if (data.Buttons.IsSet(NetworkInputData.SpawnButton))
                {

                }
            }
        }

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
        if (!Object.HasStateAuthority)
            return;

        if (canAttack)
        {
            attackCooldown = TickTimer.CreateFromSeconds(Runner, _unitData.AttackDelay);
            canAttack = false;
            attackBehaviour.Attack(this, target);
        }
    }

    private void UpdateHPBar()
    {
        if (hpBar == null || _unitData == null)
            return;

        hpBar.value = (float)currentHP / _unitData.MaxHP;
    }

    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority)
            return;

        currentHP -= damage;

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
        {
            _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, target.transform.position, _unitData.MoveSpeed * Runner.DeltaTime));
            //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _unitData.MoveSpeed * Runner.DeltaTime);
        }
        else
        {
            _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, targetBase.transform.position, _unitData.MoveSpeed * Runner.DeltaTime));
            //transform.position = Vector3.MoveTowards(transform.position, targetBase.transform.position, _unitData.MoveSpeed * Runner.DeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (other.gameObject.CompareTag("Ground"))
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            
            spawnDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);

            _collider.isTrigger = false;
        }
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
