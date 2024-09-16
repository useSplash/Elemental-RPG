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
    private List<CharacterBattle> validTargetCharacterBattles = new List<CharacterBattle>();
    private CharacterBattle playerSelectedCharacterBattle;
    private int characterBattleIndex;

    private State state;
    private List<Card> cardSequence = new List<Card>();

    private enum State {
        WaitingForPlayer,
        Busy,
    }


    private void Start(){

        for (int i = 0; i < 6; i++) {
            characterBattles[i] = SpawnCharacter(i);
        }

        ChooseNextActiveCharacter();
        CardSequenceDisplay.Instance.SetCardText(cardSequence);
    }

    private void Awake(){
        instance = this;
    }

    private void Update(){
        if (state == State.WaitingForPlayer) {
            if (Input.GetKeyDown(KeyCode.Space)){
                // Attack
                state = State.Busy;
                if (cardSequence.Count >= 1){
                    PlayCardActionSequence(cardSequence[0]);
                }
                else {
                    ChooseNextActiveCharacter();
                }
            }
            if (Input.GetKeyDown(KeyCode.R)){
                // Reset Chosen Cards
                ResetCardSequence();
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

        if (activeCharacterBattle != null){
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
        }
        else { 
            characterBattleIndex = 0;
        }

        SetActiveCharacterBattle(characterBattles[characterBattleIndex]);
        validTargetCharacterBattles.Clear();
        if (characterBattles[characterBattleIndex].GetIsPlayerTeam()){
            for (int i = 3; i < 6; i++) {
                if (characterBattles[i] && !characterBattles[i].GetIsDead()){
                    validTargetCharacterBattles.Add(characterBattles[i]);
                }
            }
            state = State.WaitingForPlayer;

            // Swap target if invalid
            if (validTargetCharacterBattles.Contains(playerSelectedCharacterBattle)){
                selectedCharacterBattle = playerSelectedCharacterBattle;
            }
            else {
                selectedCharacterBattle = validTargetCharacterBattles[0];
            }
            selectedCharacterBattle.ShowTargetIndicator();
        }
        else {
            for (int i = 0; i < 3; i++) {
                if (characterBattles[i] && !characterBattles[i].GetIsDead()){
                    validTargetCharacterBattles.Add(characterBattles[i]);
                }
            }
            state = State.Busy;
            selectedCharacterBattle = validTargetCharacterBattles[0];
            if (cardSequence[0] != null){
                PlayCardActionSequence(cardSequence[0]);
            }
            else {
                ChooseNextActiveCharacter();
            }
        }
    }

    private void PlayCardActionSequence(Card card){
        targetCharacterBattles.Clear();
        selectedCharacterBattle.HideTargetIndicator();

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

    private void PerformAction(Card card, int actionIndex){
        if  (actionIndex >= card.actionSequence.Count) {
            cardSequence.RemoveAt(0);
            CardSequenceDisplay.Instance.SetCardText(cardSequence);
            if (cardSequence.Count <= 0){
                activeCharacterBattle.DashToPosition(activeCharacterBattle.GetBasePosition(), () => {      
                    activeCharacterBattle.SetToIdleState();
                    ChooseNextActiveCharacter();
                });
            }
            else {
                PlayCardActionSequence(cardSequence[0]);
            }
            return;
        }

        // Debug.Log(card.name + ": " + card.actionSequence[actionIndex]); 

        switch (card.actionSequence[actionIndex]){
            case Card.Action.Melee_Attack1:
                activeCharacterBattle.Melee_Attack1(targetCharacterBattles, 
                                                    card.damageAmount, 
                                                    () => {

                    PerformAction(card, actionIndex + 1);
                });
                break;
            case Card.Action.Ranged_Attack1:
                activeCharacterBattle.Ranged_Attack1(targetCharacterBattles, 
                                                    card.damageAmount,
                                                    () => {

                    PerformAction(card, actionIndex + 1);
                });
                break;
            case Card.Action.Buff:
                activeCharacterBattle.BuffTarget(targetCharacterBattles, 
                                            card.buffAmount,
                                            () => {

                    PerformAction(card, actionIndex + 1);
                });
                break;
            case Card.Action.Dash:
                Vector3 dashPosition;

                if (card.target == Card.Target.SingleTarget) {
                    dashPosition = targetCharacterBattles[0].GetPosition() 
                        - new Vector3 (((targetCharacterBattles[0].GetPosition() 
                                        - activeCharacterBattle.GetPosition()).normalized 
                                        * 2.5f).x, 0, 0);
                }
                else {
                    dashPosition = Vector3.zero;
                }

                if (Vector3.Distance(activeCharacterBattle.GetPosition(), dashPosition) > 0.1f){
                    activeCharacterBattle.DashToPosition(dashPosition, () => {
                                                PerformAction(card, actionIndex + 1);
                    });
                }
                else {
                    PerformAction(card, actionIndex + 1);
                }
                break;
            case Card.Action.Return:
                activeCharacterBattle.DashToPosition(activeCharacterBattle.GetBasePosition(), () => {
                    PerformAction(card, actionIndex + 1);
                });
                break;
        }
    }

    public void SelectCharacterBattle(CharacterBattle characterBattle){
        if (state == State.WaitingForPlayer && validTargetCharacterBattles.Contains(characterBattle)){
            // Hide old indicator
            if (selectedCharacterBattle){
                selectedCharacterBattle.HideTargetIndicator();
            }

            // Select new and show indicator
            selectedCharacterBattle = characterBattle;
            selectedCharacterBattle.ShowTargetIndicator();

            // To save player choice of target
            playerSelectedCharacterBattle = selectedCharacterBattle;
        }
    }

    public void AddToCardSequence(Card card){
        if (state == State.WaitingForPlayer){
            cardSequence.Add(card);
            CardSequenceDisplay.Instance.SetCardText(cardSequence);
        }
    }

    public void ResetCardSequence(){
        if (state == State.WaitingForPlayer){
            cardSequence.Clear();
            CardSequenceDisplay.Instance.SetCardText(cardSequence);
        }
    }
}
