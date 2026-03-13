using UnityEngine;


namespace WarriorQuest.Character.Player
{

    public class Warrior : Player
    {
        [Header("전사 전용 스탯")]
        [SerializeField] private float Defense = 10f;

        #region 유니티 생명주기
        protected override void Awake()
        {
            base.Awake();
            MaxHP = 150f;
            MoveSpeed = 4f;
            AttackDamage = 25f;
            AttackCooldown = .7f;
    
            Debug.Log($"Warrior Awake! 방어력 :{Defense} ");
            
        }
        #endregion


        protected override void Attack()
        {
            Debug.Log("Warrior Attack!");
        }

        public override void TakeDamage(float damage)
        {
            float ActualDamage = Mathf.Max(damage - Defense, 5f); // 방어력을 고려한 실제 데미지 계산
            Debug.Log($"Warrior takes {ActualDamage} damage after defense!");
            base.TakeDamage(damage);
        }
    }
}
