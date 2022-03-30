using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector3 originalSize;

    public event Action<UnitList> OnEncountered;

    protected virtual void Start()
    {
        originalSize = transform.localScale;
    }

    public void HandleUpdate()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
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
    }
}
