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

    [Header("Ground & Jump")]
    public float jumpBufferTime = 0.15f;
    public float groundCheckBuffer = 0.3f;

    public bool hasKey = false;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;
    private bool canDoubleJump;
    private bool isGrounded;
    private float jumpBufferTimer;
    private bool jumpedThisFixedUpdate;

    private MovingPlatform currentPlatform;

    private PlayerStateMachine stateMachine;
    private Animator animator;

    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded => isGrounded;

    public IdleState      Idle      { get; private set; }
    public RunState       Run       { get; private set; }
    public JumpStartState JumpStart { get; private set; }
    public FallingState   Falling   { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponentInChildren<Animator>();
        stateMachine = new PlayerStateMachine();

        Idle      = new IdleState(this, stateMachine, animator);
        Run       = new RunState(this, stateMachine, animator);
        JumpStart = new JumpStartState(this, stateMachine, animator);
        Falling   = new FallingState(this, stateMachine, animator);

        stateMachine.Initialize(Idle);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (stateMachine.CurrentState != null)
        {
            stateMachine.CurrentState.Tick();
        }
    }

    private void FixedUpdate()
    {
        jumpedThisFixedUpdate = false;

        RefreshPlatformReference();
        ApplyPlatformMovement();
        HandleGroundCheck();
        HandleMovement();
        ApplyPlatformStick();
    }

    private void RefreshPlatformReference()
    {
        if (currentPlatform != null) return;

        if (TryGetGroundHit(out RaycastHit groundHit, groundCheckBuffer + 0.15f))
        {
            currentPlatform = groundHit.collider.GetComponentInParent<MovingPlatform>();
        }
    }

    private void LateUpdate()
    {
        HandleCamera();
    }

    private void ApplyPlatformMovement()
    {
        if (currentPlatform == null) return;

        controller.Move(currentPlatform.GetMovementDelta());
    }

    private void HandleGroundCheck()
    {
        const float tightGroundDistance = 0.1f;

        bool tightGround = controller.isGrounded;
        if (!tightGround)
        {
            tightGround = TryGetGroundHit(out _, tightGroundDistance);
        }

        bool platformSupport = false;
        RaycastHit platformHit = default;

        if (currentPlatform != null && verticalVelocity <= 0f)
        {
            float platformBuffer = groundCheckBuffer
                + Mathf.Max(0f, currentPlatform.GetVelocity().y) * Time.fixedDeltaTime * 2f;
            platformSupport = TryGetGroundHit(out platformHit, platformBuffer);
        }

        isGrounded = verticalVelocity <= 0.05f && (tightGround || platformSupport);

        if (!isGrounded)
        {
            currentPlatform = null;
            return;
        }

        if (TryGetGroundHit(out RaycastHit hit, platformSupport ? groundCheckBuffer : tightGroundDistance))
        {
            MovingPlatform hitPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
            if (hitPlatform != null)
            {
                currentPlatform = hitPlatform;
                hitPlatform.Activate();
            }
        }

        canDoubleJump = doubleJumpActive;

        if (verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
    }

    private void ApplyPlatformStick()
    {
        if (jumpedThisFixedUpdate || currentPlatform == null || verticalVelocity > 0f) return;

        float platformBuffer = groundCheckBuffer
            + Mathf.Max(0f, currentPlatform.GetVelocity().y) * Time.fixedDeltaTime * 2f;

        if (TryGetGroundHit(out RaycastHit platformHit, platformBuffer))
        {
            StickToGround(platformHit.distance, groundCheckBuffer);
        }
    }

    private bool TryGetGroundHit(out RaycastHit hit, float checkBuffer)
    {
        Vector3 capsuleBottom = transform.position + controller.center - Vector3.up * (controller.height * 0.5f);
        Vector3 origin = capsuleBottom + Vector3.up * controller.radius;
        float castDistance = controller.skinWidth + checkBuffer;
        float radius = controller.radius * 0.9f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out hit, castDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        if (Physics.Raycast(origin, Vector3.down, out hit, castDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        hit = default;
        return false;
    }

    private void StickToGround(float hitDistance, float maxSnap)
    {
        if (hitDistance <= controller.skinWidth) return;

        float snap = hitDistance - controller.skinWidth;
        if (snap > 0f && snap <= maxSnap * 0.25f)
        {
            controller.Move(Vector3.down * snap);
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
            out RaycastHit cameraHit, desiredDistance, collisionLayers))
        {
            updatedCameraPosition = pivotPoint + directionToCamera.normalized * cameraHit.distance;
        }

        cameraTransform.position = updatedCameraPosition;
        cameraTransform.rotation = camRotation;
    }

    private void HandleMovement()
    {
        if (DialogueState.isInDialogue) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 inputAxes = new Vector2(horizontal, vertical);
        Vector3 moveDirection = Vector3.zero;

        if (cameraTransform != null && inputAxes.sqrMagnitude > 0.01f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * inputAxes.y + right * inputAxes.x).normalized;
        }

        Vector3 horizontalMove = moveDirection * moveSpeed;

        if (jumpBufferTimer > 0f)
        {
            if (isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
                jumpBufferTimer = 0f;
                jumpedThisFixedUpdate = true;
            }
            else if (canDoubleJump && doubleJumpActive)
            {
                canDoubleJump = false;
                verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
                jumpBufferTimer = 0f;
                jumpedThisFixedUpdate = true;
            }
        }

        verticalVelocity += gravity * Time.fixedDeltaTime;
        horizontalMove.y = verticalVelocity;

        controller.Move(horizontalMove * Time.fixedDeltaTime);
    }

    public void ApplyBounce(float bounceForce)
    {
        verticalVelocity = bounceForce;
    }

    public void ResetVerticalVelocity()
    {
        verticalVelocity = 0f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        MovingPlatform platform = hit.collider.GetComponentInParent<MovingPlatform>();
        if (platform != null)
        {
            platform.Activate();
            currentPlatform = platform;
        }

        EnemyBase touchedEnemy = hit.gameObject.GetComponentInParent<EnemyBase>();

        if (touchedEnemy != null)
        {
            touchedEnemy.OnPlayerContact(this);
        }
    }
}
