using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(SphereCollider))]
    public class Ringworld : MonoBehaviour
    {
        public float spinningSpeed = 10;

        [HideInInspector]
        public SphereCollider trigger = null;

        /// <summary> DO NOT CHANGE THE VALUE OF THIS VARIABLE. The radius of the ring, as given by RingworldForger on creation. </summary>
        [HideInInspector]
        public float radius = 0.0f;
        /// <summary> DO NOT CHANGE THE VALUE OF THIS VARIABLE. The radius of the ring, as given by RingworldForger on creation. </summary>
        [HideInInspector]
        public float width = 0.0f;
        public float diameter { get; private set; } = 0.0f;
        public float halfWidth { get; private set; } = 0.0f;
        public Vector3 Centre => transform.position;
        public Transform capturedObjectsParent { get; private set; } = null;

        private void Awake()
        {
            diameter = radius * 2.0f;
            halfWidth = width / 2.0f;

            UpdateBounds();

            capturedObjectsParent = new GameObject("Captured Objects").transform;
            capturedObjectsParent.parent = transform;
        }

        public void UpdateBounds()
        {
            SphereCollider trigger = GetComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.center = Vector3.zero;
            trigger.radius = radius;
        }

        private void Update()
        {
            transform.Rotate(Vector3.right, spinningSpeed * Time.deltaTime);
        }

        private void OnTriggerStay(Collider other)
        {
            if(other.gameObject.TryGetComponent(out CentrifugalBody cb))
            {
                if(cb.isActiveAndEnabled)
                {
                    Vector3 localPosition = transform.InverseTransformPoint(cb.transform.position);
                    // Is the object within the ring?
                    if(Mathf.Abs(localPosition.x) <= halfWidth)
                    {
                        cb.transform.parent = capturedObjectsParent;

                        // Calculate the distance from ring's centre.
                        Vector3 flattenedLocalPosition = localPosition;
                        flattenedLocalPosition.x = 0;
                        Vector3 flattenedDirection = flattenedLocalPosition.normalized;

                        // Apply centrifugal force.
                        float w = spinningSpeed;
                        float r = flattenedLocalPosition.magnitude;
                        float v = w * r;
                        float m = cb.rb.mass;
                        float G = v * v / r;
                        float F = m * G;
                        cb.rb.AddForce(flattenedDirection * F, ForceMode.Acceleration);
                        //float centrifugalTorque = cb.rb.mass * (spinningSpeed * spinningSpeed * (distanceFromCentre));
                        //cb.rb.AddTorque(flattenedDirection * centrifugalTorque, ForceMode.Acceleration);

                        if(cb.alignUpToRingCentre)
                        {
                            cb.transform.up = -flattenedDirection;
                        }
                    }
                }
            }
        }
    }
}
