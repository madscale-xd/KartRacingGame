using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private CheckpointAndRespawn assignedCheckpoint; // The checkpoint this KillZone is linked to

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // If the player enters the KillZone
        {
            if (assignedCheckpoint != null)
            {
                assignedCheckpoint.RespawnPlayer(other.transform); // Call Respawn from the linked checkpoint
            }
            else
            {
                Debug.LogWarning($"KillZone {gameObject.name} has no assigned Checkpoint!");
            }
        }
    }
}
