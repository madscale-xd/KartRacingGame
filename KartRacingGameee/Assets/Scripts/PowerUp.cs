using UnityEngine;
using System.Collections.Generic;
using System.Collections; 

public class PowerUp : MonoBehaviour
{
    private int currentPowerUp = 0;
    private List<GameObject> deactivatedPowerUps = new List<GameObject>();

    [SerializeField] private GameObject slowZonePrefab; // Assign this in Unity Inspector

    [SerializeField] private GameObject barrelPrefab; // Assign this in Unity Inspector

    [SerializeField] private GameObject flashPrefab; // Assign this in Unity Inspector
    [SerializeField] private Transform player; // Reference to the player's transform
    [SerializeField] private Rigidbody playerRb; // Reference to the player's Rigidbody
    [SerializeField] private float squidBoostForce = 15f; // How much force to apply

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            currentPowerUp = Random.Range(1, 6);
            Debug.Log("Power-up collected! Number: " + currentPowerUp);

            other.gameObject.SetActive(false);
            deactivatedPowerUps.Add(other.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentPowerUp > 0)
        {
            ActivatePowerUp();
            currentPowerUp = 0;
        }
    }

    private void ActivatePowerUp()
    {
        switch (currentPowerUp)
        {
            case 1:
                Debug.Log("Power-up 1 activated! Squid.");
                ApplySquidEffect();
                break;
            case 2:
                Debug.Log("Power-up 2 activated! Gigant.");
                ApplyGigant();
                break;
            case 3:
                Debug.Log("Power-up 3 activated! Barrel Roll.");
                ApplyBarrelRoll();
                break;
            case 4:
                Debug.Log("Power-up 4 activated! Minimize.");
                ApplyMinimize();
                break;
            case 5:
                Debug.Log("Power-up 5 activated! Flashbang.");
                ApplyFlashbang();
                break;
            default:
                Debug.Log("No power-up available.");
                break;
        }
    }

    private void ApplySquidEffect()
    {
        Debug.Log("Squid Boost Activated!");

        // Apply a forward boost to the player
        Vector3 boostDirection = player.forward * squidBoostForce;
        playerRb.velocity = new Vector3(boostDirection.x, playerRb.velocity.y, boostDirection.z);

        // Calculate spawn position behind the player
        Vector3 spawnPosition = player.position - player.forward * 7f; // Move it back

        // Move the player forward a bit after boosting
        player.position += player.forward * 2f; // Adjust this value as needed

        // Instantiate the slow zone behind the player
        GameObject slowZone = Instantiate(slowZonePrefab, spawnPosition, Quaternion.identity);

        // Destroy the slow zone after 10 seconds
        Destroy(slowZone, 10f);
    }

        private void ApplyBarrelRoll()
    {
        Debug.Log("Barrel Roll Activated!");

        // Apply a forward boost to the player
        Vector3 boostDirection = player.forward * squidBoostForce;
        playerRb.velocity = new Vector3(boostDirection.x, playerRb.velocity.y, boostDirection.z);

        // Calculate spawn position behind the player
        Vector3 spawnPosition = player.position - player.forward * 4f; // Move it back

        // Move the player forward a bit after boosting
        player.position += player.forward * 2f; // Adjust this value as needed

        // Instantiate the barrel behind the player
        GameObject barrel = Instantiate(barrelPrefab, spawnPosition, Quaternion.identity);

        // Resize the barrel
        Vector3 originalBarrelScale = barrel.transform.localScale;
        Vector3 newBarrelScale = originalBarrelScale * 1.25f; // Adjust this scale factor as needed
        barrel.transform.localScale = newBarrelScale;

        // Add force to eject the barrel from the player
        Rigidbody barrelRb = barrel.GetComponent<Rigidbody>();
        if (barrelRb != null)
        {
            Vector3 ejectDirection = -player.forward * 45f; // Adjust the force magnitude as needed
            barrelRb.AddForce(ejectDirection, ForceMode.Impulse);
        }

        // Destroy the barrel after 3 seconds
        Destroy(barrel, 3f);
    }

    private void ApplyFlashbang()
    {
        Debug.Log("Flashbang Activated!");

        // Apply a forward boost to the player
        Vector3 boostDirection = player.forward * squidBoostForce;
        playerRb.velocity = new Vector3(boostDirection.x, playerRb.velocity.y, boostDirection.z);

        // Calculate spawn position behind the player
        Vector3 spawnPosition = player.position - player.forward * 7f; // Move it back

        // Move the player forward a bit after boosting
        player.position += player.forward * 2f; // Adjust this value as needed

        // Instantiate the slow zone behind the player
        GameObject flash = Instantiate(flashPrefab, spawnPosition, Quaternion.identity);

        // Destroy the slow zone after 10 seconds
        Destroy(flash, 1.25f);
    }


    private void ApplyGigant()
    {
        Debug.Log("Gigant Power-up Activated!");

        // Move the player forward
        player.position += player.forward * 5f;

        // Log original size
        Vector3 originalScale = player.localScale;
        Debug.Log("Original Scale: " + originalScale);

        // Increase player size
        Vector3 gigantScale = originalScale * 1.75f; // Doubles the size
        player.localScale = gigantScale;

        // Log new size
        Debug.Log("Gigant Scale: " + gigantScale);

        // Start coroutine to reset size after 10 seconds
        StartCoroutine(ResetSizeAfterDelay(originalScale, 10f));
    }

    private void ApplyMinimize()
    {
        Debug.Log("Minimize Power-up Activated!");

        // Move the player forward
        player.position += player.forward * 5f;

        // Log original size
        Vector3 originalScale = player.localScale;
        Debug.Log("Original Scale: " + originalScale);

        // Decrease player size
        Vector3 minimizeScale = originalScale * 0.5f; // Halves the size
        player.localScale = minimizeScale;

        // Log new size
        Debug.Log("Minimize Scale: " + minimizeScale);

        // Start coroutine to reset size after 10 seconds
        StartCoroutine(ResetSizeAfterDelay(originalScale, 10f));
    }

    // Coroutine to reset player size after a delay
    private IEnumerator ResetSizeAfterDelay(Vector3 originalScale, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the player's size
        player.localScale = originalScale;
        Debug.Log("Gigant effect ended. Player size reset.");
    }

    public void ReactivatePowerUps()
    {
        foreach (GameObject powerUp in deactivatedPowerUps)
        {
            if (powerUp != null)
            {
                powerUp.SetActive(true);
            }
        }
        deactivatedPowerUps.Clear();
    }
}
