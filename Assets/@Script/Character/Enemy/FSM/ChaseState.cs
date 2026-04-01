using UnityEngine;

namespace WarriorQuest.Character.Enemy.FSM
{
    public class ChaseState : IState
    {

        public void OnEnter(Enemy enemy)
        {
            Debug.Log("Chase 진입");
        }
        public void OnUpdate(Enemy enemy)
        {
            Debug.Log("Chase 진행");
        }
        public void OnExit(Enemy enemy)
        {
            Debug.Log("Chase 종료");
        }
    }
}