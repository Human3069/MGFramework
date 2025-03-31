using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    // hook ����� ���� �������̽� (�Ƚᵵ ����)
    public interface IPoolable
    {
        void OnBeforeEnable();
        void OnAfterEnable();

        void OnBeforeDisable();
        void OnAfterDisable();
    }
}