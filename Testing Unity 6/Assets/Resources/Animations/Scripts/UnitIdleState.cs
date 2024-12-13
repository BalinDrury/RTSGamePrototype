using Unity.Collections;
using UnityEngine;

public class UnitIdleState : StateMachineBehaviour
{
    AttackController attackController;
    public Animator animator;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        attackController = animator.transform.GetComponent<AttackController>();

        attackController.SetIdleMaterial();

        animator.SetBool("isFollowing", true);
        animator.SetBool("isAttacking", false);


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if there is an available target
        if (attackController.targetToAttack != null)
        {
            // Transition to the Follwing State
            animator.SetBool("isFollowing", true);
        }

    }
}