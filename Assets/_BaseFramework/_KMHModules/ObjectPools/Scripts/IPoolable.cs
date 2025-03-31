using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    // hook 기능을 위한 인터페이스 (안써도 무방)
    public interface IPoolable
    {
        void OnBeforeEnable();
        void OnAfterEnable();

        void OnBeforeDisable();
        void OnAfterDisable();
    }
}