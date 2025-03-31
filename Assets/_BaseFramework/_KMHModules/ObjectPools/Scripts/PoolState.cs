using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework.Pool
{
    public struct PoolState 
    {
        public PoolType PoolType;
        public int CurrentCount;
        public int MaxCount;

        public PoolState(PoolType poolType, int currentCount, int maxCount)
        {
            this.PoolType = poolType;
            this.CurrentCount = currentCount;
            this.MaxCount = maxCount;
        }

        public override string ToString()
        {
            return "PoolType : " + PoolType + ", CurrentCount : " + CurrentCount + ", MaxCount : " + MaxCount;
        }
    }
}