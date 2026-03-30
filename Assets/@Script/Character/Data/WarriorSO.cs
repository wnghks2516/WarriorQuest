using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "WarriorSO", menuName = "Warrior/WarriorSO", order = 1)]
public class WarriorSO : UnityEngine.ScriptableObject
{
    [Header("전사 전용 능력치")]
    public float maxHP = 150f;
    public float moveSpeed = 5f;
    public float attackDamage = 25f;
    public float attackCooldown = .7f;
    public float defense = 10f;

}