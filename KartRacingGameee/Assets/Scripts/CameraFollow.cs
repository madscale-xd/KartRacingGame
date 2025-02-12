using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target; // Player's transform
    public Vector3 offset = new Vector3(0, 3, -6); // Default camera position
    public float followSpeed = 10f; // Smooth follow speed
    public float rotationSpeed = 5f; // Smooth rotation speed

    private Rigidbody targetRb;

    void FixedUpdate()
    {
        // Find the player if it's missing
        if (target == null)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                target = player.transform;
                targetRb = player.GetComponent<Rigidbody>();
            }
        }

        if (target == null) return;

        // Smoothly interpolate camera position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.fixedDeltaTime);

        // Align camera tilt to match the target's tilt on slopes
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
