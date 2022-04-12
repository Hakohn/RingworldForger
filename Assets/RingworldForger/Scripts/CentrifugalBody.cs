using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(Rigidbody))]
    public class CentrifugalBody : MonoBehaviour
    {
        [HideInInspector]
        public Rigidbody rb = null;
        private Ringworld rg = null;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        private void FixedUpdate()
        {
            // If centrifugal force is available, it will be calculated accordingly.
            if (rg != null)
            {
                // Is the object within the ring?
                Vector3 localRgCentre = Vector3.zero;
                Vector3 flatLocalPos = rg.transform.InverseTransformPoint(transform.position);
                flatLocalPos.x = 0;
                float flatDistToCentre = (localRgCentre - flatLocalPos).magnitude;
                Vector3 flatDirToCentre = (localRgCentre - flatLocalPos).normalized;
                flatDirToCentre = rg.transform.TransformDirection(flatDirToCentre);

                // Apply centrifugal force.
                float w = rg.spinningSpeed;
                float r = flatDistToCentre;
                float v = w * r;
                float m = rb.mass;
                float G = v * v / r;
                float F = m * G;
                rb.AddForce(-flatDirToCentre * F, ForceMode.Force);

                RotateAround(rg.Centre, rg.transform.right, rg.spinningSpeed * Time.fixedDeltaTime);
            }
        }

        // https://answers.unity.com/questions/1751620/rotating-around-a-pivot-point-using-a-quaternion.html
        void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            Quaternion rot = Quaternion.AngleAxis(angle, axis);
            rb.MovePosition(rot * (rb.position - point) + point);
            rb.MoveRotation(rot * rb.rotation);
        }

        private void OnRingworldEnter(Ringworld ring)
        {
            rg = ring;
        }

        private void OnRingworldExit(Ringworld ring)
        {
            rg = null;
        }

        private void OnDrawGizmos()
        {
            if(rg != null)
            {
                Gizmos.color = Color.magenta;
                Vector3 localRgCentre = Vector3.zero;
                Vector3 flatLocalPos = rg.transform.InverseTransformPoint(transform.position);
                flatLocalPos.x = 0;
                float flatDistToCentre = (localRgCentre - flatLocalPos).magnitude;
                Vector3 flatDirToCentre = (localRgCentre - flatLocalPos).normalized;
                flatDirToCentre = rg.transform.TransformDirection(flatDirToCentre);

                Gizmos.DrawLine(rb.position, rb.position + flatDirToCentre * flatDistToCentre);
            }
        }
    }
}
