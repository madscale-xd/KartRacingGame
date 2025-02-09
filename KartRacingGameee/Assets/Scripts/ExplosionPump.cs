using UnityEngine;
using System.Collections;

public class ExplosionPump : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f; // How far it moves
    [SerializeField] private float moveDuration = 1f; // Time to reach max distance
    [SerializeField] private float returnDuration = 5f; // Time to return
    [SerializeField] private float waitAtPeak = 2f; // How long to wait at max distance
    [SerializeField] private float cycleInterval = 3f; // Time before repeating cycle

    private Vector3 originalPosition;
    
    void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(PumpCycle());
    }

    private IEnumerator PumpCycle()
    {
        while (true)
        {
            yield return MoveTo(originalPosition + Vector3.forward * moveDistance, moveDuration);
            yield return new WaitForSeconds(waitAtPeak); // Wait at peak
            yield return MoveTo(originalPosition, returnDuration);
            yield return new WaitForSeconds(cycleInterval); // Wait before restarting
        }
    }

    private IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}
