using UnityEngine;
using System.Collections;
using System.IO;



[System.Serializable]
public class AbilityData
{
    public int coins = 0;
    public bool isBoughtTurbo = false;
    public bool isBoughtStopMovement = false;
    public bool isBoughtFall = false;
    public bool isBoughtStick = false;
}


public class Movement : MonoBehaviour
{

    public bool isBoughtTurbo = false;
    public bool isBoughtStopMovement = false;
    public bool isBoughtFall = false;
    public bool isBoughtStick = false;
    private string savePath;

    public bool isSticked = false;
    public Vector3 gravityDirection = Vector3.down;
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
    public float customGravityScale = 20f;
    public Transform stickCheck;
    public Camera cam;
    public LayerMask wallLayer;

    public KeyCode stickKey;
    public KeyCode jumpKey;
    public KeyCode stopMovementKey;
    public KeyCode turboMovementKey;
    public KeyCode fallKey;

    private Vector2 moveInput;
    public bool isGrounded;
    public bool isStickingToWall = false;
    public Vector3 customGravity;
    private Vector3 wallRight;
    private Vector3 wallUp;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        customGravity = new Vector3(0, -9.81f * customGravityScale, 0);
        savePath = Path.Combine(Application.dataPath, "AbilityBoughtStatus.json");

        LoadAbilityStatus();
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
                rb.useGravity = true;
                rb.AddForce(-gravityDirection * jumpForce, ForceMode.Impulse);
            }
            else if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        if (isBoughtStick && Input.GetKeyDown(stickKey))
        {
            if (isStickingToWall)
            {
                isStickingToWall = false;
                rb.useGravity = true;
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

            rb.AddForce(Vector3.down * customGravityScale, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(gravityDirection * stickForce, ForceMode.Acceleration);

            Vector3 moveDir = wallRight * moveInput.x + wallUp * moveInput.y;
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.VelocityChange);
        }

        if (!isGrounded && isBoughtFall && Input.GetKey(fallKey))
        {
            rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration);
        }

        if (isGrounded && isBoughtStopMovement && Input.GetKey(stopMovementKey))
        {
            rb.linearDamping = brakeDrag;
            rb.angularDamping = brakeAngularDrag;
        }
        else
        {
            rb.linearDamping = normalDrag;
            rb.angularDamping = normalAngularDrag;
        }

        if (isBoughtTurbo && Input.GetKey(turboMovementKey))
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

        if (colliders.Length == 0)
        {
            return;
        }

        Transform closest = null;
        Collider closestCollider = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            float dist = Vector3.Distance(stickCheck.position, col.ClosestPoint(stickCheck.position));
            if (dist < closestDistance)
            {
                closest = col.transform;
                closestCollider = col;
                closestDistance = dist;
            }
        }

        if (closest != null && closestCollider != null)
        {
            Vector3 directionToWall = (stickCheck.position - closestCollider.ClosestPoint(stickCheck.position)).normalized;
            gravityDirection = -directionToWall;
            rb.useGravity = false;

            wallRight = Vector3.Cross(Vector3.up, gravityDirection).normalized;
            wallUp = Vector3.Cross(gravityDirection, wallRight).normalized;

            isStickingToWall = true;
        }
    }



    [System.Serializable]
    public class AbilityData
    {
        public int coins = 0;
        public bool isBoughtTurbo = false;
        public bool isBoughtStopMovement = false;
        public bool isBoughtFall = false;
        public bool isBoughtStick = false;
    }

    private IEnumerator CheckAbilitiesBought()
{
    float timer = 0f;

    while (timer < 2f)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            AbilityData data = JsonUtility.FromJson<AbilityData>(json);

            // Оновлюємо здібності, якщо їх ще не активовано
            if (data.isBoughtTurbo && !isBoughtTurbo)
            {
                isBoughtTurbo = true;
                Debug.Log("✔ Turbo ability activated!");
            }

            if (data.isBoughtStopMovement && !isBoughtStopMovement)
            {
                isBoughtStopMovement = true;
                Debug.Log("✔ StopMovement ability activated!");
            }

            if (data.isBoughtFall && !isBoughtFall)
            {
                isBoughtFall = true;
                Debug.Log("✔ Fall ability activated!");
            }

            if (data.isBoughtStick && !isBoughtStick)
            {
                isBoughtStick = true;
                Debug.Log("✔ Stick ability activated!");
            }
        }

        timer += Time.deltaTime;
        yield return null;
    }
}


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shop_obj"))
        {
            StartCoroutine(CheckAbilitiesBought());
        }

        if (isStickingToWall && other.CompareTag("Vidlip"))
    {
            isStickingToWall = false;
            rb.useGravity = true;
    }
    }



    private void LoadAbilityStatus()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            AbilityData data = JsonUtility.FromJson<AbilityData>(json);

            // Оновлюємо локальні змінні
            isBoughtTurbo = data.isBoughtTurbo;
            isBoughtStopMovement = data.isBoughtStopMovement;
            isBoughtFall = data.isBoughtFall;
            isBoughtStick = data.isBoughtStick;

            // Оновлюємо coins у Status
            Transform statusTransform = transform.Find("Status");
            if (statusTransform != null)
            {
                Status status = statusTransform.GetComponent<Status>();
                if (status != null)
                {
                    status.coins = data.coins;
                }
            }

            // Оновлюємо текст в CountCoin (Canvas)
            GameObject coinTextObj = GameObject.Find("CountCoin");
            if (coinTextObj != null)
            {
                TMPro.TextMeshProUGUI coinText = coinTextObj.GetComponent<TMPro.TextMeshProUGUI>();
                if (coinText != null)
                {
                    coinText.text = "Coins: " + data.coins;
                }
            }
            else
            {
                Debug.LogWarning("⚠ Не знайдено об'єкт CountCoin у Canvas.");
            }




            Debug.Log("✔ Ability and coin data loaded from JSON.");
        }
        else
        {
            Debug.LogWarning("⚠ JSON file not found: AbilityBoughtStatus.json");
        }
    }


}
