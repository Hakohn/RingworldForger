using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(BoxCollider)), DisallowMultipleComponent]
    public class Ringworld : MonoBehaviour
    {
        public float spinningSpeed = 10;
        [SerializeField, DisableField]
        private float gravitationalPullAtRadius = 0;


        [HideInInspector]
        public BoxCollider trigger = null;
        /// <summary> DO NOT CHANGE THE VALUE OF THIS VARIABLE. The radius of the ring, as given by RingworldForger on creation. </summary>
        [HideInInspector]
        public float radius = 0.0f;
        /// <summary> DO NOT CHANGE THE VALUE OF THIS VARIABLE. The radius of the ring, as given by RingworldForger on creation. </summary>
        [HideInInspector]
        public float width = 0.0f;
        public float diameter { get; private set; } = 0.0f;
        public float halfWidth { get; private set; } = 0.0f;
        public Vector3 Centre => transform.position;

        public List<GameObject> capturedGameObjects = new List<GameObject>();

        private void Awake()
        {
            diameter = radius * 2.0f;
            halfWidth = width / 2.0f;

            UpdateBounds();
        }

        private void OnValidate()
        {
            float w = spinningSpeed;
            float r = radius;
            float v = w * r;
            float m = 1;
            float G = v * v / r;
            float F = m * G;

            gravitationalPullAtRadius = -F;
        }

        public void UpdateBounds()
        {
            diameter = radius * 2.0f;
            halfWidth = width / 2.0f;

            trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = new Vector3(0, 0, 0);
            trigger.size = new Vector3(width, diameter, diameter) + Vector3.one * 2;
        }

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.right, spinningSpeed * Time.fixedDeltaTime, Space.Self);
        }

        private void OnTriggerEnter(Collider other)
        {
            //OnTriggerStay(other);
        }

        private void OnTriggerStay(Collider other)
        {
            Vector3 localRgCentre = Vector3.zero;
            Vector3 flatLocalPos = transform.InverseTransformPoint(other.transform.position);
            flatLocalPos.x = 0;
            float flatDistToCentre = (localRgCentre - flatLocalPos).magnitude;
            //Vector3 flatDirToCentre = (localRgCentre - flatLocalPos).normalized;
            //flatDirToCentre = transform.TransformDirection(flatDirToCentre);

            if (flatDistToCentre > radius)
            {
                if (capturedGameObjects.Contains(other.gameObject))
                {
                    other.gameObject.SendMessage("OnRingworldExit", this, SendMessageOptions.DontRequireReceiver);
                    capturedGameObjects.Remove(other.gameObject);
                }
            }
            else
            {
                if (!capturedGameObjects.Contains(other.gameObject))
                {
                    other.gameObject.SendMessage("OnRingworldEnter", this, SendMessageOptions.DontRequireReceiver);
                    capturedGameObjects.Add(other.gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (capturedGameObjects.Contains(other.gameObject))
            {
                other.gameObject.SendMessage("OnRingworldExit", this, SendMessageOptions.DontRequireReceiver);
                capturedGameObjects.Remove(other.gameObject);
            }
        }
    }
}
