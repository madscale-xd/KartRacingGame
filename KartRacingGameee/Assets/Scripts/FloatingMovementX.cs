using UnityEngine;

public class FloatingMovementX : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float amplitude = 3f; // Distance it moves up and down
    [SerializeField] private float speed = 2f; // Speed of movement

    private Vector3 startPos;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Prevents gravity from affecting it
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        Vector3 targetPosition = new Vector3(startPos.x, newY, startPos.z);

        rb.MovePosition(targetPosition); // Moves using physics
    }
}
