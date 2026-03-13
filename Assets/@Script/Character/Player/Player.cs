using System;
using UnityEngine;
using WarriorQuest.Character.Interface;
using WarriorQuest.InputSystem;

namespace WarriorQuest.Character.Player
{
    //자동으로 추가됨
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(InputHandler))]


    public abstract class Player : MonoBehaviour, IDamageable
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
        //Facing 처리를 위한 Weapon Transform
        [NonSerialized] protected Transform ArmTransform;

        //애니메이션 파라미터 해시값을 미리 계산
        protected static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
        protected static readonly int HashAttack = Animator.StringToHash("Attack");
        protected static readonly int HashHit = Animator.StringToHash("Hit");

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
            ArmTransform = transform.Find("Arm");

            //Weapon이 Arm설정
            //ArmTransform = transform.Find("Arm");
        }


        protected void OnEnable()
        {
            //이벤트 구독
            if (!IsDead)
            {
                InputHandler.OnMoveAction += OnMove;
                InputHandler.OnAttackAction += OnAttack;
                InputHandler.OnIntractAction += OnIntract;
            }
        }



        protected void OnDisable()
        {
            //이벤트 구독 해제
            InputHandler.OnMoveAction -= OnMove;
            InputHandler.OnAttackAction -= OnAttack;
            InputHandler.OnIntractAction -= OnIntract;
        }
        #endregion

        #region 공통 매서드

        private void FlipSprite(bool facingRight)
        {
            if (facingRight)
            {
                SpriteRenderer.flipX = false;
                ArmTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                SpriteRenderer.flipX = true;
                ArmTransform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }

        public virtual void TakeDamage(float damage)
        {
            if (IsDead) return;
            CurrentHP -= damage;
            Anim.SetTrigger(HashHit);

            if (CurrentHP <= 0)
            {
                
                Die();
            }
        }

        protected virtual void Die()
        {
            CurrentHP = 0;
            Debug.Log("플레이어 사망");
        }
        #endregion


        #region 입력 처리 매서드
        private void OnMove(Vector2 context)
        { 

            RB.linearVelocity = context.normalized * MoveSpeed;

            //방향 전환
            if(context.x !=0) FlipSprite(context.x > 0);


            //애니메이션 처리
            Anim.SetBool(HashIsMoving, context.sqrMagnitude > 0.01f);

        }
        private void OnAttack()
        {
            Anim.SetTrigger(HashAttack);
            Attack();
        }

        private void OnIntract(bool context)
        {
            Debug.Log("상호작용 입력 감지: " + context);
        }
        #endregion

        #region 추상 매서드
        protected abstract void Attack();

        

        
        #endregion

    }

}