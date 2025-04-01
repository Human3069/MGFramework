using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public interface IMonsterState 
    {
        void Enter();
        void Exit();
        void SlowTick(); // 0.5초마다 호출
        void Tick(); // FixedUpdate 호출
    }
}