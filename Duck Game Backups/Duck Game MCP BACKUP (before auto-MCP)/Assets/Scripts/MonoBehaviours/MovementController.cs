using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float movementSpeed = 3.0f;

    public Vector2 MoveDir { get; private set; }
    public Vector2 LastMoveDir { get; private set; } = Vector2.down;

    private Animator animator;
    Rigidbody2D rb2D;

    private const string animationState = "AnimationState";

    private enum CharStates
    {
        walkEast = 1,
        walkSouth = 2,
        walkWest = 3,
        walkNorth = 4,

        idleSouth = 5
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ReadInput();
        UpdateState();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 v = new Vector2(x, y);

        if (v.sqrMagnitude > 0.0001f)
        {
            v.Normalize();
            MoveDir = v;
            LastMoveDir = v;
        }
        else
        {
            MoveDir = Vector2.zero;
        }
    }

    private void MoveCharacter()
    {
        rb2D.linearVelocity = MoveDir * movementSpeed;
    }

    private void UpdateState()
    {
        if (MoveDir.x > 0) animator.SetInteger(animationState, (int)CharStates.walkEast);
        else if (MoveDir.x < 0) animator.SetInteger(animationState, (int)CharStates.walkWest);
        else if (MoveDir.y > 0) animator.SetInteger(animationState, (int)CharStates.walkNorth);
        else if (MoveDir.y < 0) animator.SetInteger(animationState, (int)CharStates.walkSouth);
        else animator.SetInteger(animationState, (int)CharStates.idleSouth);
    }
    
}
