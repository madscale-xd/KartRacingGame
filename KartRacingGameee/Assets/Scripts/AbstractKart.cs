using UnityEngine;
using System.Collections;

public abstract class AbstractKart : MonoBehaviour
{
    protected float acceleration = 10f;
    protected float maxSpeed = 20f;
    protected float turnSpeed = 100f;
    protected float deceleration = 5f;
    protected float driftFactor = 0.95f;
    protected float boostMultiplier = 1.5f;
    protected float boostDuration = 2f;
    protected float slowMultiplier = 0.5f; // Slowdown multiplier
    protected float slowDuration = 2f;    // Duration of slowdown effect
    protected float bounceForce = 2f;
    protected float controlsDisableTime = 1.5f;

    protected float maxAirTime = 1.8f;

    protected float knockbackForce = 30000f;
    private float airTime = 0f;
    private float downwardForce = 0f;
    private float maxDownwardForce = 300f; // Cap for downward force
    private float downwardForceIncreaseRate = 60f; // How fast force increases

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
    private bool isHumPlaying = false;
    private Coroutine stopHumCoroutine = null; // Reference to the stop coroutine

    public int LapCount = 0;

    private TimerUI timer;

    private SceneButtonManager scene;

    private SFXManager sfxman;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        originalMaxSpeed = maxSpeed;
        timer = GameObject.Find("KartUI").GetComponent<TimerUI>();
        scene = GameObject.Find("LapStarter").GetComponent<SceneButtonManager>();
        sfxman = GameObject.Find("EventSystem").GetComponent<SFXManager>();
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
        if (controlsDisabled)
        {
            if (isHumPlaying)
            {
                if (stopHumCoroutine != null)
                {
                    StopCoroutine(stopHumCoroutine); // Cancel the delay if already scheduled
                }
                stopHumCoroutine = StartCoroutine(DelayedStopHum(0.5f)); // Stop with delay
            }
            return;
        }

        bool hasInput = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        // Start or stop looping sound based on input
        if (hasInput)
        {
            if (!isHumPlaying)
            {
                if (stopHumCoroutine != null)
                {
                    StopCoroutine(stopHumCoroutine); // Cancel the scheduled stop if input resumes
                    stopHumCoroutine = null;
                }
                sfxman.StartLoopingHum("Hum");
                isHumPlaying = true;
            }
        }
        else
        {
            if (isHumPlaying && stopHumCoroutine == null)
            {
                stopHumCoroutine = StartCoroutine(DelayedStopHum(0.5f)); // Stop with delay
            }
        }

        float previousTurnInput = turnInput;

        // Acceleration Input
        speedInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        speedInput *= acceleration;

        // Turn Input
        turnInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);

        isDrifting = Input.GetKey(KeyCode.LeftControl);

        // Bounce only if drifting and turning
        if (isGrounded && isDrifting && turnInput != 0 && turnInput != previousTurnInput)
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

    // Coroutine to delay stopping the hum sound
    private IEnumerator DelayedStopHum(float delay)
    {
        yield return new WaitForSeconds(delay);

        sfxman.StopLoopingHum("Hum");
        isHumPlaying = false;
        stopHumCoroutine = null;
    }

    void ApplyExtraGravity()
    {
        airTime += Time.fixedDeltaTime;
        if (airTime >= maxAirTime)
        {
            //Debug.Log("HEAVIER!!!");
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
            // Check if the object hit is tagged as "Ground" or "MovingGround"
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("MovingGround"))
            {
                // Adjust movement direction based on valid ground normal
                moveDirection = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;

                // Rotate the player to align with the valid ground
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, hit.normal);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
        else
        {
            // If airborne, prevent extreme tilting
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.fixedDeltaTime * 2f);
        }

        // Movement logic
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
            sfxman.PlaySound("Speed");
            Boost();
        }
        else if (other.CompareTag("Slowdown")) // If hitting a slowdown object
        {
            sfxman.PlaySound("SlowEnter");
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
            sfxman.StartLoopingSlow("SlowStay");
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
            sfxman.StopLoopingSlow("SlowStay");
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
            sfxman.PlaySound("Crash");
            Knockback(collision);
        }

        if (collision.gameObject.name == "LapStarter" && LapActive == true)
        {
            Debug.Log("Lap++");
            LapCount++;
            LapActive = false;
            timer.UpdateLapText(LapCount);
            
            // Reactivate all deactivated power-ups
            FindObjectOfType<PowerUp>().ReactivatePowerUps();
            
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
