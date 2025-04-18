using UnityEngine;

namespace MGFramework
{
    public static class GlobalUtility 
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
    }
}
