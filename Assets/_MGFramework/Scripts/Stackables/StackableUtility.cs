using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public static class StackableUtility 
    {
        public static bool IsAllPoppable(this BaseStackableStore[] stores)
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

        public static void PopAll(this BaseStackableStore[] stores)
        {
            foreach (BaseStackableStore store in stores)
            {
                store.Pop();
            }
        }

        public static void AddAll(this BaseStackableStore[] stores)
        {
            foreach (BaseStackableStore store in stores)
            {
                store.Add();
            }
        }
    }
}
