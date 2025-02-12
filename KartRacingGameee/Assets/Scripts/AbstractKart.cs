using UnityEngine;
using System.Collections;

public abstract class AbstractKart : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] protected float acceleration = 10f;
    [SerializeField] protected float maxSpeed = 20f;
    [SerializeField] protected float turnSpeed = 100f;
    [SerializeField] protected float deceleration = 5f;
    [SerializeField] protected float driftFactor = 0.95f;
    [SerializeField] protected float boostMultiplier = 1.5f;
    [SerializeField] protected float boostDuration = 2f;
    [SerializeField] protected float slowMultiplier = 0.5f; // Slowdown multiplier
    [SerializeField] protected float slowDuration = 2f;    // Duration of slowdown effect
    [SerializeField] protected float bounceForce = 2f;
    [SerializeField] protected float controlsDisableTime = 1.5f;
    private float airTime = 0f;
    private float downwardForce = 0f;
    [SerializeField] private float maxDownwardForce = 50f; // Cap for downward force
    [SerializeField] private float downwardForceIncreaseRate = 10f; // How fast force increases

    private Rigidbody rb;
    private float speedInput;
    private float turnInput;
    private bool isDrifting;
    private bool isBoosting = false;
    private bool isSlowed = false; // Track slowdown status
    private float directionChangeCooldown = 0.25f;
    private float lastDirectionChangeTime = 0f;
    private bool isGrounded = false;

    private Vector3 platformVelocity = Vector3.zero;
    private bool controlsDisabled = false;
    private float originalMaxSpeed;
    private bool boostActive = false;
    private bool slowdownActive = false;

    private bool LapActive = false;

    public int LapCount = 0;

    private TimerUI timer;

    private SceneButtonManager scene;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        originalMaxSpeed = maxSpeed;
        timer = GameObject.Find("KartUI").GetComponent<TimerUI>();
        scene = GameObject.Find("LapStarter").GetComponent<SceneButtonManager>();
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
        ApplyExtraGravity();
    }

    void HandleInput()
    {
        if (controlsDisabled) return;

        float previousTurnInput = turnInput;

        // Acceleration Input
        speedInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        speedInput *= acceleration;

        // Turn Input (instant response)
        turnInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);

        isDrifting = Input.GetKey(KeyCode.LeftControl);

        // Bounce effect is limited by directionChangeCooldown
        if (isGrounded && turnInput != 0 && turnInput != previousTurnInput)
        {
            if (Time.time - lastDirectionChangeTime >= directionChangeCooldown)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.5f))
                {
                    Vector3 bounceDirection = Vector3.Project(hit.normal, Vector3.up).normalized;
                    rb.velocity += bounceDirection * bounceForce;
                }
                else
                {
                    rb.velocity += Vector3.up * bounceForce;
                }

                lastDirectionChangeTime = Time.time; // Apply cooldown only for bounce
            }
        }
    }


    void ApplyExtraGravity()
    {
        airTime += Time.fixedDeltaTime;
        if (airTime >= 1.8f)
        {
            Debug.Log("HEAVIER!!!");
            downwardForce += downwardForceIncreaseRate * Time.fixedDeltaTime;
            downwardForce = Mathf.Min(downwardForce, maxDownwardForce);
            rb.AddForce(Vector3.down * downwardForce, ForceMode.Acceleration);
        }
    }

    void Move()
    {
        RaycastHit hit;
        Vector3 moveDirection = transform.forward;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.5f))
        {
            moveDirection = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, hit.normal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
        else
        {
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
        if (rb.velocity.magnitude > 1f)
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
        if (!isBoosting)
        {
            StartCoroutine(BoostForSeconds(boostDuration));
        }
    }

    private IEnumerator BoostForSeconds(float duration)
    {
        Debug.Log("Boosting!");
        isBoosting = true;
        boostActive = true;

        float boostedSpeed = originalMaxSpeed * boostMultiplier;
        maxSpeed = boostedSpeed;
        rb.velocity *= boostMultiplier;

        yield return new WaitForSeconds(duration);

        isBoosting = false;
        boostActive = false;

        // Only reset maxSpeed if a slowdown isn't currently active
        if (!slowdownActive)
        {
            maxSpeed = originalMaxSpeed;
        }
    }

    public void Slowdown()
    {
        if (!isSlowed)
        {
            StartCoroutine(SlowdownForSeconds(slowDuration));
        }
    }

    private IEnumerator SlowdownForSeconds(float duration)
    {
        Debug.Log("Slowing down!");
        isSlowed = true;
        slowdownActive = true;

        float slowedSpeed = originalMaxSpeed * slowMultiplier;
        maxSpeed = slowedSpeed;
        rb.velocity *= slowMultiplier;

        yield return new WaitForSeconds(duration);

        isSlowed = false;
        slowdownActive = false;

        // Only reset maxSpeed if a boost isn't currently active
        if (!boostActive)
        {
            maxSpeed = originalMaxSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boost"))
        {
            Boost();
        }
        else if (other.CompareTag("Slowdown")) // If hitting a slowdown object
        {
            Slowdown();
        }
        else if (other.CompareTag("LapCounter"))
        {
            Debug.Log("Eligible for a lap!");
            LapActive = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Boost"))
        {
            if (!isBoosting)
            {
                Boost();
            }
        }
        else if (other.CompareTag("Slowdown"))
        {
            if (!isSlowed)
            {
                Slowdown();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boost"))
        {
            StartCoroutine(ResetSpeedAfterDelay(boostDuration));
        }
        else if (other.CompareTag("Slowdown"))
        {
            maxSpeed = originalMaxSpeed; // Immediately restore original speed
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingGround"))
        {
            isGrounded = true;
            airTime = 0f;
            downwardForce = 0f; // Reset force
        }

        if (collision.gameObject.CompareTag("Danger"))
        {
            Knockback(collision);
        }

        if (collision.gameObject.name == "LapStarter" && LapActive == true)
        {
            Debug.Log("Lap++");
            LapCount++;
            LapActive = false;
            timer.UpdateLapText(LapCount);
            if (LapCount == 3)
            {
                timer.ExportTime();
                scene.LoadEnd();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingGround") || collision.gameObject.CompareTag("Danger"))
        {
            isGrounded = false;
            StartCoroutine(ResetGroundedStatusAfterDelay(3.5f));
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingGround"))
        {
            isGrounded = true;
            airTime = 0f;
            downwardForce = 0f; // Reset force
        }
        if (collision.gameObject.CompareTag("MovingGround"))
        {
            Rigidbody platformRb = collision.rigidbody;
            if (platformRb != null)
            {
                platformVelocity = platformRb.velocity;
            }
        }
    }

    void Knockback(Collision collision)
    {
        speedInput = 0;
        turnInput = 0;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 knockbackDirection = (transform.position - collision.contacts[0].point).normalized;
        knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 0.5f;

        float knockbackForce = 30000f;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        StartCoroutine(DisableControlsForSeconds(controlsDisableTime));
    }

    private IEnumerator DisableControlsForSeconds(float duration)
    {
        controlsDisabled = true;
        yield return new WaitForSeconds(duration);
        controlsDisabled = false;
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        maxSpeed = originalMaxSpeed;
    }

    private IEnumerator ResetGroundedStatusAfterDelay(float delay)
    {
        Debug.Log("Ground Reset.");
        yield return new WaitForSeconds(delay);
        isGrounded = true;
    }
}
