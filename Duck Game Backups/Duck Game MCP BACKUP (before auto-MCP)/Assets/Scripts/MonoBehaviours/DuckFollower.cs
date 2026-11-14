using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class DuckFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Follow Settings")]
    [SerializeField] private float followBehindDistance = 0.75f;
    [SerializeField] private Vector2 extraOffset = Vector2.zero;
    [SerializeField] private float stopRadius = 0.12f;
    [SerializeField] private float maxSpeed = 3.5f;
    [SerializeField] private float followSmoothTime = 0.18f;
    [SerializeField] private float teleportIfFartherThan = 10f;

    [Header("Direction Detection")]
    [SerializeField] private float minimumPlayerMoveForDirection = 0.001f;

    [Header("Animation State Names")]
    [SerializeField] private string idleSouthState = "idleSouth";
    [SerializeField] private string walkSouthState = "walkSouth";
    [SerializeField] private string walkNorthState = "walkNorth";
    [SerializeField] private string walkEastState = "walkEast";
    [SerializeField] private string walkWestState = "walkWest";

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 lastPlayerPosition;
    private Vector2 lastFollowDirection = Vector2.down;
    private Vector2 smoothMoveVelocity;

    private string currentAnimState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void Reset()
    {
        followBehindDistance = 0.75f;
        extraOffset = Vector2.zero;
        stopRadius = 0.12f;
        maxSpeed = 3.5f;
        followSmoothTime = 0.18f;
        teleportIfFartherThan = 10f;
        minimumPlayerMoveForDirection = 0.001f;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            smoothMoveVelocity = Vector2.zero;
            UpdateAnim(Vector2.zero);
            return;
        }

        Vector2 playerPosition = player.position;
        Vector2 playerDelta = playerPosition - lastPlayerPosition;

        if (playerDelta.sqrMagnitude > minimumPlayerMoveForDirection * minimumPlayerMoveForDirection)
        {
            lastFollowDirection = playerDelta.normalized;
        }

        lastPlayerPosition = playerPosition;

        Vector2 desiredPosition =
            playerPosition
            - lastFollowDirection * followBehindDistance
            + extraOffset;

        float distanceToDesiredPosition = Vector2.Distance(rb.position, desiredPosition);

        if (distanceToDesiredPosition > teleportIfFartherThan)
        {
            TeleportTo(desiredPosition);
            return;
        }

        if (distanceToDesiredPosition <= stopRadius)
        {
            smoothMoveVelocity = Vector2.zero;
            UpdateAnim(Vector2.zero);
            return;
        }

        Vector2 oldPosition = rb.position;

        Vector2 newPosition = Vector2.SmoothDamp(
            rb.position,
            desiredPosition,
            ref smoothMoveVelocity,
            followSmoothTime,
            maxSpeed,
            Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        Vector2 animationVelocity = (newPosition - oldPosition) / Time.fixedDeltaTime;
        UpdateAnim(animationVelocity);
    }

    private void TeleportTo(Vector2 targetPosition)
    {
        rb.position = targetPosition;
        smoothMoveVelocity = Vector2.zero;
        UpdateAnim(Vector2.zero);
    }

    private void UpdateAnim(Vector2 velocity)
    {
        if (animator == null)
        {
            return;
        }

        if (velocity.sqrMagnitude < 0.001f)
        {
            PlayAnim(idleSouthState);
            return;
        }

        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            if (velocity.x > 0f)
            {
                PlayAnim(walkEastState);
            }
            else
            {
                PlayAnim(walkWestState);
            }
        }
        else
        {
            if (velocity.y > 0f)
            {
                PlayAnim(walkNorthState);
            }
            else
            {
                PlayAnim(walkSouthState);
            }
        }
    }

    private void PlayAnim(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return;
        }

        if (currentAnimState == stateName)
        {
            return;
        }

        currentAnimState = stateName;
        animator.Play(stateName);
    }
}