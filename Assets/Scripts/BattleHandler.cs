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
    public TeamHandler playerTeam;
    public TeamHandler enemyTeam;    

    private CharacterBattle[] characterBattles = new CharacterBattle[6];

    private CharacterBattle activeCharacterBattle;
    private CharacterBattle selectedCharacterBattle;
    private List<CharacterBattle> targetCharacterBattles = new List<CharacterBattle>();
    private int characterBattleIndex;
    private State state;

    public Card card;

    private enum State {
        WaitingForPlayer,
        Busy,
    }


    private void Start(){

        for (int i = 0; i < 6; i++) {
            characterBattles[i] = SpawnCharacter(i);
        }

        characterBattleIndex = 0;
        SetActiveCharacterBattle(characterBattles[characterBattleIndex]);
        selectedCharacterBattle = characterBattles[3];
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
                PlayCardSequence(card);
            }
        }
    }

    private CharacterBattle SpawnCharacter(int placement){
        Vector3 position;
        bool isPlayerTeam;
        AnimatorOverrideController animator;

        switch (placement){
            case 0:
                if (playerTeam.member1) {
                    position = new Vector3(-2f, 1f);
                    isPlayerTeam = true;
                    animator = playerTeam.member1;
                } 
                else {
                    return null;
                }
                break;
            case 1:
                if (playerTeam.member2) {
                    position = new Vector3(-2f, -1f);
                    isPlayerTeam = true;
                    animator = playerTeam.member2;
                } 
                else {
                    return null;
                }
                break;
            case 2:
                if (playerTeam.member3) {
                    position = new Vector3(-5.5f, 0f);
                    isPlayerTeam = true;
                    animator = playerTeam.member3;
                }
                else {
                    return null;
                }
                break;
            case 3:
                if (enemyTeam.member1) {
                    position = new Vector3(2f, 1f);
                    isPlayerTeam = false;
                    animator = enemyTeam.member1;
                }
                else {
                    return null;
                }
                break;
            case 4:
                if (enemyTeam.member2) {
                    position = new Vector3(2f, -1f);
                    isPlayerTeam = false;
                    animator = enemyTeam.member2;
                }
                else {
                    return null;
                }
                break;
            case 5:
                if (enemyTeam.member3) {
                    position = new Vector3(5.5f, 0f);
                    isPlayerTeam = false;
                    animator = enemyTeam.member3;
                }
                else {
                    return null;
                }
                break;
            default:
                return null;
        }

        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.Setup(isPlayerTeam, animator);

        return characterBattle;
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle){
        if (activeCharacterBattle != null) {
            activeCharacterBattle.HideSelectionCircle();
        }
        activeCharacterBattle = characterBattle;
        characterBattle.ShowSelectionCircle();
    }

    private bool TestBattleOver(){
        bool gameEnd = true;
        for (int i = 0; i < 3; i++) {
            if (characterBattles[i] && !characterBattles[i].GetIsDead()){
                gameEnd = false;
            }
        }
        if (gameEnd) {
            Debug.Log("LOSE");
            return true;
        }

        gameEnd = true;
        for (int i = 3; i < 6; i++) {
            if (characterBattles[i] && !characterBattles[i].GetIsDead()){
                gameEnd = false;
            }
        }
        if (gameEnd) {
            Debug.Log("WIN");
            return true;
        }
        return false;
    }

    private void ChooseNextActiveCharacter(){

        if (TestBattleOver()){
            return;
        }

        characterBattleIndex += 1;
        if (characterBattleIndex >= 6){
            characterBattleIndex = 0;
        }
        while (!characterBattles[characterBattleIndex] || characterBattles[characterBattleIndex].GetIsDead()){
            characterBattleIndex += 1;
            if (characterBattleIndex >= 6){
                characterBattleIndex = 0;
            }
        }

        SetActiveCharacterBattle(characterBattles[characterBattleIndex]);
        if (characterBattles[characterBattleIndex].GetIsPlayerTeam()){
            state = State.WaitingForPlayer;
            selectedCharacterBattle = characterBattles[3];
        }
        else {
            state = State.Busy;
            selectedCharacterBattle = characterBattles[0];
            PlayCardSequence(card);
        }
    }

    private void PlayCardSequence(Card card){
        targetCharacterBattles.Clear();
        switch (card.target){
            case Card.Target.SingleTarget:
                targetCharacterBattles.Add(selectedCharacterBattle);
                break;
            case Card.Target.AllAllies:
                if (activeCharacterBattle.GetIsPlayerTeam()){
                    for (int i = 0; i < 3; i++) {
                        if (characterBattles[i]){
                            targetCharacterBattles.Add(characterBattles[i]);
                        }
                    }
                }
                else {
                    for (int i = 3; i < 6; i++) {
                        if (characterBattles[i]){
                            targetCharacterBattles.Add(characterBattles[i]);
                        }
                    }
                }
                break;
            case Card.Target.AllEnemies:
                if (activeCharacterBattle.GetIsPlayerTeam()){
                    for (int i = 3; i < 6; i++) {
                        if (characterBattles[i]){
                            targetCharacterBattles.Add(characterBattles[i]);
                        }
                    }
                }
                else {
                    for (int i = 0; i < 3; i++) {
                        if (characterBattles[i]){
                            targetCharacterBattles.Add(characterBattles[i]);
                        }
                    }
                }
                break;
            case Card.Target.Self:
                targetCharacterBattles.Add(activeCharacterBattle);
                break;
        }
        PerformAction(card, 0);
    }

    private void PerformAction(Card card, int action){
        if  (action >= card.actionSequence.Count) {
            activeCharacterBattle.DashToPosition(activeCharacterBattle.GetBasePosition(), () => {      
                activeCharacterBattle.SetToIdleState();
                ChooseNextActiveCharacter();
            });
            return;
        }

        switch (card.actionSequence[action]){
            case Card.Action.Melee_Attack1:
                activeCharacterBattle.Melee_Attack1(targetCharacterBattles, 
                                                    card.damageAmount, 
                                                    () => {

                    PerformAction(card, action + 1);
                });
                break;
            case Card.Action.Ranged_Attack1:
                activeCharacterBattle.Ranged_Attack1(targetCharacterBattles, 
                                                    card.damageAmount,
                                                    () => {

                    PerformAction(card, action + 1);
                });
                break;
            case Card.Action.Buff:
                activeCharacterBattle.Buff(targetCharacterBattles, 
                                            card.damageAmount,
                                            () => {

                    PerformAction(card, action + 1);
                });
                break;
            case Card.Action.Dash:
                if (card.target == Card.Target.SingleTarget) {
                    activeCharacterBattle.DashToPosition(targetCharacterBattles[0].GetPosition() 
                        - new Vector3 (((targetCharacterBattles[0].GetPosition() 
                                        - activeCharacterBattle.GetPosition()).normalized 
                                        * 2.5f).x, 0, 0), () => {
                                                PerformAction(card, action + 1);
                    });
                }
                else {
                    activeCharacterBattle.DashToPosition(Vector3.zero , () => {
                        PerformAction(card, action + 1);
                    });
                }
                break;
            case Card.Action.Return:
                activeCharacterBattle.DashToPosition(activeCharacterBattle.GetBasePosition(), () => {
                    PerformAction(card, action + 1);
                });
                break;
        }
    }
}
