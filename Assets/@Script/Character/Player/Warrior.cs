using UnityEngine;


namespace WarriorQuest.Character.Player
{

    public class Warrior : Player
    {
        [Header("전사 전용 스탯")]
        [SerializeField] private WarriorSO warriorSO;


        [SerializeField] private float Defense = 10f;

        #region 유니티 생명주기
        protected override void Awake()
        {
            base.Awake();
            MaxHP = warriorSO.maxHP;
            MoveSpeed = warriorSO.moveSpeed;
            AttackDamage = warriorSO.attackDamage;
            AttackCooldown = warriorSO.attackCooldown;
            CurrentHP = MaxHP;

            Debug.Log($"Warrior Awake! 방어력 :{warriorSO.defense} ");
        }
        #endregion


        #region 공격 및 데미지 처리

        protected override void Attack()
        {
            Debug.Log("Warrior Attack!");
        }

        //애니메이션의 이벤트에서 호출할 매서드
        public void OnAttackAnimEvent()
        {
            //실제 공격 처리 로직
            Debug.Log("Warrior OnAttackAnimEvent! 실제 공격 처리 로직 실행");
        }

        #endregion
        public override void TakeDamage(float damage)
        {
            //방어력 적용
            float ActualDamage = Mathf.Max(damage - warriorSO.defense, 5f); // 방어력을 고려한 실제 데미지 계산
            Debug.Log($"Warrior takes {ActualDamage} damage after defense!");
            base.TakeDamage(ActualDamage);
        }
    }
}
