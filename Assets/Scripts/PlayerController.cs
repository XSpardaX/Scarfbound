using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;
    public bool doubleJumpActive = true;

    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float pitchMin = -60f;
    public float pitchMax = 80f;
    public float cameraDistance = 5f;
    public Vector3 offsetCamera = new Vector3(0f, 1.5f, 0f);
    public LayerMask collisionLayers;
    public float cameraCollisionRadius = 0.2f;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;
    private bool canDoubleJump;
    private bool isGrounded;
    public bool hasKey = false;

    private MovingPlatform currentPlatform;
    private Vector3 platformVelocity;

    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // ? reset every frame BEFORE movement
        platformVelocity = Vector3.zero;

        HandleGroundCheck();
        HandleCamera();
        HandleMovement();
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            canDoubleJump = doubleJumpActive;

            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            Ray ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
            {
                currentPlatform = hit.collider.GetComponent<MovingPlatform>();
            }
            else
            {
                currentPlatform = null;
            }
        }
        else
        {
            currentPlatform = null;
        }
    }

    private void HandleCamera()
    {
        if (cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, pitchMin, pitchMax);

        Vector3 pivotPoint = transform.position + offsetCamera;
        Quaternion camRotation = Quaternion.Euler(cameraPitch, transform.eulerAngles.y, 0f);

        Vector3 normalCameraPosition = pivotPoint - (camRotation * Vector3.forward * cameraDistance);
        Vector3 directionToCamera = normalCameraPosition - pivotPoint;

        float desiredDistance = directionToCamera.magnitude;
        Vector3 updatedCameraPosition = normalCameraPosition;

        if (Physics.SphereCast(pivotPoint, cameraCollisionRadius, directionToCamera.normalized,
            out RaycastHit hit, desiredDistance, collisionLayers))
        {
            updatedCameraPosition = pivotPoint + directionToCamera.normalized * hit.distance;
        }

        cameraTransform.position = updatedCameraPosition;
        cameraTransform.rotation = camRotation;
    }

    private void HandleMovement()
    {
        if (DialogueState.isInDialogue) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 input = new Vector2(horizontal, vertical);
        Vector3 moveDir = Vector3.zero;

        if (cameraTransform != null && input.sqrMagnitude > 0.01f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDir = (forward * input.y + right * input.x).normalized;
        }

        Vector3 move = moveDir * moveSpeed;

        Vector3 platformMove = Vector3.zero;

        if (currentPlatform != null)
        {
            Vector3 pv = currentPlatform.GetVelocity();
            platformMove = new Vector3(pv.x, 0f, pv.z);
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
            }
            else if (canDoubleJump && doubleJumpActive)
            {
                canDoubleJump = false;
                verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
            }
        }

        // Gravity
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move((move + platformMove) * Time.deltaTime);
    }

    public void ApplyBounce(float bounceForce)
    {
        verticalVelocity = bounceForce;
    }

    public void ResetVerticalVelocity()
    {
        verticalVelocity = 0f;
    }
}

/*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GenericEnemy enemy = hit.gameObject.GetComponent<GenericEnemy>();
        if (enemy == null) return;
        bool movingDown = verticalVelocity < 0f;
        enemy.OnPlayerHit(transform, hit.point, movingDown);
    }*/