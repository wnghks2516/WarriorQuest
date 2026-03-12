using UnityEngine;
using WarriorQuest.InputSystem;

namespace WarriorQuest.Character.Player
{
    //자동으로 추가됨
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(InputHandler))]


    public class Player : MonoBehaviour
    {
        #region 기본 스탯
        [Header("기본 스탯")]
        [SerializeField] protected float MaxHP = 100f;
        [SerializeField] protected float CurrentHP = 100f;
        [SerializeField] protected float MoveSpeed = 5f;
        [SerializeField] protected float AttackDamage = 20f;
        [SerializeField] protected float AttackCooldown = .5f;

        protected bool IsDead => CurrentHP <= 0f;
        #endregion

        #region 프로퍼티
        public float GetMaxHP => MaxHP;
        public float GetCurrentHP => CurrentHP;
        public float GetMoveSpeed => MoveSpeed;
        public float GetAttackDamage => AttackDamage;
        public float GetAttackCooldown => AttackCooldown;

        #endregion


        #region 컴포넌트 캐싱

        protected Rigidbody2D RB;
        protected Animator Anim;
        protected SpriteRenderer SpriteRenderer;
        protected InputHandler InputHandler;

        #endregion

        #region 유니티 생명주기 함수

        protected virtual void Awake()
        {
            //초기 체력

            CurrentHP = MaxHP;
            //컴포넌트 캐싱
            RB = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            InputHandler = GetComponent<InputHandler>();
        }

        #endregion
    }
}
