using UnityEngine;

namespace MGFramework
{
    public static class HunterUtility
    {
        public static bool IsInAttackRange(this HunterContext context, HunterData data, Vector3 destination)
        {
            float distance = Vector3.Distance(context.Transform.position, destination);
            return distance <= data.AttackRange;
        }

        public static void LookTowardTarget(this HunterContext context, float lookAtSpeed)
        {
            if (context == null)
            {
                Debug.LogError("context == null");
                return;
            }
            else if (context.TargetDamageable == null)
            {
                Debug.LogError("context.TargetDamageable == null");
                return;
            }

            Quaternion lookRotation = Quaternion.LookRotation(context.TargetDamageable.transform.position - context.Transform.position);
            context.Transform.rotation = Quaternion.RotateTowards(context.Transform.rotation, lookRotation, lookAtSpeed);
        }

        public static void MoveToTargetForward(this HunterContext context, float margin)
        {
            if (context == null)
            {
                Debug.LogError("context == null");
                return;
            }
            else if (context.TargetDamageable == null)
            {
                Debug.LogError("context.TargetDamageable == null");
                return;
            }

            Vector3 targetPos = context.TargetDamageable.transform.position;
            Vector3 direction = (context.Transform.position - targetPos).normalized;
            Vector3 forwardedPos = targetPos + (direction * margin);

            context.Agent.destination = forwardedPos;
        }
    }
}