using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public class KeyframeEventHandler : MonoBehaviour
    {
        public delegate void KeyframeReached();
        public event KeyframeReached OnKeyframeReached;

        public void OnKeyframeReach()
        {
            if (OnKeyframeReached != null)
            {
                OnKeyframeReached();
            }
        }
    }
}