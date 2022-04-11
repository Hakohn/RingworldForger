using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(CentrifugalBody))]
    public class Simple3DController : MonoBehaviour
    {
        [Header("Movement")]
        public float movementSpeed = 4;
        public float accelerationSpeed = 40;
        public float sensitivity = 100;
        public float jumpForce = 10;

        [Header("Keybinds")]
        public string movementVerticalAxis = "Vertical";
        public KeyCode jump = KeyCode.Space;
        public string mouseXAxis = "Mouse X";
        public string mouseYAxis = "Mouse Y";

        [HideInInspector]
        public CentrifugalBody cb = null;

        private float verticalInput = default;
        private Vector2 mouseMovement = default;
        private bool wantsToJump = false;

        [Header("Camera")]
        public new Camera camera = null;
        public Vector3 offset = new Vector3(0, 2.5f, -5.5f);
        public float maxVerticalLookAngle = 80f;
        public float minVerticalLookAngle = -20f;
        private float verticalLookAngle = 0;
        [SerializeField, HideInInspector]
        private Transform cameraLook = null;

        private void Awake()
        {
            cb = GetComponent<CentrifugalBody>();

            FocusCameraOnThis();
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.25f);
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

           
            camera.transform.position = transform.TransformPoint(offset);
            camera.transform.parent = cameraLook;
        }

        private void Update()
        {
            verticalInput = Input.GetAxisRaw(movementVerticalAxis);

            mouseMovement.x = Input.GetAxisRaw(mouseXAxis);
            mouseMovement.y = -Input.GetAxisRaw(mouseYAxis);

            wantsToJump = false;
            if (Input.GetKeyDown(jump)) wantsToJump = true;

            if (verticalInput != 0)
            {
                if(cb.transform.InverseTransformVector(cb.rb.velocity).z < movementSpeed)
                {
                    cb.rb.AddForce(verticalInput * transform.forward * accelerationSpeed, ForceMode.Acceleration);
                }
            }
            if(mouseMovement.sqrMagnitude != 0)
            {
                transform.Rotate(Vector3.up, sensitivity * mouseMovement.x * Time.deltaTime, Space.Self);

                float delta = mouseMovement.y * sensitivity * Time.deltaTime;
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
            }

            if (wantsToJump)
            {
                cb.rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                wantsToJump = false;
            }
        }

        private void FixedUpdate()
        {
        }
    }
}
