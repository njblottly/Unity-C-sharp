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
    public float InvulnerabilityRemaining => Mathf.Max(0,nextEnemyDamageTime-Time.time);
    // All combat sources share this cooldown; TakeDamage retains its direct scripted-damage API.
    public bool TryTakeEnemyDamage(int amount)
    {
        if(isDead||amount<=0||Time.time<nextEnemyDamageTime)return false;
        nextEnemyDamageTime=Time.time+enemyDamageCooldown;TakeDamage(amount);return true;
    }
    public Inventory PlayerInventory => inventory;
    [SerializeField] private Inventory sceneInventory;
    [SerializeField] private HealthBar sceneHealthBar;
    public event System.Action Died;
    public event System.Action Respawned;
    public event System.Action<Item> ItemCollected;

    // Reach the existing respawn flow instead of Character's destructive default.
    public override void KillCharacter()
    {
        Die();
    }

    public void Start()
    {
        hitPoints.value = startingHitPoints;

        Inventory[] inventories = FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        HealthBar[] healthBars = FindObjectsByType<HealthBar>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (inventories.Length > 1 || healthBars.Length > 1)
            throw new System.InvalidOperationException("Player requires one Inventory and one HealthBar; remove stale duplicates.");
        inventory = sceneInventory != null ? sceneInventory : (inventories.Length == 1 ? inventories[0] : Instantiate(inventoryPrefab));
        healthBar = sceneHealthBar != null ? sceneHealthBar : (healthBars.Length == 1 ? healthBars[0] : Instantiate(healthBarPrefab));
        healthBar.character = this;

        isDead = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Trigger hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TryTakeEnemyContactDamage(collision);
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
            TryTakeEnemyContactDamage(collision);
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
            case Item.ItemType.SWORD:
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
            ItemCollected?.Invoke(hitObject);
        }
    }

    private void TryTakeEnemyContactDamage(Collider2D source)
    {
        EnemyHitPoints health=source.GetComponentInParent<EnemyHitPoints>();
        if(health==null)throw new System.InvalidOperationException("Enemy contact has no EnemyHitPoints: "+source.name);
        if(health.IsDead)return;
        DuckAdventure.Agent.EnemyAgent agent=source.GetComponentInParent<DuckAdventure.Agent.EnemyAgent>();
        DuckAdventure.Agent.MarshPatrol patrol=source.GetComponentInParent<DuckAdventure.Agent.MarshPatrol>();
        int damage=agent!=null?agent.Data.contactDamage:patrol!=null&&patrol.Definition!=null?patrol.Definition.contactDamage:enemyTouchDamage;
        TryTakeEnemyDamage(damage);
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
            // Teleport the physics body as well as the interpolated render transform.
            rb.position = spawnPoint.position;
            rb.rotation = spawnPoint.eulerAngles.z;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        hitPoints.value = startingHitPoints;
        nextEnemyDamageTime = Time.time + 0.25f;
        isDead = false;

        print("Player respawned. HP restored to: " + hitPoints.value);
        Respawned?.Invoke();
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

        Died?.Invoke();
        gameObject.SetActive(false);
    }
}
