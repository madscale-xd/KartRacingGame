using UnityEngine;

public class BarrelDestroy : MonoBehaviour
{
    private void Start()
    {
        Invoke("DestroySelf", 10f); // Schedule destruction after 20 seconds
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BarrelDestroyer"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            Invoke("DestroySelf", 2f);  // Start a new 5s countdown
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
