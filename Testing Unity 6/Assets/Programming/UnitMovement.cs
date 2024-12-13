using System.Dynamic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;
    Animator animator;

    public bool isCommandedToMove;

    private UnitSelectionManager UnitSelectionManager;


    // public UnitSelectionManager script;

    // public TagHandle SelectionManagerTag;
    // public string tagnameSelectionManage = "selectionManager";
    //GameObject[] SelectionManager;

    private void Start()
    {

        UnitSelectionManager = FindAnyObjectByType<UnitSelectionManager>(); ;
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && ! UnitSelectionManager.isEnemyHovered())
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                
                isCommandedToMove = true;
                agent.SetDestination(hit.point);
                UnitSelectionManager.Instance.PlayRandomMovementClip();

                //if (animator.GetBool("isAttacking") == true)
                //{
                //    animator.SetBool("isAttacking", false); // Move to Follow State
                //    animator.SetBool("isFollowing", true);
                //}
                //Debug.Log("Moving!");

            }
        }

        if (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        {
            isCommandedToMove = false;
        }

    }

}
