using UnityEngine;

namespace MGFramework
{
    public static class MonsterUtility
    {
        public static bool IsInAlertRange(this MonsterContext context, MonsterData data)
        {
            if (context == null)
            {
                Debug.LogError("context == null");
                return false;
            }
            else if (context.TargetDamageable == null)
            {
                Debug.LogError("context.TargetDamageable == null");
                return false;
            }

            float distance = Vector3.Distance(context.Transform.position, context.TargetDamageable.transform.position);
            return distance <= data.MaxAlertRange;
        }

        public static bool IsInAlertRange(this MonsterContext context, MonsterData data, Vector3 targetPos)
        {
            if (context == null)
            {
                Debug.LogError("context == null");
                return false;
            }

            float distance = Vector3.Distance(context.Transform.position, targetPos);
            return distance <= data.MaxAlertRange;
        }

        public static bool IsInMoveToAttackRange(this MonsterContext context, MonsterData data)
        {
            if (context == null)
            {
                Debug.LogError("context == null");
                return false;
            }
            else if (context.TargetDamageable == null)
            {
                Debug.LogError("context.TargetDamageable == null");
                return false;
            }

            float distance = Vector3.Distance(context.Transform.position, context.TargetDamageable.transform.position);
            return distance <= data.MaxMoveToAttackRange;
        }

        public static bool IsInAttackRange(this MonsterContext context, MonsterData data)
        {
            if (context == null)
            {
                Debug.LogError("context == null");
                return false;
            }
            else if (context.TargetDamageable == null)
            {
                Debug.LogError("context.TargetDamageable == null");
                return false;
            }

            float distance = Vector3.Distance(context.Transform.position, context.TargetDamageable.transform.position);
            return distance <= data.MaxAttackRange;
        }

        public static void LookTowardTarget(this MonsterContext context, float lookAtSpeed)
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

        public static void MoveToTargetForward(this MonsterContext context, float margin)
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

        public static bool IsPlaying(this Animator animator, int layerIndex)
        {
            AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
           
            return transitionInfo.normalizedTime != 0f ||
                   stateInfo.normalizedTime < 1f;
        }
    }
}