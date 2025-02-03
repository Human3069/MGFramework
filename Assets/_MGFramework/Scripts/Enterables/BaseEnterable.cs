using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseEnterable : Poolable
    {
        protected abstract void OnTriggerEnter(Collider collider);

        protected abstract void OnTriggerExit(Collider collider);
    }
}