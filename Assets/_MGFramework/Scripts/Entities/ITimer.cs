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

        float Normal
        {
            get;
            set;
        }
    }
}