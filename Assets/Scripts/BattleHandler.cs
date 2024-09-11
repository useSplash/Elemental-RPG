using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    private static BattleHandler instance;
    public static BattleHandler GetInstance() {
        return instance;
    }
    [SerializeField] private Transform pfCharacterBattle;
    public AnimatorOverrideController playerAnimatorController;
    public AnimatorOverrideController enemyAnimatorController;

    private CharacterBattle playerCharacterBattle;
    private CharacterBattle enemyCharacterBattle;
    private CharacterBattle activeCharacterBattle;
    private State state;

    public Card card;

    private enum State {
        WaitingForPlayer,
        Busy,
    }


    private void Start(){
        playerCharacterBattle = SpawnCharacter(true);
        enemyCharacterBattle = SpawnCharacter(false);

        SetActiveCharacterBattle(playerCharacterBattle);
        state = State.WaitingForPlayer;
    }

    private void Awake(){
        instance = this;
    }

    private void Update(){
        if (state == State.WaitingForPlayer) {
            if (Input.GetKeyDown(KeyCode.Space)){
                // Attack
                state = State.Busy;
                // playerCharacterBattle.MeleeAttack(enemyCharacterBattle, () => {
                //     ChooseNextActiveCharacter();
                // });
                PlayCardSequence(card);
            }
        }
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam){
        Vector3 position;
        if (isPlayerTeam){
            position = new Vector3(-4, 1);
        }
        else {
            position = new Vector3(+4, 1);
        }
        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.Setup(isPlayerTeam);

        return characterBattle;
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle){
        activeCharacterBattle = characterBattle;
    }

    private void ChooseNextActiveCharacter(){
        if (activeCharacterBattle == playerCharacterBattle){
            SetActiveCharacterBattle(enemyCharacterBattle);
            state = State.Busy;

            // Enemy Attacks Player
            // enemyCharacterBattle.MeleeAttack(playerCharacterBattle, () => {
            ChooseNextActiveCharacter();
            // });
        }
        else {
            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WaitingForPlayer;
        }
    }

    private void PlayCardSequence(Card card){
        CharacterBattle targetCharacterBattle;
        if (card.target == Card.Target.SingleTarget){
            targetCharacterBattle = enemyCharacterBattle;
        }
        else {
            targetCharacterBattle = null;
        }
        PerformAction(card, 0, targetCharacterBattle);
    }

    private void PerformAction(Card card, int action, CharacterBattle targetCharacterBattle){
        if  (action >= card.actionSequence.Count) {
            activeCharacterBattle.SetToIdleState();
            ChooseNextActiveCharacter();
            return;
        }

        switch (card.actionSequence[action]){
            case (Card.Action.Melee_Attack1):
                activeCharacterBattle.Melee_Attack1(targetCharacterBattle, () => {
                    PerformAction(card, action + 1, targetCharacterBattle);
                });
                break;
            case (Card.Action.Ranged_Attack1):
                activeCharacterBattle.Ranged_Attack1(targetCharacterBattle, () => {
                    PerformAction(card, action + 1, targetCharacterBattle);
                });
                break;
            case (Card.Action.Dash):
                activeCharacterBattle.DashToPosition(targetCharacterBattle.GetPosition() - (targetCharacterBattle.GetPosition() - playerCharacterBattle.GetPosition()).normalized * 2.5f, () => {
                    PerformAction(card, action + 1, targetCharacterBattle);
                });
                break;
            case (Card.Action.Return):
                activeCharacterBattle.DashToPosition(activeCharacterBattle.GetBasePosition(), () => {
                    PerformAction(card, action + 1, targetCharacterBattle);
                });
                break;
        }
    }
}
