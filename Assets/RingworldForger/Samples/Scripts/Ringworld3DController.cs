using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(CentrifugalBody))]
    public class Ringworld3DController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("The movement speed of the character while walking.")]
        public float movementSpeed = 2;
        [Tooltip("The acceleration speed of the character. For best results, keep this around 10 times larger than the movement speed.")]
        public float accelerationSpeed = 20;
        [Tooltip("While sprinting (tapping the sprint button once while moving forward), how quicker will the movement speed and acceleration in comparison to the walking speed?")]
        public float sprintSpeedMultiplier = 3f;
        [Tooltip("While tactical sprinting (tapping the sprint button twice while moving forward), how quicker will the movement speed and acceleration in comparison to the walking speed?")]
        public float tacticalSprintSpeedMultiplier = 4f;
        [Tooltip("While not touching the ground, how quicker will the movement in comparison to the walking speed?")]
        public float airSpeedMultiplier = 1.0f;
        [Tooltip("The impulse force applied to the character when pressing the jump button while on ground.")]
        public float jumpForce = 500;
        [Tooltip("What object layers are considered ground by this character?")]
        public LayerMask groundMask = 1;
        [Tooltip("When checking for ground touching, this character is doing a sphere check at its feet. What will the radius be of that sphere? The higher the value, the less likely it is to be floating randomly, but the accuracy is also going to decrease.")]
        public float groundCheckRadius = 0.25f;
        private Vector3 movementVelocity = Vector3.zero;
        private bool isTacticalSprinting = false;
        private bool isSprinting = false;
        private bool isGrounded = false;

        [Header("Input")]
        [Tooltip("The axis used for vertical (forward and backward) movement. The axis' settings can be changed from Unity's panel ProjectSettings/Input.")]
        public string movementVerticalAxis = "Vertical";
        [Tooltip("The axis used for horizontal (left and right) movement. The axis' settings can be changed from Unity's panel ProjectSettings/Input.")]
        public string movementHorizontalAxis = "Horizontal";
        [Tooltip("The button used for jumping. The button's settings can be changed from Unity's panel ProjectSettings/Input.")]
        public string jumpButton = "Jump";
        [Tooltip("The button used for sprinting and tactical sprinting. The button's settings can be changed from Unity's panel ProjectSettings/Input.")]
        public string sprintButton = "Sprint";
        [Space]
        [Tooltip("The mouse look sensitivity.")]
        public float sensitivity = 300;
        [Tooltip("The axis used for horizontal (left and right) look rotation. The axis' settings can be changed from Unity's panel ProjectSettings/Input.")]
        public string mouseXAxis = "Mouse X";
        [Tooltip("The axis used for vertical (up and down) look rotation. The axis' settings can be changed from Unity's panel ProjectSettings/Input.")]
        public string mouseYAxis = "Mouse Y";
        private Vector2 moveInput = default;
        private Vector2 lookInput = default;
        private const float buttonCooldown = 0.1f;
        private bool pressedSprint = false;
        private float pressedSprintTimer = 0.0f;
        private bool pressedJump = false;
        private float pressedJumpTimer = 0.0f;

        [HideInInspector]
        public CentrifugalBody cb = null;
        private Ringworld rg = null;

        [Header("Camera")]
        [Tooltip("The camera used to view this character. It should be attached to an empty object, child of the main character.")]
        public new Camera camera = null;
        [Tooltip("The offset of the camera, in comparison to the character.")]
        public Vector3 offset = new Vector3(0.5f, 0f, -2.0f);
        [Tooltip("The maximum vertical look rotation angle of the camera.")]
        public float maxVerticalLookAngle = 80f;
        [Tooltip("The maximum vertical look rotation angle of the camera.")]
        public float minVerticalLookAngle = -80f;
        private float verticalLookAngle = 0;
        [SerializeField, HideInInspector]
        private Transform cameraLook = null;

        [Header("Animation")]
        [Tooltip("The mesh's animator. Should not be left empty.")]
        public Animator animator = null;
        [Tooltip("The lerping used for inputting float values to the animator, such as the character's velocity. For smooth transitions, keep this higher.")]
        public float animationLerp = 20f;

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
                pressedSprint = Input.GetButtonDown(sprintButton);
                if (pressedSprint) pressedSprintTimer = 0f;
            }

            if (pressedJumpTimer < buttonCooldown)
            {
                pressedJumpTimer += Time.deltaTime;
            }
            else
            {
                pressedJump = Input.GetButtonDown(jumpButton);
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
