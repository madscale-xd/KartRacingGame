using UnityEngine;

public abstract class AbstractKart : MonoBehaviour
{
    [Header("Speed Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float turnSpeed = 100f;
    public float deceleration = 5f;
    public float driftFactor = 0.95f;
    public float boostMultiplier = 1.5f;
    public float bounceForce = 2f; // Added bounce force when steering
    
    private Rigidbody rb;
    private float speedInput;
    private float turnInput;
    private bool isDrifting;
    private float directionChangeCooldown = 0.25f;
    private float lastDirectionChangeTime = 0f;

    private bool isGrounded = false; // Tracks if kart is touching ground
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void FixedUpdate()
    {
        Move();
        Turn();
        ApplyDrift();
    }

    void HandleInput()
    {
        float previousTurnInput = turnInput;

        speedInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        speedInput *= acceleration;

        if (Time.time - lastDirectionChangeTime >= directionChangeCooldown)
        {
            turnInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        }

        isDrifting = Input.GetKey(KeyCode.LeftControl);

        // Apply bounce when switching directions, only if grounded
        if (turnInput != 0 && turnInput != previousTurnInput && Time.time - lastDirectionChangeTime >= directionChangeCooldown)
        {
            if (isGrounded) // Only bounce if the kart is touching ground
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.5f)) 
                {
                    // Apply bounce **along the normal of the ground** instead of world up
                    Vector3 bounceDirection = hit.normal; 
                    rb.velocity += bounceDirection * bounceForce; // Apply force along slope
                }
                else
                {
                    // Default to world up if no ground detected
                    rb.velocity += Vector3.up * bounceForce;
                }

                lastDirectionChangeTime = Time.time; // Reset cooldown
            }
        }
    }

    // Check if the kart lands on the ground
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Check if the kart leaves the ground
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Helper function to check if the kart is on the ground
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 1.5f);
    }

    
    void Move()
    {
        RaycastHit hit;
        Vector3 moveDirection = transform.forward;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.5f))
        {
            // Align forward movement to slope
            moveDirection = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;

            // Force rotation to match ground when landing
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, hit.normal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
        else
        {
            // Optional: Reset rotation gradually in air
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.fixedDeltaTime * 2f);
        }

        if (speedInput > 0)
        {
            rb.AddForce(moveDirection * speedInput, ForceMode.Acceleration);
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    
    void Turn()
    {
        if (rb.velocity.magnitude > 1f) // Turning is always allowed
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(Vector3.up * turn);
        }
    }
    
    void ApplyDrift()
    {
        if (isDrifting)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * rb.velocity.magnitude, driftFactor * Time.fixedDeltaTime);
        }
    }
    
    public void Boost()
    {
        rb.velocity *= boostMultiplier;
    }
}
