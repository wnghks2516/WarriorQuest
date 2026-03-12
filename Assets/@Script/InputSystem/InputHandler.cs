using System;
using UnityEngine;
using UnityEngine.InputSystem;



/* 스텍 (Stack )
 * 힙(Heap)
 * 
 * 스텍은 함수의 매개변수, 지역 변수, 리턴 주소 등을 저장하는 메모리 영역입니다.
 * 스텍은 LIFO(Last In First Out) 방식으로 작동하며, 
 * 함수가 호출될 때마다 새로운 스텍 프레임이 생성되고,
 * 함수가 종료되면 해당 프레임이 제거됩니다. 
 * 스텍은 빠른 접근 속도를 제공하지만, 크기가 제한되어 있습니다.
 * 스텍의 예시 = int, float, bool, char, struct 등과 같은 값 형식 (Value Type) 
 * 
 * 
 * 힙이란 동적으로 할당되는 메모리 영역입니다.
 * 힙의 예시 = Class 
 */



namespace WarriorQuest.InputSystem
{

    public class InputHandler : MonoBehaviour
    {
        //inputsystem_Action의 인스턴스
        private InputSystem_Actions inputSystemAction;

        // 액션 참조 변수
        private InputAction MoveAction;
        private InputAction AttackAction;
        private InputAction IntractAction;


        // 이벤트 선언

        public static event Action<Vector2> OnMoveAction;
        public static event Action OnAttackAction;
        public static event Action<bool> OnIntractAction;


        #region 유니티 생명주기 함수
        private void Awake()
        {
            inputSystemAction = new InputSystem_Actions();

            MoveAction = inputSystemAction.Player.Move;
            AttackAction  = inputSystemAction.Player.Attack;
            IntractAction = inputSystemAction.Player.Interact;
        }
        private void OnEnable()
        {
            //입력시스템을 활성화

            //performed = 입력이 발생했을 때 실행되는 이벤트
            //Started = 입력이 시작될 때 실행되는 이벤트
            //Canceled = 입력이 취소될 때 실행되는 이벤트
            inputSystemAction.Enable();
            MoveAction.performed += OnMove;
            MoveAction.canceled += OnMove;


            AttackAction.performed += OnAttack;

            IntractAction.performed += OnIntract;
            IntractAction.canceled += OnIntract;
        }


        private void OnDisable()
        {
            //입력시스템을 비활성화
            inputSystemAction.Disable();
            MoveAction.performed -= OnMove;
            MoveAction.canceled -= OnMove;

            AttackAction.performed -= OnAttack;

            IntractAction.performed -= OnIntract;
            IntractAction.canceled -= OnIntract;
        }
        #endregion

        //콜백 매서드 - 입력이 발생했을 때 실행되는 매서드
        #region 콜백 매서드
        private void OnMove(InputAction.CallbackContext context)
        {
            OnMoveAction?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            
        }

        private void OnIntract(InputAction.CallbackContext context)
        {
           

            if (context.phase == InputActionPhase.Performed)
            {
                OnIntractAction?.Invoke(true);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                OnIntractAction?.Invoke(false);
            }
        }


        #endregion

    }

}