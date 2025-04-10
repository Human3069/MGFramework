using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public interface ITimer 
    {
        Transform transform
        {
            get;
        }

        GameObject gameObject
        {
            get;
        }

        float TimerOffsetHeight
        {
            get;
        }

        float _Normal
        {
            get;
            set;
        }

        public float Normal
        {
            get
            {
                return _Normal;
            }
            set
            {
                if (_Normal != value)
                {
                    _Normal = value;
                    OnNormalChangedEvent?.Invoke(value);
                }
            }
        }

        public delegate void NormalChangedDelegate(float normal);
        NormalChangedDelegate OnNormalChangedEvent
        {
            get;
            set;
        }
    }
}