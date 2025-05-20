using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public DirectionPointer directionPointer;
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool isGrounded;

    public KeyCode stopMovementKey;
    public KeyCode turboMovementKey;
    public KeyCode fallKey;
    public KeyCode gravityChangrerKey;

    public Vector2 moveInput;
    public LayerMask groundFilter;

    void ReadInput_Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            MakeJump();
        }
    }
    void MakeJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
     
    }
    void CheckGround_FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.1f ,0) , 0.5f, groundFilter);
    }

    void Move_FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = directionPointer.cameraObject.transform.TransformDirection(moveDirection);
        moveDirection.y = 0; // Keep the movement on the XZ plane
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.VelocityChange);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

 

    void Update()
    {
        ReadInput_Update();
    }
    void FixedUpdate()
    {
        CheckGround_FixedUpdate();
        Move_FixedUpdate();
       // Stopmoving_FixedUpdate();
    //    TurboMoving_FixedUpdate();



    if(isGrounded==false){

        if (Input.GetKey(fallKey))
        {
            Vector3 customGravity = new Vector3(0, -100f, 0);
            rb.AddForce(customGravity, ForceMode.Acceleration);

        }

    }


    if (isGrounded){
    if (Input.GetKey(stopMovementKey))
    {
        rb.angularDamping = 5;
        rb.linearDamping = 5;
        moveSpeed = 0f;
        return; 
    }}


    if (Input.GetKey(turboMovementKey))
    {
        moveSpeed = 3f;
        rb.angularDamping = 0f;
        rb.linearDamping = 0f; 
    }
    else
    {
        moveSpeed = 0.4f;
        rb.angularDamping = 0.5f;
        rb.linearDamping = 0.5f;
    }
   

    }
}  //прилипаня до стін зміна модельок двойний прижок магазин деш 