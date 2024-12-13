using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();
    public List<AudioClip> audioSelectClips = new List<AudioClip>();
    public List<AudioClip> audioMoveClips = new List<AudioClip>();
    public List<AudioClip> audioAttackClips = new List<AudioClip>();


    //references
    public LayerMask clickable;
    public LayerMask ground;
    public TagHandle enemy;
    public GameObject groundMarker;
    public AudioSource audioSource;
    

    public bool attackCursorVisible;

    private static Camera cam;

    



    private void Awake()
    {
        if (Instance != null && Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSelectClips.Count == 0)
        {
            Debug.LogWarning("No audio clips assigned to the list");
        }
        
        cam = Camera.main;
        Debug.Log("Alive");
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            isEnemyHovered();
        }


        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //If we are hitting a clickable object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectByClicking(hit.collider.gameObject);
                }
            }
            else  //If we are NOT hitting a clickable object
            {
                if (Input.GetKey(KeyCode.LeftShift) == false) 
                {
                    DeselectAll();
                }

            }
        }


        if (Input.GetMouseButton(1) && unitsSelected.Count > 0 && !isEnemyHovered())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //If we are hitting a clickable object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                //Debug.Log("Ground here!");
                groundMarker.transform.position = hit.point;

                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }


        // Attack Target

        if (unitsSelected.Count > 0 && AtleastOneOffensiveUnit(unitsSelected))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Debug.Log("Test");

            //If we are hitting a clickable object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                

                attackCursorVisible = true;

                if (Input.GetMouseButton(1))
                {
                   // Debug.Log("Enemy Hovered With Mouse");



                    Transform target = hit.transform;

                    foreach (GameObject unit in unitsSelected)
                    {
                        if (unit.GetComponent<AttackController>())
                        {
                            groundMarker.SetActive(false);

                            unit.GetComponent<AttackController>().targetToAttack = target;

                            PlayRandomAttackClip();
                        }
                    }
                }

            }
            else
            {
                attackCursorVisible = false;
              //  Debug.Log("No Enemy");
            }
        }


        


    }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit.GetComponent<AttackController>())
            {
              //  Debug.Log("Offensive Unit Detected");
                return true;
            }
        }
        return false;
    }

    

    public bool isEnemyHovered()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Enemy"))
            {
                Debug.Log("Hovered");
                return true;
            }
            else
            {
                Debug.Log("NoBadGuy");
                return false;
            }
            
        }
        else
        {
          //  Debug.Log("NoBadGuy");
            return false;

        }




    }

    private void MultiSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            
            unitsSelected.Remove(unit);
            SelectUnit(unit, false);
        }

    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);
        }

        groundMarker.SetActive(false);

        unitsSelected.Clear();
    }

    public void PlayRandomSelectionClip()
    {
        if (audioSelectClips.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, audioSelectClips.Count);
            audioSource.clip = audioSelectClips[randomIndex];
            audioSource.Play();
        }
    }

    public void PlayRandomMovementClip()
    {
        if (audioMoveClips.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, audioMoveClips.Count);
            audioSource.clip = audioMoveClips[randomIndex];
            audioSource.Play();
        }
    }

    public void PlayRandomAttackClip()
    {
        if (audioAttackClips.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, audioAttackClips.Count);
            audioSource.clip = audioAttackClips[randomIndex];
            audioSource.Play();
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitsSelected.Add(unit);


        SelectUnit(unit, true);

        PlayRandomSelectionClip();
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisible);
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);

            SelectUnit(unit, true);
            
        PlayRandomSelectionClip();
        }
    }

    private void SelectUnit (GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator (unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }
}
