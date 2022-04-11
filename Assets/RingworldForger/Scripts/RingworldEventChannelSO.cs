using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE.Events
{
    [CreateAssetMenu(fileName = "new Ringworld Event Channel", menuName = "Ringworld Forger/Event Channel")]
    public class RingworldEventChannelSO : ScriptableObject
    {
        public event EventHandler<RingworldCreateArgs> OnRingworldCreate = null;
        public void InvokeOnRingworldCreate(RingworldCreateArgs args) => OnRingworldCreate?.Invoke(this, args);
    }

    public class RingworldCreateArgs : EventArgs
    {
    }
}
