using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float stickForce = 100f;
    public float turboMultiplier = 3f;
    public float normalMoveSpeed = 0.4f;
    public float normalDrag = 0.5f;
    public float normalAngularDrag = 0.5f;
    public float brakeDrag = 5f;
    public float brakeAngularDrag = 5f;
    public float stickRadius = 1.5f;
    public Transform stickCheck;
    public Transform directionPointer;
    public Camera cam;
    public LayerMask wallLayer;
    public KeyCode stickKey;
    public KeyCode jumpKey;
    public KeyCode stopMovementKey;
    public KeyCode turboMovementKey;
    public KeyCode fallKey;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isStickingToWall = false;
    private Vector3 customGravity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        customGravity = Physics.gravity;
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(jumpKey))
        {
            if (isStickingToWall)
            {
                isStickingToWall = false;
                Physics.gravity = customGravity;
                rb.AddForce(-rb.transform.forward * jumpForce + Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        if (Input.GetKeyDown(stickKey))
        {
            if (isStickingToWall)
            {
                isStickingToWall = false;
                Physics.gravity = customGravity;
            }
            else
            {
                StickToWall();
            }
        }
    }

    void FixedUpdate()
    {
        CheckGround();

        if (!isStickingToWall)
        {
            Vector3 moveDir = cam.transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
            moveDir.y = 0;
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 wallForward = Vector3.Cross(Vector3.up, -Physics.gravity.normalized);
            Vector3 wallUp = Vector3.Cross(wallForward, -Physics.gravity.normalized);
            Vector3 moveDir = wallForward * moveInput.x + wallUp * moveInput.y;
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.VelocityChange);
        }

        if (!isGrounded && Input.GetKey(fallKey))
        {
            rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration);
        }

        if (isGrounded && Input.GetKey(stopMovementKey))
        {
            rb.linearDamping = brakeDrag;
            rb.angularDamping = brakeAngularDrag;
        }
        else
        {
            rb.linearDamping = normalDrag;
            rb.angularDamping = normalAngularDrag;
        }

        if (Input.GetKey(turboMovementKey))
        {
            moveSpeed = turboMultiplier;
        }
        else
        {
            moveSpeed = normalMoveSpeed;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.1f, 0), 0.5f, wallLayer);
    }

    void StickToWall()
    {
        Collider[] colliders = Physics.OverlapSphere(stickCheck.position, stickRadius, wallLayer);
        if (colliders.Length == 0) return;

        Transform closest = colliders[0].transform;
        float minDist = Vector3.Distance(stickCheck.position, closest.ClosestPoint(stickCheck.position));

        foreach (Collider col in colliders)
        {
            float dist = Vector3.Distance(stickCheck.position, col.ClosestPoint(stickCheck.position));
            if (dist < minDist)
            {
                closest = col.transform;
                minDist = dist;
            }
        }

        Vector3 directionToWall = (stickCheck.position - closest.ClosestPoint(stickCheck.position)).normalized;
        Physics.gravity = -directionToWall * stickForce;
        isStickingToWall = true;
    }
}
