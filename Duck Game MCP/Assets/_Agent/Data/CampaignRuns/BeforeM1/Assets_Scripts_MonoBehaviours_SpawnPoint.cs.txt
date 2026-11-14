using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Optional Prefab Spawning")]
    [SerializeField] private GameObject objectPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private bool useSpawnPointRotation = true;

    public Vector3 SpawnPosition => transform.position;

    public Quaternion SpawnRotation
    {
        get
        {
            if (useSpawnPointRotation)
            {
                return transform.rotation;
            }

            return Quaternion.identity;
        }
    }

    public GameObject SpawnObject()
    {
        if (objectPrefab == null)
        {
            Debug.LogWarning(gameObject.name + " cannot spawn because no Object Prefab is assigned.");
            return null;
        }

        GameObject spawnedObject = Instantiate(
            objectPrefab,
            SpawnPosition,
            SpawnRotation
        );

        return spawnedObject;
    }

    public GameObject SpawnObject(GameObject prefabOverride)
    {
        if (prefabOverride == null)
        {
            Debug.LogWarning(gameObject.name + " cannot spawn because prefabOverride is null.");
            return null;
        }

        GameObject spawnedObject = Instantiate(
            prefabOverride,
            SpawnPosition,
            SpawnRotation
        );

        return spawnedObject;
    }

    public void MoveObjectToSpawnPoint(GameObject objectToMove)
    {
        if (objectToMove == null)
        {
            Debug.LogWarning(gameObject.name + " cannot move object because objectToMove is null.");
            return;
        }

        objectToMove.transform.position = SpawnPosition;
        objectToMove.transform.rotation = SpawnRotation;

        Rigidbody2D rb = objectToMove.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.15f);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.35f);
    }
}