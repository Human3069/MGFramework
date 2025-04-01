using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public static class MonsterUtility 
    {
        public static float GetDistanceToPlayer(this Transform monsterT)
        {
            if (PlayerController.Instance == null)
            {
                Debug.LogError("PlayerController.Instance is null");
                return float.MaxValue;
            }

            return Vector3.Distance(monsterT.position, PlayerController.Instance.transform.position);
        }

        public static void RotateTowardsPlayer(this Transform monsterT, float speed)
        {
            if (PlayerController.Instance == null)
            {
                Debug.LogError("PlayerController.Instance is null");
                return;
            }

            Vector3 playerPos = PlayerController.Instance.transform.position;
            Vector3 direction = (playerPos - monsterT.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(direction);

            monsterT.rotation = Quaternion.Slerp(monsterT.rotation, targetRot, speed);
        }

        public static Vector3 GetForwardPositionTowardsPlayer(this Transform monsterT, float offset)
        {
            if (PlayerController.Instance == null)
            {
                Debug.LogError("PlayerController.Instance is null");
                return monsterT.position;
            }

            Vector3 playerPos = PlayerController.Instance.transform.position;
            Vector3 direction = (monsterT.position - playerPos).normalized;
            Vector3 resultPos = playerPos + direction * offset;

            return resultPos;
        }
    }
}