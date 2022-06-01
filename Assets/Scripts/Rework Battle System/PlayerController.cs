using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Image talkingNPC;
    public float moveSpeed;
    public bool isPaused;
    public Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector3 originalSize;
    private SpriteRenderer spriteR;

    public event Action<UnitList> OnEncountered;

    public event Action<UnitList, Challenger> OnChallenged;

    protected virtual void Start()
    {
        originalSize = transform.localScale;
    }

    public void SetPlayerSprite()
    {
        // set gameobject sprite based on what class player is
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        var playerSprite = gameObject.GetComponent<UnitList>().GetPlayerUnit().Base.Sprite;
        spriteR.sprite = playerSprite;
    }

    public void HandleUpdate()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        // freeze when paused
        // unfreeze when not paused
        if (isPaused)
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
        // flip character
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
        // Set NPC image
        if (collider.tag == "NPC")
        {
            talkingNPC.sprite = collider.gameObject.GetComponent<SpriteRenderer>().sprite;
        }

        if (collider.tag == "WildEnemy")
        {
            var enemyUnit = collider.gameObject.GetComponent<UnitList>();
            OnEncountered(enemyUnit);
            Destroy(collider.gameObject);
        }
    }
}
