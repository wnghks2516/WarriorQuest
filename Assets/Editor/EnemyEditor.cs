using UnityEngine;
using UnityEditor;
using WarriorQuest.Character.Enemy;
using WarriorQuest.Character.Enemy.FSM;

[CustomEditor(typeof(Enemy),true)]
public class  EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
       Enemy enemy = (Enemy)target;


        //기본 인스펙터 그리기
        DrawDefaultInspector();
        
        GUI.enabled = Application.isPlaying;

        //현재 상태 레이블
        EditorGUILayout.LabelField("현재 상태 : " + enemy.currentStatName);
        EditorGUILayout.Space(50);
       

        if (GUILayout.Button("Idle 상태"))
        {
            enemy.ChangeState<IdleState>();
        }
        if (GUILayout.Button("Chase 상태"))
        {
            enemy.ChangeState<ChaseState>();
        }
        if (GUILayout.Button("공격 상태"))
        {
            enemy.ChangeState<AttackState>();
        }

        GUI.enabled = true;
    }

}