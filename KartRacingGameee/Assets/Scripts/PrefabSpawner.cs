using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject spawnPrefab; // The prefab to spawn
    public float spawnDistance = 2f; // Distance from the spawner along x-axis
    public float spawnInterval = 1f; // Time between spawns
    public Transform targetPoint; // The point the spawned objects should face
    public float minZ = -2f; // Minimum Z position offset
    public float maxZ = 2f;  // Maximum Z position offset

    [Header("Spawn Force Settings")]
    [SerializeField] private float forwardPushForce = 5f; // Forward force when spawning

    private void Start()
    {
        if (spawnPrefab != null)
        {
            StartCoroutine(SpawnPrefabsRoutine());
        }
    }

    private IEnumerator SpawnPrefabsRoutine()
    {
        while (true)
        {
            SpawnPrefab();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnPrefab()
    {
        if (spawnPrefab == null || targetPoint == null) return;

        // Get a random Z offset within the given range
        float randomZOffset = Random.Range(minZ, maxZ);

        // Calculate spawn position along local x-axis with a random Z offset
        Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
        spawnPos.z += randomZOffset; // Apply the random Z offset

        // Spawn the object
        GameObject spawnedObject = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);

        // Make it face the target
        spawnedObject.transform.LookAt(targetPoint);

        // Rotate 90 degrees so the cylinder lies flat
        spawnedObject.transform.Rotate(0, 0, 90);

        // Apply forward force if the spawned object has a Rigidbody
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDirection = spawnedObject.transform.forward * forwardPushForce;
            rb.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
