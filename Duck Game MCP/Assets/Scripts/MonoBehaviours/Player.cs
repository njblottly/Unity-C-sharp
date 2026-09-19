using UnityEngine;

public class Player : Character
{
    [Header("Inventory")]
    public Inventory inventoryPrefab;
    private Inventory inventory;

    [Header("Health Bar")]
    public HealthBar healthBarPrefab;
    private HealthBar healthBar;

    [Header("Enemy Contact Damage")]
    [SerializeField] private int enemyTouchDamage = 1;
    [SerializeField] private float enemyDamageCooldown = 1f;

    private float nextEnemyDamageTime;
    private bool isDead;

    public bool IsDead => isDead;

    public void Start()
    {
        hitPoints.value = startingHitPoints;

        inventory = Instantiate(inventoryPrefab);

        healthBar = Instantiate(healthBarPrefab);
        healthBar.character = this;

        isDead = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Trigger hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TryTakeEnemyContactDamage();
            return;
        }

        if (collision.gameObject.CompareTag("CanBePickedUp"))
        {
            HandlePickup(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TryTakeEnemyContactDamage();
        }
    }

    private void HandlePickup(Collider2D collision)
    {
        print("Tag matched");

        Consumable consumable = collision.gameObject.GetComponent<Consumable>();

        if (consumable == null)
        {
            return;
        }

        Item hitObject = consumable.item;

        if (hitObject == null)
        {
            return;
        }

        print("Item type: " + hitObject.itemType + ", stackable: " + hitObject.stackable);

        bool shouldDisappear = false;

        switch (hitObject.itemType)
        {
            case Item.ItemType.COIN:
                shouldDisappear = inventory.AddItem(hitObject);
                print("AddItem returned: " + shouldDisappear);
                break;

            case Item.ItemType.HEALTH:
                shouldDisappear = Heal(hitObject.quantity);
                break;

            default:
                break;
        }

        if (shouldDisappear)
        {
            collision.gameObject.SetActive(false);
        }
    }

    private void TryTakeEnemyContactDamage()
    {
        if (isDead)
        {
            return;
        }

        if (Time.time < nextEnemyDamageTime)
        {
            return;
        }

        TakeDamage(enemyTouchDamage);
        nextEnemyDamageTime = Time.time + enemyDamageCooldown;
    }

    public bool AdjustHitPoints(int amount)
    {
        if (amount > 0)
        {
            return Heal(amount);
        }

        if (amount < 0)
        {
            TakeDamage(-amount);
            return true;
        }

        return false;
    }

    public bool Heal(int amount)
    {
        if (isDead)
        {
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        if (hitPoints.value >= maxHitPoints)
        {
            return false;
        }

        hitPoints.value = Mathf.Clamp(hitPoints.value + amount, 0f, maxHitPoints);

        print("Healed by: " + amount + ". New HP: " + hitPoints.value);

        return true;
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        hitPoints.value = Mathf.Clamp(hitPoints.value - amount, 0f, maxHitPoints);

        print("Player took damage: " + amount + ". New HP: " + hitPoints.value);

        if (hitPoints.value <= 0f)
        {
            KillCharacter();
        }
    }

    public void RespawnAt(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Cannot respawn player because spawnPoint is null.");
            return;
        }

        gameObject.SetActive(true);

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        hitPoints.value = startingHitPoints;
        nextEnemyDamageTime = Time.time + 0.25f;
        isDead = false;

        print("Player respawned. HP restored to: " + hitPoints.value);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        print("Player died.");

        if (RPGGameManager.sharedInstance != null)
        {
            RPGGameManager.sharedInstance.NotifyPlayerDied();
        }
        else
        {
            Debug.LogWarning("Player died, but no RPGGameManager.sharedInstance was found.");
        }

        gameObject.SetActive(false);
    }
}