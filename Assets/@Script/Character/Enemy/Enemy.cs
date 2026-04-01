using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WarriorQuest.Character.Enemy.FSM;

namespace WarriorQuest.Character.Enemy
{

    public abstract class Enemy : MonoBehaviour
    {
        //상태 머신 변수

        protected StateMachine stateMachine;

        //상태 머신 프로퍼티
        public StateMachine StateMachine => stateMachine;

        //현재 상태를 표기
        public string currentStatName => stateMachine?.currentState?.GetType().Name ?? "None";

        //상태 전환 매서드
        public void ChangeState<T>() where T : IState
        {
            //딕셔너리에서 상태를 가져와 상태 전환
            // out 이란 TryGetValue 메서드가 상태를 찾았는지 여부를 나타내는 bool 값을 반환하는 키워드
            if(states.TryGetValue(typeof(T), out IState state))
            {
                stateMachine.ChangeState(state);
            }
        }

        //컴포넌트 캐싱
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        //애니메이션 해시 추출
        protected static readonly int hashIsMoving = Animator.StringToHash("IsMoving");
        protected static readonly int hashHit = Animator.StringToHash("Hit");



        //상태를 저장할 딕셔너리 선언
        protected Dictionary<Type, IState> states;

        //상태 초기화 추상 매서드
        protected abstract void InitStates();


        #region 유니티 생명주기

        protected virtual void Awake()
        {
            //상태 초기화 호출
            InitStates();

            //컴포넌트 캐싱
            InitComponents();
        }
        protected void Start()
        {
            //상태 머신 초기화
            stateMachine = new StateMachine(this);
            //초기 상태 설정 (예: IdleState)
            ChangeState<IdleState>();

        }

        private void Update()
        {
            stateMachine.Update();
            //테스트 코드
            TestFSM();

        }

        #endregion

        #region 초기화 매서드

        private void InitComponents()
        {
            rb= GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }


        #endregion

        #region 테스트 코드
        private void TestFSM()
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChangeState<IdleState>();
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ChangeState<ChaseState>();
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ChangeState<AttackState>();
            }

        }
        #endregion
    }
}