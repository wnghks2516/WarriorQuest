using UnityEngine;

namespace WarriorQuest.Character.Enemy.FSM
{
    public class AttackState : IState
    {
        

        public void OnEnter(Enemy enemy)
        {
            Debug.Log("Attack 진입");
        }
        public void OnUpdate(Enemy enemy)
        {
            Debug.Log("Attack 진행");
        }
        public void OnExit(Enemy enemy)
        {
            Debug.Log("Attack 종료");
        }
    }
}