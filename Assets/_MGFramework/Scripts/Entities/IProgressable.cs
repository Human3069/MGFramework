using UnityEngine;

namespace MGFramework
{
    public interface IProgressable
    {
        GameObject gameObject
        {
            get;
        }

        Transform transform
        {
            get;
        }

        float MaxProgress
        {
            get;
        }

        float _CurrentProgress
        {
            get;
            set;
        }

        public float CurrentProgress
        {
            get
            {
                return _CurrentProgress;
            }
            set
            {
                if (_CurrentProgress != value)
                {
                    _CurrentProgress = Mathf.Clamp(value, 0f, MaxProgress);
                    ProgressChanged();
                }
            }
        }

        public delegate void ProgressChangedDelegate(float maxProgress, float currentProgress);
        ProgressChangedDelegate OnProgressChangedEvent
        {
            get;
            set;
        }

        float OffsetHeight
        {
            get;
        }

        public void ProgressChanged()
        {
            OnProgressChangedEvent?.Invoke(MaxProgress, CurrentProgress);
            OnProgressChanged();
        }

        void OnProgressChanged();
    }
}