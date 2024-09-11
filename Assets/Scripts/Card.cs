using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite artwork;
    public new string name;
    public string description;
    public int damageAmount;
    public int buffAmount;

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
        Buff,
        Dash,
        Return,
    }

    public List<Action> actionSequence;
}