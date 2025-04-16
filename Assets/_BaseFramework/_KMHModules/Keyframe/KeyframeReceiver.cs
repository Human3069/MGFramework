using UnityEngine;

namespace _KMH_Framework
{
    public class KeyframeReceiver : MonoBehaviour
    {
        public delegate void KeyframeReached(int index);
        public event KeyframeReached OnKeyframeReachedEvent;

        private void OnKeyframeReached(int index)
        {
            OnKeyframeReachedEvent?.Invoke(index);
        }
    }
}