using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    private Animator animator;
    private Action onAnimationComplete;
    private Action onHit;
    private Action onBuff;

    private void Awake(){
        animator = GetComponent<Animator>();
    }

    public void SetAnimatorController(AnimatorOverrideController newAnimatorController){
        animator.runtimeAnimatorController = newAnimatorController;
    }

    public void PlayMeleeAttack1Anim(Vector3 dir, Action onHit, Action onAnimationComplete) {
        animator.Play("Melee_Attack");

        // Invert the GameObject's scale on the X axis to flip it
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(dir.x);
        transform.localScale = localScale;

        this.onHit = onHit;
        this.onAnimationComplete = onAnimationComplete;
    }

    public void PlayRangedAttack1Anim(Vector3 dir, Action onHit, Action onAnimationComplete) {
        animator.Play("Ranged_Attack");

        // Invert the GameObject's scale on the X axis to flip it
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(dir.x);
        transform.localScale = localScale;

        this.onHit = onHit;
        this.onAnimationComplete = onAnimationComplete;
    }

    public void PlayBuffAnim(Action onBuff, Action onAnimationComplete){
        animator.Play("Buff");

        this.onBuff = onBuff;
        this.onAnimationComplete = onAnimationComplete;
    }

    public void PlayHitAnim(Action onAnimationComplete){
        animator.Play("Hit");

        this.onAnimationComplete = onAnimationComplete;
    }

    public void PlayIdleAnim(Vector3 dir) {
        animator.Play("Idle");

        // Invert the GameObject's scale on the X axis to flip it
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(dir.x);
        transform.localScale = localScale;
    }

    public void PlayDashAnim(Vector3 dir) {
        animator.Play("Dash");

        // Invert the GameObject's scale on the X axis to flip it
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(dir.x);
        transform.localScale = localScale;
    }

    public void PlayDeathAnim(){
        animator.Play("Death");
    }

    public void AnimationComplete(){
        onAnimationComplete();
    }

    public void AnimationHit(){
        onHit();
        CameraShake.Instance.ShakeCamera(0.1f, 0.1f);
    }

    public void AnimationBuff(){
        onBuff();
    }
}
