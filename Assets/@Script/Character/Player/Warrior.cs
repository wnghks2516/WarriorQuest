using UnityEngine;


namespace WarriorQuest.Character.Player
{

    public class Warrior : Player
    {
        protected override void Attack()
        {
            Debug.Log("Warrior Attack!");
        }
    }
}
