using System;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;

    public float stopAttackingDistance = 1.2f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        attackController.SetAttackMaterial();
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (attackController.targetToAttack != null && animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
        {

            LookAtTarget();

            //// Keep moving towardsenemy
            agent.SetDestination(attackController.targetToAttack.position);

            var damageToInflict = attackController.unitDamage;

            // Actually Attack Unit
            attackController.targetToAttack.GetComponent<Enemy>.ReceiveDamage(damageToInflict);

            // Should Unit still attack
            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

            if (attackController.targetToAttack == null || distanceFromTarget > stopAttackingDistance)
            {
                Debug.LogWarning("Exiting Attack State");
                animator.SetBool("isAttacking", false);
                animator.SetBool("isFollowing", distanceFromTarget <= stopAttackingDistance); // Follow if target is still in range

            }

        }
        else
        {
            animator.SetBool("isAttacking", false);
        }

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isFollowing", false);
        Debug.Log("Exiting UnitAttackState");
    }






    public float lookOffset = 90; // Set this to -90 or 90 for left or right look
    public float rotationSpeed = 10f; // Rotation speed in degrees per second

    private void LookAtTarget()
    {
        // Calculate the direction vector towards the target
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;

        // Get the rotation towards the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Extract the Y rotation from the target rotation
        float targetYRotation = targetRotation.eulerAngles.y;

        // Apply the look offset
        float desiredYRotation = targetYRotation + lookOffset;

        // Get the current Y rotation of the agent
        float currentYRotation = agent.transform.eulerAngles.y;

        // Calculate the shortest rotation direction
        float rotationDifference = Mathf.DeltaAngle(currentYRotation, desiredYRotation);

        // Smoothly adjust the current rotation towards the desired rotation
        float newYRotation = Mathf.MoveTowardsAngle(currentYRotation, desiredYRotation, rotationSpeed * Time.deltaTime);

        // Apply the new rotation to the agent
        agent.transform.rotation = Quaternion.Euler(0, newYRotation, 0);
    }

    //private void LookAtTarget()
    //{
    //    Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
    //    agent.transform.rotation = Quaternion.LookRotation(direction);

    //    var yRotation = agent.transform.eulerAngles.y;
    //    agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    //}

    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}


}
