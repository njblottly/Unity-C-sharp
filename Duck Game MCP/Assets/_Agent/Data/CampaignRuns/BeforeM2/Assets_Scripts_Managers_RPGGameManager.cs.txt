using System.Collections;
using UnityEngine;

public class RPGGameManager : MonoBehaviour
{
    public static RPGGameManager sharedInstance = null;

    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private SpawnPoint playerSpawnPoint;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1.5f;
    [SerializeField] private bool respawnPlayerOnDeath = true;

    [Header("Camera")]
    [SerializeField] private RPGCameraManager cameraManager;

    private Coroutine respawnCoroutine;

    public Player Player => player;
    public SpawnPoint PlayerSpawnPoint => playerSpawnPoint;

    private void Awake()
    {
        if (sharedInstance != null && sharedInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        sharedInstance = this;
    }

    private void Start()
    {
        SetupScene();
    }

    public void SetupScene()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
            }
        }

        if (player == null)
        {
            Debug.LogWarning("RPGGameManager could not find a Player.");
            return;
        }

        if (playerSpawnPoint == null)
        {
            Debug.LogWarning("RPGGameManager has no PlayerSpawnPoint assigned.");
            return;
        }

        EnsureCameraManager();
        UpdateCameraFollowTarget();
    }

    public void NotifyPlayerDied()
    {
        if (!respawnPlayerOnDeath)
        {
            Debug.Log("Player died, but respawnPlayerOnDeath is disabled.");
            return;
        }

        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        respawnCoroutine = StartCoroutine(RespawnPlayerAfterDelay());
    }

    public void RespawnPlayerNow()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        RespawnPlayer();
    }

    public void SetPlayerSpawnPoint(SpawnPoint newSpawnPoint)
    {
        if (newSpawnPoint == null)
        {
            Debug.LogWarning("Tried to set player spawn point to null.");
            return;
        }

        playerSpawnPoint = newSpawnPoint;
    }

    private IEnumerator RespawnPlayerAfterDelay()
    {
        Debug.Log("Respawn timer started.");

        yield return new WaitForSeconds(respawnDelay);

        RespawnPlayer();

        respawnCoroutine = null;
    }

    private void RespawnPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Cannot respawn player because Player reference is missing.");
            return;
        }

        if (playerSpawnPoint == null)
        {
            Debug.LogWarning("Cannot respawn player because PlayerSpawnPoint reference is missing.");
            return;
        }

        player.RespawnAt(playerSpawnPoint.transform);

        UpdateCameraFollowTarget();

        Debug.Log("Player respawned at spawn point.");
    }


    private bool EnsureCameraManager()
    {
        if (cameraManager != null)
        {
            return true;
        }

        cameraManager = RPGCameraManager.sharedInstance;

        if (cameraManager != null)
        {
            return true;
        }

        cameraManager = FindFirstObjectByType<RPGCameraManager>();

        if (cameraManager == null)
        {
            Debug.LogWarning("RPGGameManager could not find an RPGCameraManager. Camera follow will not be updated.");
            return false;
        }

        return true;
    }

    private void UpdateCameraFollowTarget()
    {
        if (player == null)
        {
            return;
        }

        if (!EnsureCameraManager())
        {
            return;
        }

        cameraManager.SetFollowTarget(player.transform);
    }
}