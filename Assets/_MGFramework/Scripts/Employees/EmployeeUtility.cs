using _KMH_Framework.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public static class EmployeeUtility 
    {
        public static bool IsInAttackRange(this EmployeeContext context, EmployeeData data, Vector3 destination)
        {
            float distance = Vector3.Distance(context.Transform.position, destination);
            return distance <= data.AttackRange;
        }

        // 우선 순위 1 : opposite Input이 가장 적을 것
        // 우선 순위 2 : target Input이 가장 적을 것
        public static Payload FindPoorestPayload(this Transform employeeTransform, PoolType targetType, PoolType oppositeType, System.Predicate<Payload> predicate = null)
        {
            Payload[] foundPayloads = Object.FindObjectsByType<Payload>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            List<Payload> foundPayloadList = new List<Payload>(foundPayloads);
            foundPayloadList.Sort(SortComparison);
            int SortComparison(Payload a, Payload b)
            {
                int inputCountA = a.GetInputCount(targetType);
                int inputCountB = b.GetInputCount(targetType);

                return inputCountA.CompareTo(inputCountB);
            }

            Payload poorestPayload = null;
            int poorestCount = -1;

            foreach (Payload iteratingPayload in foundPayloadList)
            {
                if (iteratingPayload.transform == employeeTransform ||
                   (predicate != null && predicate(iteratingPayload) == false))
                {
                    continue;
                }

                float distance = Vector3.Distance(employeeTransform.position, iteratingPayload.transform.position);
                int count = iteratingPayload.GetInputCount(oppositeType);

                if (poorestCount < count)
                {
                    poorestCount = count;
                    poorestPayload = iteratingPayload;
                }
            }

            return poorestPayload;
        }
    }
}