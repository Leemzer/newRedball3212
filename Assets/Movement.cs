using UnityEngine;

public class Movement : MonoBehaviour
{
    public DirectionPointer directionPointer;
    public Rigidbody rb;
    public Transform stickCheck;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool isGrounded;
    public bool isSticked;

    public KeyCode stopMovementKey;
    public KeyCode turboMovementKey;
    public KeyCode fallKey;
    public KeyCode gravityChangerKey;

    public Vector2 moveInput;
    public LayerMask groundFilter;
    public LayerMask wallLayer;

    private Quaternion originalDirectionPointerRotation;
    private bool directionAdjusted = false;
    private Vector3 currentCustomGravity = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalDirectionPointerRotation = directionPointer.transform.localRotation;
    }

    void Update()
    {
        ReadInput_Update();

        // Стрибок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            MakeJump();

            // Також скасовує прилипання
            if (isSticked)
            {
                isSticked = false;
                currentCustomGravity = Vector3.zero;
                rb.useGravity = true;
                directionPointer.transform.localRotation = originalDirectionPointerRotation;
                directionAdjusted = false;
            }
        }
    }

    void FixedUpdate()
    {
        CheckGround_FixedUpdate();
        Move_FixedUpdate();

        // Увімкнення/вимкнення прилипання
        if (Input.GetKeyDown(gravityChangerKey))
        {
            isSticked = !isSticked;

            if (!isSticked)
            {
                currentCustomGravity = Vector3.zero;
                rb.useGravity = true;
                directionPointer.transform.localRotation = originalDirectionPointerRotation;
                directionAdjusted = false;
            }
        }

        if (isSticked)
        {
            rb.useGravity = false;

            Collider[] hits = Physics.OverlapSphere(stickCheck.position, 3f, wallLayer);
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;

            foreach (Collider col in hits)
            {
                if (col.attachedRigidbody == rb) continue;

                Vector3 closestPoint = col.ClosestPoint(stickCheck.position);
                float distance = Vector3.Distance(stickCheck.position, closestPoint);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = col;
                }
            }

            if (closestCollider != null)
            {
                Ray ray = new Ray(stickCheck.position, (closestCollider.ClosestPoint(stickCheck.position) - stickCheck.position).normalized);
                if (closestCollider.Raycast(ray, out RaycastHit hitInfo, 5f))
                {
                    Vector3 normal = hitInfo.normal;
                    currentCustomGravity = -normal * 100f;

                    if (!directionAdjusted)
                    {
                        Vector3 forward = Vector3.Cross(directionPointer.cameraObject.transform.right, normal).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(forward, -normal);
                        directionPointer.transform.rotation = targetRotation;
                        directionAdjusted = true;
                    }
                }
            }

            rb.AddForce(currentCustomGravity, ForceMode.Acceleration);
        }

        if (!isSticked && directionAdjusted)
        {
            directionPointer.transform.localRotation = originalDirectionPointerRotation;
            directionAdjusted = false;
        }

        if (Input.GetKey(fallKey) && !isGrounded && !isSticked)
        {
            rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration);
        }

        if (isGrounded && Input.GetKey(stopMovementKey))
        {
            rb.angularDamping = 5;
            rb.linearDamping = 5;
            moveSpeed = 0f;
            return;
        }

        if (Input.GetKey(turboMovementKey))
        {
            moveSpeed = 3f;
            rb.angularDamping = 0f;
            rb.linearDamping = 0f;

            if (isSticked)
            {
                isSticked = false;
                currentCustomGravity = Vector3.zero;
                rb.useGravity = true;
                directionPointer.transform.localRotation = originalDirectionPointerRotation;
                directionAdjusted = false;
            }
        }
        else
        {
            moveSpeed = 0.4f;
            rb.angularDamping = 0.5f;
            rb.linearDamping = 0.5f;
        }
    }

    void ReadInput_Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
    }

    void MakeJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CheckGround_FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.1f, 0), 0.5f, groundFilter);
    }

    void Move_FixedUpdate()
    {
        if (!isSticked)
        {
            // Поворот directionPointer лише коли не прилип
            Quaternion targetRot = Quaternion.Euler(0, directionPointer.cameraObject.transform.eulerAngles.y, 0);
            directionPointer.transform.rotation = targetRot;
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = directionPointer.transform.TransformDirection(moveDirection);
        moveDirection.y = 0; // рух по XZ
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.VelocityChange);
    }
}
