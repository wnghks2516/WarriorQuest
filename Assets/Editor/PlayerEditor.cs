using UnityEditor;
using UnityEngine;
using WarriorQuest.Character.Player;

[CustomEditor(typeof(Player),true)]
public class  PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //적용대상 클래스를 가져오기
        Player player = (Player)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("테스트 기능", EditorStyles.boldLabel);

        var damage = EditorGUILayout.FloatField("공격 데미지", player.GetAttackDamage);

        if (GUILayout.Button($"피해 입히기 {damage}"))
        {
            player.TakeDamage(damage);
        }
    }
}
