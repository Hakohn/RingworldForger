using ChironPE.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(Rigidbody))]
    public class CentrifugalBody : MonoBehaviour
    {
        [HideInInspector]
        public Rigidbody rb = null;

        public bool alignUpToRingCentre = false;
        //[HideInInspector]
        //public Ringworld parentRingworld = null;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        private void FixedUpdate()
        {
            //Vector3 centrifugalForce = rb.mass * ((rb.velocity.magnitude * rb.velocity.magnitude)/())
            //rb.AddForce
        }

        private void OnTriggerEnter(Collider other)
        {
            //if(other.TryGetComponent(out Ringworld ringworld))
            //{
            //    parentRingworld = ringworld;
            //}
        }
    }
}
