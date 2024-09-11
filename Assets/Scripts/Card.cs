using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;

    public Target target;

    public enum Target{
        SingleTarget,
        AllAllies,
        AllEnemies,
        Self,
    }

    public enum Action{
        Melee_Attack1,
        Ranged_Attack1,
        Dash,
        Return,
    }

    public List<Action> actionSequence;
}