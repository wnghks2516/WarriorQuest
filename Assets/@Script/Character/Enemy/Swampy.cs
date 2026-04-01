using System;
using System.Collections.Generic;
using UnityEngine;
using WarriorQuest.Character.Enemy;
using WarriorQuest.Character.Enemy.FSM;

public class Swampy : Enemy
{
    protected override void InitStates()
    {
        states = new Dictionary<Type, IState>
        {
            [typeof(IdleState)] = new IdleState(),
            [typeof(ChaseState)] = new ChaseState(),
            [typeof(AttackState)] = new AttackState(),
        };

        Debug.Log("스왐피 상태 초기화 완료");
    }

    
}
