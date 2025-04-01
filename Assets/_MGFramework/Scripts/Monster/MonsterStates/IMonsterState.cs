using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public interface IMonsterState 
    {
        void Enter();
        void Exit();
        void SlowTick(); // 0.5�ʸ��� ȣ��
        void Tick(); // FixedUpdate ȣ��
    }
}