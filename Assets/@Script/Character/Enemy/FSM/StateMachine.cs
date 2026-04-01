namespace WarriorQuest.Character.Enemy.FSM
{
    public class StateMachine
    {
        private Enemy enemy;

        //생성자
        public StateMachine(Enemy enemy)
        {
            this.enemy = enemy;
        }

        //현재 상태를 저장하는 변수
        public IState currentState;

        //상태 전환 매서드
        public void ChangeState ( IState newState )
        {
            currentState?.OnExit(enemy);
            currentState = newState;
            currentState.OnEnter(enemy);

        }

        //상태 업데이트 매서드
        public void Update()
        {
            currentState?.OnUpdate(enemy);

        }
    }
}
