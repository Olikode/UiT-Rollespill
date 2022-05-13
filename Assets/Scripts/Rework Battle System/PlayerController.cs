using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private DialougeManager dialougeManager;

    public DialougeManager DialougeManager => dialougeManager;

    public IInteractable Interactable { get; set; }


    public float moveSpeed;

    public bool isPaused;

    public Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector3 originalSize;

    public event Action<UnitList> OnEncountered;

    public event Action<UnitList, Challenger> OnChallenged;

    protected virtual void Start()
    {
        originalSize = transform.localScale;
    }

    public void HandleUpdate()
    {
        ProcessInputs();
    }

    private void Update()
    {
        //If player presses M, the dialouge starts. Also checks if the dialouge is open, to stop the player from pressing M again, and ending up with double the text. 
        if (Input.GetKeyDown(KeyCode.M) && dialougeManager.IsOpen == false)
        {
            if (Interactable != null)
            {
                Interactable.Interact(this);
            }
        }
    }
    void FixedUpdate()
    {
        if (isPaused || dialougeManager.IsOpen)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        else if (!isPaused)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveDirection.x > 0)
        {
            transform.localScale = originalSize;
        }
        else if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.z);
        }

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    // When colliding with enemy battle starts
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "WildEnemy")
        {
            var enemyUnit = collider.gameObject.GetComponent<UnitList>();
            OnEncountered(enemyUnit);
            Destroy(collider.gameObject);
        }

        if (collider.tag == "Challenger")
        {
            var enemyUnits = collider.gameObject.GetComponent<UnitList>();
            Challenger challenger = collider.gameObject.GetComponent<Challenger>();
            OnChallenged(enemyUnits, challenger);
            Destroy(collider.gameObject);
        }
    }

    /*private bool IsWalkable(Vector3 targetPos){
        if (Physics2D.OverlapCircle(targetPos, 0.3f, blockingLayer) != null){
            return false;
        }

        return true;
    }*/
}
