using UnityEngine;

public class GravityController : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float baseGravityScale = 1f;
    [SerializeField] private float airborneGravityMultiplier = 1.5f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float surfaceAttractionForce = 8f;
    [SerializeField] private float surfaceReleaseForce = 7f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float moveRightForce = 4f;
    [SerializeField] private float curveAdjustmentFactor = 1.5f;

    [Header("Collision Settings")]
    [SerializeField] private float shellRadius = 0.01f;
    [SerializeField] private int collisionIterations = 3;

    [Header("Curve Boost Settings")]
    [SerializeField] private float curveBoostMultiplier = 1.2f;
    [SerializeField] private float minNormalXForBoost = 0.3f;

    private Rigidbody2D rb;
    private bool gravityDown = true;
    private bool isGrounded;
    private Vector2 groundNormal;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hitBuffer = new RaycastHit2D[1];
    private float currentBoost = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityDown ? baseGravityScale : -baseGravityScale;

        contactFilter.SetLayerMask(groundLayer);
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;
    }

    void Update()
    {
        bool inputDetected = false;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                inputDetected = true;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
        }

        if (inputDetected)
        {
            ToggleGravity();
            ApplyReleaseForce();
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();

        float currentGravity = isGrounded ? baseGravityScale : baseGravityScale * airborneGravityMultiplier;
        rb.gravityScale = gravityDown ? currentGravity : -currentGravity;

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, rb.velocity.normalized * maxSpeed, Time.fixedDeltaTime * 5f);
        }

        if (isGrounded)
        {
            Vector2 gravityDirection = gravityDown ? Vector2.down : Vector2.up;
            Vector2 surfaceGravity = Vector3.Project(gravityDirection * 9.81f, -groundNormal);
            rb.AddForce(surfaceGravity * surfaceAttractionForce);

            Vector2 globalRightForce = Vector2.right * moveRightForce;

            if (Mathf.Abs(groundNormal.x) > minNormalXForBoost)
            {
                currentBoost = Mathf.Lerp(currentBoost, curveBoostMultiplier, Time.fixedDeltaTime * 2f);
            }
            else
            {
                currentBoost = Mathf.Lerp(currentBoost, 1f, Time.fixedDeltaTime * 2f);
            }

            globalRightForce *= currentBoost;

            float curveFactor = 0.5f + Mathf.Abs(groundNormal.x) * curveAdjustmentFactor;
            rb.AddForce(globalRightForce * curveFactor, ForceMode2D.Force);

            PreventPenetration();
        }

        float targetRotation = gravityDown ? 0f : 180f;
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                           Quaternion.Euler(0, 0, targetRotation),
                                           rotationSpeed * Time.fixedDeltaTime);
    }

    private void PreventPenetration()
    {
        Vector2 direction = gravityDown ? Vector2.down : Vector2.up;
        float distance = groundCheckDistance + shellRadius;

        for (int i = 0; i < collisionIterations; i++)
        {
            int count = rb.Cast(direction, contactFilter, hitBuffer, distance);
            if (count > 0)
            {
                float pushDistance = hitBuffer[0].distance - shellRadius;
                rb.position += direction * pushDistance;
            }
        }
    }

    private void ApplyReleaseForce()
    {
        if (isGrounded)
        {
            Vector2 forceDirection = gravityDown ? Vector2.up : Vector2.down;
            rb.AddForce(forceDirection * surfaceReleaseForce, ForceMode2D.Impulse);
        }
    }

    private void ToggleGravity()
    {
        AudioManager.Instance.PlayGravity();
        gravityDown = !gravityDown;
    }

    private void CheckGrounded()
    {
        int count = rb.Cast(gravityDown ? Vector2.down : Vector2.up, contactFilter, hitBuffer, groundCheckDistance + shellRadius);
        isGrounded = count > 0;

        if (isGrounded)
        {
            groundNormal = hitBuffer[0].normal;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 direction = gravityDown ? Vector2.down : Vector2.up;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(direction * groundCheckDistance));

        if (isGrounded)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2f);
        }
    }
}