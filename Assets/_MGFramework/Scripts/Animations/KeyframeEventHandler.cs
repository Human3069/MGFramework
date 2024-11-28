using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public class KeyframeEventHandler : MonoBehaviour
    {
        public delegate void KeyframeReached();
        public event KeyframeReached OnKeyframeReached;

        public delegate void KeyframeReached_Int(int typeIndex);
        public event KeyframeReached_Int OnKeyframeReached_Int;

        public void OnKeyframeReach()
        {
            if (OnKeyframeReached != null)
            {
                OnKeyframeReached();
            }
        }

        public void OnKeyframeReach_Int(int typeIndex)
        {
            if (OnKeyframeReached_Int != null)
            {
                OnKeyframeReached_Int(typeIndex);
            }
        }
    }
}