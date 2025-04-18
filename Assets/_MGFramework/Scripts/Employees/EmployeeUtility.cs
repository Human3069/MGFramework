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
    }
}