using UnityEngine;

namespace WarriorQuest.Character.Enemy.FSM
{
    public class IdleState : IState
    {

        public void OnEnter(Enemy enemy)
        {
            Debug.Log("Idle 진입");
        }

        public void OnUpdate(Enemy enemy)
        {
            Debug.Log("Idle 갱신");
        }
        public void OnExit(Enemy enemy)
        {
            Debug.Log("Idle 종료");
        }
    }
}