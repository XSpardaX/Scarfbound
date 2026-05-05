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

    public bool hasKey = false;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;
    private bool canDoubleJump;
    private bool isGrounded;

    private MovingPlatform currentPlatform;
    private Vector3 platformVelocity;

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
        HandleGroundCheck();

        if (stateMachine.CurrentState != null)
        {
            stateMachine.CurrentState.Tick();
        }
    }

    private void LateUpdate()
    {
        platformVelocity = Vector3.zero;

        HandleCamera();
        HandleMovement();
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (!isGrounded)
        {
            currentPlatform = null;
            return;
        }

        canDoubleJump = doubleJumpActive;

        if (verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        Ray groundRay = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(groundRay, out RaycastHit groundHit, 1.5f))
        {
            currentPlatform = groundHit.collider.GetComponent<MovingPlatform>();
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
        Vector3 platformMove = Vector3.zero;

        if (currentPlatform != null)
        {
            Vector3 platformWorldVelocity = currentPlatform.GetVelocity();
            platformMove = new Vector3(platformWorldVelocity.x, 0f, platformWorldVelocity.z);
        }

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

        verticalVelocity += gravity * Time.deltaTime;
        horizontalMove.y = verticalVelocity;

        controller.Move((horizontalMove + platformMove) * Time.deltaTime);
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
        EnemyBase touchedEnemy = hit.gameObject.GetComponentInParent<EnemyBase>();

        if (touchedEnemy != null)
        {
            touchedEnemy.OnPlayerContact(this);
        }
    }
}
