using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Data / UnitData")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string unitName;

    [SerializeField]
    private int maxHP;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float detectionRange;

    [SerializeField]
    private float detectionCenterOffset;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private int attackPower;

    [SerializeField]
    private float attackDelay;

    public string UnitName => unitName;
    public int MaxHP => maxHP;
    public float MoveSpeed => moveSpeed;
    public float DetectionRange => detectionRange;
    public float DetectionCenterOffset => detectionCenterOffset;
    public float AttackRange => attackRange;
    public int AttackPower => attackPower;
    public float AttackDelay => attackDelay;
}
