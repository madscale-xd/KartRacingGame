using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the kart
    public Vector3 offset = new Vector3(0, 3, -6); // Default camera position
    public float followSpeed = 10f; // Smooth follow speed
    public float rotationSpeed = 5f; // Smooth rotation speed

    private Rigidbody targetRb;

    void Start()
    {
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody>();
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Smoothly interpolate camera position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.fixedDeltaTime);

        // Align camera tilt to match the target's tilt on slopes
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
