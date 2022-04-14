using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(CentrifugalBody))]
    public class Ringworld3DController : MonoBehaviour
    {
        [Header("Movement")]
        public float movementSpeed = 2;
        public float accelerationSpeed = 20;
        public float sprintSpeedMultiplier = 3f;
        public float tacticalSprintSpeedMultiplier = 4f;
        public float airSpeedMultiplier = 0.5f;
        public float sensitivity = 600;
        public float jumpForce = 500;
        public LayerMask groundMask = 1;
        public float groundCheckRadius = 0.25f;
        private Vector3 movementVelocity = Vector3.zero;

        [Header("Keybinds")]
        public string movementVerticalAxis = "Vertical";
        public string movementHorizontalAxis = "Horizontal";
        public string jump = "Jump";
        public string sprint = "Sprint";
        public string mouseXAxis = "Mouse X";
        public string mouseYAxis = "Mouse Y";

        [HideInInspector]
        public CentrifugalBody cb = null;
        private Ringworld rg = null;

        private Vector2 moveInput = default;
        private Vector2 lookInput = default;
        private const float buttonCooldown = 0.1f;
        private bool pressedSprint = false;
        private float pressedSprintTimer = 0.0f;
        private bool pressedJump = false;
        private float pressedJumpTimer = 0.0f;
        private bool isTacticalSprinting = false;
        private bool isSprinting = false;
        private bool isGrounded = false;

        [Header("Camera")]
        public new Camera camera = null;
        public Vector3 offset = new Vector3(0, 2.5f, -5.5f);
        public float maxVerticalLookAngle = 80f;
        public float minVerticalLookAngle = -20f;
        private float verticalLookAngle = 0;
        [SerializeField, HideInInspector]
        private Transform cameraLook = null;

        [Header("Animation")]
        public Animator animator = null;
        public float animationLerp = 0.1f;

        private void Awake()
        {
            cb = GetComponent<CentrifugalBody>();

            FocusCameraOnThis();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1.0f);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            cameraLook.localRotation = Quaternion.identity;
            verticalLookAngle = 0;
        }

        private void OnValidate()
        {
            if(!Application.isPlaying)
            {
                FocusCameraOnThis();
            }
        }

        private void FocusCameraOnThis()
        {
            if (camera == null) return;

            if (cameraLook == null)
            {
                cameraLook = new GameObject("Look").transform;
                cameraLook.parent = transform;
            }

            camera.transform.parent = cameraLook;
            camera.transform.localPosition = offset;
        }

        private void Update()
        {
            CheckInput();
            UpdateAnimations();
        }
        private void CheckInput()
        {
            // Movement
            moveInput = new Vector2(Input.GetAxisRaw(movementHorizontalAxis), Input.GetAxisRaw(movementVerticalAxis));
            if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;

            if (pressedSprintTimer < buttonCooldown)
            {
                pressedSprintTimer += Time.deltaTime;
            }
            else
            {
                pressedSprint = Input.GetButtonDown(sprint);
                if (pressedSprint) pressedSprintTimer = 0f;
            }

            if (pressedJumpTimer < buttonCooldown)
            {
                pressedJumpTimer += Time.deltaTime;
            }
            else
            {
                pressedJump = Input.GetButtonDown(jump);
                if (pressedJump) pressedJumpTimer = 0f;
            }

            // Movement
            bool isMoving = moveInput.sqrMagnitude > 0;

            // Stopping sprinting, if necessary.
            if (moveInput.y < 0 || moveInput.magnitude < 0.5f) isSprinting = false;
            if (moveInput.y < 0.75f)
            {
                if (isTacticalSprinting)
                {
                    isTacticalSprinting = false;
                    isSprinting = true;
                }
            }

            // Handling sprint button presses.
            if (pressedSprint)
            {
                if (isSprinting && moveInput.y > 0.75f)
                {
                    isSprinting = false;
                    isTacticalSprinting = true;
                }
                else if (!isTacticalSprinting)
                {
                    if (isMoving && moveInput.y >= 0 && moveInput.magnitude > 0.5f)
                    {
                        isSprinting = true;
                    }
                }

                pressedSprint = false;
            }

            // Look
            lookInput.x += Input.GetAxisRaw(mouseXAxis);
            lookInput.y += -Input.GetAxisRaw(mouseYAxis);
        }


        private void FixedUpdate()
        {
            if (rg != null) UpdateGroundCheck(); else isGrounded = false;
            UpdateMovement();
            UpdateLook();
        }


        private void UpdateGroundCheck()
        {
            // Ground Check
            bool touchedObject = false;
            foreach(Collider collider in Physics.OverlapSphere(cb.rb.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore))
            {
                if (collider.attachedRigidbody != null && collider.attachedRigidbody == cb.rb) continue;

                touchedObject = true;
                break;
            }
            isGrounded = touchedObject;
        }


        private void UpdateMovement()
        {
            // Jump
            if (pressedJump && isGrounded)
            {
                cb.rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                pressedJump = false;
            }

            float multiplier = !isGrounded ? airSpeedMultiplier : (isTacticalSprinting ? tacticalSprintSpeedMultiplier : (isSprinting ? sprintSpeedMultiplier : 1f));
            float desiredMovementSpeed = movementSpeed * multiplier;


            movementVelocity = Vector3.Lerp(
                movementVelocity,
                (transform.forward * moveInput.y + transform.right * moveInput.x) * desiredMovementSpeed,
                accelerationSpeed * multiplier * Time.fixedDeltaTime
            );
            cb.rb.MovePosition(cb.rb.position + movementVelocity * Time.fixedDeltaTime);
        }
        private void UpdateLook()
        {
            // Look
            if (lookInput.sqrMagnitude != 0)
            {
                cb.rb.MoveRotation(cb.rb.rotation * Quaternion.Euler(0, sensitivity * lookInput.x * Time.fixedDeltaTime, 0));

                float delta = lookInput.y * sensitivity * Time.deltaTime;
                verticalLookAngle += delta;
                if (verticalLookAngle > maxVerticalLookAngle)
                {
                    delta = 0;
                    verticalLookAngle = maxVerticalLookAngle;
                }
                else if (verticalLookAngle < minVerticalLookAngle)
                {
                    delta = 0;
                    verticalLookAngle = minVerticalLookAngle;
                }

                cameraLook.Rotate(Vector3.right, delta, Space.Self);

                lookInput = Vector2.zero;
            }
        }
        private void UpdateAnimations()
        {
            // Animation Update
            if (animator != null)
            {
                animator.SetBool("isGrounded", isGrounded);
                float multiplier = (isTacticalSprinting ? 1.5f : (isSprinting ? 1f : 0.5f));
                animator.SetFloat("velocity.z", Mathf.Lerp(animator.GetFloat("velocity.z"), moveInput.y * multiplier, animationLerp * Time.deltaTime));
                animator.SetFloat("velocity.x", Mathf.Lerp(animator.GetFloat("velocity.x"), moveInput.x * multiplier, animationLerp * Time.deltaTime));
            }
        }

        private void OnRingworldEnter(Ringworld ring)
        {
            rg = ring;
        }

        private void OnRingworldExit(Ringworld ring)
        {
            rg = null;
        }
    }
}
