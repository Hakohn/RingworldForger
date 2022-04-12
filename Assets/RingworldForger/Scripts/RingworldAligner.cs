using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(CentrifugalBody))]
    public class RingworldAligner : MonoBehaviour
    {
        [HideInInspector]
        public CentrifugalBody cb = null;
        private Ringworld rg = null;

        private void Awake()
        {
            cb = GetComponent<CentrifugalBody>();
        }
        private void FixedUpdate()
        {
            if (rg != null) UpdateRingworldAlignment();
        }
        private void UpdateRingworldAlignment()
        {
            // Ringworld Alignment
            if (rg != null)
            {
                Vector3 localRgCentre = Vector3.zero;
                Vector3 flatLocalPos = rg.transform.InverseTransformPoint(transform.position);
                flatLocalPos.x = 0;
                Vector3 flatDirToCentre = (localRgCentre - flatLocalPos).normalized;
                flatDirToCentre = rg.transform.TransformDirection(flatDirToCentre);

                cb.rb.MoveRotation(Quaternion.FromToRotation(
                    transform.up,
                    flatDirToCentre
                ) * cb.rb.rotation);
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
