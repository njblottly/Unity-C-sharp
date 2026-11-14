using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimationDriver : MonoBehaviour
{
    [Header("Movement Detection")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float movingThreshold = 0.01f;

    [Header("Animator Parameters")]
    [SerializeField] private string isMovingParameter = "IsMoving";

    [Header("Sprite Facing")]
    [SerializeField] private bool flipSpriteWhenMovingLeft = true;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 lastPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        Vector2 velocity = GetMovementVelocity();

        bool isMoving = velocity.sqrMagnitude > movingThreshold * movingThreshold;

        animator.SetBool(isMovingParameter, isMoving);

        if (flipSpriteWhenMovingLeft && Mathf.Abs(velocity.x) > movingThreshold)
        {
            spriteRenderer.flipX = velocity.x < 0f;
        }

        lastPosition = transform.position;
    }

    private Vector2 GetMovementVelocity()
    {
        if (rb != null)
        {
            return rb.linearVelocity;
        }

        Vector2 currentPosition = transform.position;
        Vector2 estimatedVelocity = (currentPosition - lastPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
        return estimatedVelocity;
    }
}