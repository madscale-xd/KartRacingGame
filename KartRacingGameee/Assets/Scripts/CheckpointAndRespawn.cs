using UnityEngine;
using System.Collections;

public class CheckpointAndRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint; // The respawn location
    [SerializeField] private float respawnDelay = 1f; // Time before unfreezing movement
    private static Transform lastRespawnPoint; // Last valid respawn location
    private SFXManager sfxman;

    private void Start(){
        sfxman = GameObject.Find("EventSystem").GetComponent<SFXManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Player reaches this checkpoint
        {
            lastRespawnPoint = respawnPoint;
            Debug.Log($"Checkpoint {gameObject.name} activated! Respawn set to {respawnPoint.position}");
        }
    }

    public void RespawnPlayer(Transform player)
    {
        sfxman.PlaySound("Respawn");
        if (lastRespawnPoint == null)
        {
            Debug.LogWarning("No checkpoint reached yet!");
            return;
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // Freeze all positions except Y (player can still fall)
            // Freeze all rotations completely
            playerRb.constraints = RigidbodyConstraints.FreezePositionX |
                                   RigidbodyConstraints.FreezePositionZ |
                                   RigidbodyConstraints.FreezeRotation;
        }

        // Move player to respawn point & reset rotation
        player.position = lastRespawnPoint.position;
        player.rotation = lastRespawnPoint.rotation; // Set rotation to match respawn point
        playerRb.velocity = Vector3.zero; // Reset velocity

        Debug.Log($"Player respawned at {lastRespawnPoint.position}, facing {lastRespawnPoint.rotation.eulerAngles}");

        // Unfreeze after delay
        player.GetComponent<MonoBehaviour>().StartCoroutine(UnfreezePlayer(playerRb));
    }

    private IEnumerator UnfreezePlayer(Rigidbody playerRb)
    {
        yield return new WaitForSeconds(respawnDelay);

        if (playerRb != null)
        {
            // Only freeze rotation, allow full movement again
            playerRb.constraints = RigidbodyConstraints.FreezeRotation;
            Debug.Log("Player movement unfreezed!");
        }
    }
}
