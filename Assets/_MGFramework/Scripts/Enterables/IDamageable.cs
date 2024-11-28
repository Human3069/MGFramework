using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public interface IDamageable
    {
        delegate void Damaged(float normalized);
        event Damaged OnDamagedCallback;

        DamageableType _Type
        {
            get;
        }

        Transform transform
        {
            get;
        }

        float CurrentHealth
        {
            get;
            set;
        }
    }
}