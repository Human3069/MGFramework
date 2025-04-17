using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public static class EmployeeUtility 
    {
        public static T FindNearest<T>(this Transform employeeTransform, System.Predicate<T> predicate = null) where T : Component
        {
            T[] foundComponents = Object.FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            T nearestComponent = null;
            float nearestDistance = float.MaxValue;

            foreach (T iteratingComponent in foundComponents)
            {
                if (iteratingComponent.transform == employeeTransform ||
                   (predicate != null && predicate(iteratingComponent) == false))
                {
                    continue;
                }

                float distance = Vector3.Distance(employeeTransform.position, iteratingComponent.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestComponent = iteratingComponent;
                }
            }

            return nearestComponent;
        }

        public static bool IsInAttackRange(this EmployeeContext context, EmployeeData data, Vector3 destination)
        {
            float distance = Vector3.Distance(context.Transform.position, destination);
            return distance <= data.AttackRange;
        }
    }
}