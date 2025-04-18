using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public static class StackableUtility 
    {
        public static bool IsEachPoppable(this BaseStackableStore[] stores)
        {
            foreach (BaseStackableStore store in stores)
            {
                if (store.IsPoppable() == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
