using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    private CharacterBase characterBase;
    private State state;
    private Vector3 dashTargetPosition;
    private Action onDashComplete;
    private HealthSystem healthSystem;
    private Vector3 basePosition;
    private bool isPlayerTeam;

    private enum State {
        Idle,
        Dashing,
        Busy
    }

    private void Awake(){
        characterBase = GetComponent<CharacterBase>();
    }

    public void Setup(bool isPlayerTeam, AnimatorOverrideController animator){

        characterBase.SetAnimatorController(animator);
        if (isPlayerTeam){
            characterBase.PlayIdleAnim(Vector3.right);
        }
        else {
            characterBase.PlayIdleAnim(Vector3.left);
        }

        this.isPlayerTeam = isPlayerTeam;
        basePosition = GetPosition();
        
        healthSystem = new HealthSystem(100);
    }

    private void Update(){
        switch(state){
        case State.Idle:
            // Invert the GameObject's scale on the X axis to flip it
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(-basePosition.x);
            transform.localScale = localScale;
            break;
        case State.Busy:
            break;
        case State.Dashing:
            float dashSpeed = 10f;
            transform.position += (dashTargetPosition - GetPosition()) * dashSpeed * Time.deltaTime;

            float reachedDistance = 1f;
            if (Vector3.Distance(GetPosition(), dashTargetPosition) < reachedDistance){
                // Arrived at Slide Target
                transform.position = dashTargetPosition;
                onDashComplete();
            }
            break;
        }
    }

    public Vector3 GetPosition(){
        return transform.position;
    } 

    public Vector3 GetBasePosition(){
        return basePosition;
    }

    public bool GetIsPlayerTeam(){
        return isPlayerTeam;
    }

    public bool GetIsDead(){
        return healthSystem.IsDead();
    }

    public void Damage(int damageAmount){
        healthSystem.Damage(damageAmount);
        Debug.Log("Ouch: " + healthSystem.GetHealthAmount());
        characterBase.PlayHitAnim(() => {
            if (healthSystem.IsDead()){
                characterBase.PlayDeathAnim();
            }
            else {
                characterBase.PlayIdleAnim(Vector3.zero);
            }
        });
    }

    public void Buff(int buffAmount){
        Debug.Log("Wahu: " + buffAmount);
    }

    public void Melee_Attack1(List<CharacterBattle> targetCharacterBattles, 
                                int damageAmount,
                                Action onAttackComplete) {

        state = State.Busy;
        Vector3 attackDir = (targetCharacterBattles[0].GetPosition() - GetPosition()).normalized;
        characterBase.PlayMeleeAttack1Anim(attackDir, () => {
        // Target Hit
        foreach (CharacterBattle characterBattle in targetCharacterBattles){
            characterBattle.Damage(damageAmount);
        }
        }, () => {
            characterBase.PlayIdleAnim(attackDir);
            onAttackComplete();
        });
    }

    public void Ranged_Attack1(List<CharacterBattle> targetCharacterBattles, 
                                int damageAmount,
                                Action onAttackComplete) {

        state = State.Busy;
        Vector3 attackDir = (targetCharacterBattles[0].GetPosition() - GetPosition()).normalized;
        characterBase.PlayRangedAttack1Anim(attackDir, () => {
        // Target Hit
        foreach (CharacterBattle characterBattle in targetCharacterBattles){
            characterBattle.Damage(damageAmount);
        }
        }, () => {
            characterBase.PlayIdleAnim(attackDir);
            onAttackComplete();
        });
    }

    public void Buff(List<CharacterBattle> targetCharacterBattles, 
                        int buffAmount,
                        Action onAnimationComplete){
                            
        state = State.Busy;
        characterBase.PlayBuffAnim(() => {
        // Target Hit
        foreach (CharacterBattle characterBattle in targetCharacterBattles){
            characterBattle.Buff(buffAmount);
        }
        }, () => {
            characterBase.PlayIdleAnim(Vector3.zero);
            onAnimationComplete();
        });
    }

    public void DashToPosition(Vector3 dashTargetPosition, Action onDashComplete){
        this.dashTargetPosition = dashTargetPosition;
        this.onDashComplete = onDashComplete;
        state = State.Dashing;
        characterBase.PlayDashAnim(dashTargetPosition - GetPosition());
    }

    public void SetToIdleState(){
        state = State.Idle;
        characterBase.PlayIdleAnim(Vector3.zero);
    }
}
