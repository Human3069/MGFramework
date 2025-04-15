using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public interface ITimer 
    {
        float NormalizedTime
        {
            get;
            set;
        }

        Transform transform
        {
            get;
        }
    }
}
