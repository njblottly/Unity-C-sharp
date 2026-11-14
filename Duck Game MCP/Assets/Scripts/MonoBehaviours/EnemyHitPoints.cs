using UnityEngine;

[DefaultExecutionOrder(-60)]
public class EnemyHitPoints : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHitPoints = 3;
    [SerializeField] private int startingHitPoints = 3;

    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0f;
    [SerializeField] private bool disableInsteadOfDestroy = false;

    private int currentHitPoints;
    private bool isDead;

    public event System.Action<int> Damaged;
    public event System.Action Died;
    public int CurrentHitPoints => currentHitPoints;
    public void ConfigureForEncounter(int maximum)
    {
        if(maximum<=0)throw new System.ArgumentOutOfRangeException(nameof(maximum));
        maxHitPoints=maximum;startingHitPoints=maximum;ResetHitPoints();
    }
    public int MaxHitPoints => maxHitPoints;
    public bool IsDead => isDead;

    private void Awake()
    {
        maxHitPoints = Mathf.Max(1, maxHitPoints);
        startingHitPoints = Mathf.Clamp(startingHitPoints, 1, maxHitPoints);
        currentHitPoints = startingHitPoints;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        if (damageAmount <= 0)
        {
            return;
        }

        currentHitPoints -= damageAmount;
        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);

        Debug.Log($"{gameObject.name} took {damageAmount} damage. HP: {currentHitPoints}/{maxHitPoints}");

        Damaged?.Invoke(damageAmount);

        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead)
        {
            return;
        }

        if (healAmount <= 0)
        {
            return;
        }

        currentHitPoints += healAmount;
        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);

        Debug.Log($"{gameObject.name} healed by {healAmount}. HP: {currentHitPoints}/{maxHitPoints}");
    }

    public void ResetHitPoints()
    {
        isDead = false;
        currentHitPoints = startingHitPoints;
        foreach(Collider2D collider in GetComponents<Collider2D>())collider.enabled=true;
        Rigidbody2D body=GetComponent<Rigidbody2D>();if(body!=null){body.simulated=true;body.linearVelocity=Vector2.zero;}
        gameObject.SetActive(true);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        Died?.Invoke();

        if (disableInsteadOfDestroy)
        {
            gameObject.SetActive(false);
            return;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
