using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ProjectileData / ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [SerializeField]
    private bool isPiercing;

    [SerializeField]
    private float speed;

    public bool IsPiercing => isPiercing;
    public float Speed => speed;
}
