using UnityEngine;

namespace MGFramework
{
    public static class EmployeeUtility 
    {
        public static IEmployeeState GetState(this EmployeeState state)
        {
            IEmployeeState stateMachine;

            switch (state)
            {
                case EmployeeState.None:
                    stateMachine = null;
                    break;

                case EmployeeState.FindWork:
                    stateMachine = new FindWorkEmployeeState();
                    break;

                case EmployeeState.MoveToWork:
                    stateMachine = new MoveToWorkEmployeeState();
                    break;

                case EmployeeState.Work:
                    stateMachine = new WorkEmployeeState();
                    break;

                case EmployeeState.PickUpItems:
                    stateMachine = new PickUpItemsEmployeeState();
                    break;

                case EmployeeState.FindStorage:
                    stateMachine = new FindStorageEmployeeState();
                    break;

                case EmployeeState.MoveToStorage:
                    stateMachine = new MoveToStorageEmployeeState();
                    break;

                case EmployeeState.StoreItems:
                    stateMachine = new StoreItemsEmployeeState();
                    break;

                default:
                    throw new System.NotImplementedException("");
            }

            return stateMachine;
        }

        public static T FindNearest<T>(this Employee employee, System.Predicate<T> predicate = null) where T : Component
        {
            T[] foundComponents = Object.FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            T nearestComponent = null;
            float nearestDistance = float.MaxValue;

            foreach (T iteratingComponent in foundComponents)
            {
                if (iteratingComponent.transform == employee.transform ||
                   (predicate != null && predicate(iteratingComponent) == false))
                {
                    continue;
                }

                float distance = Vector3.Distance(employee.transform.position, iteratingComponent.transform.position);
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