using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public static class EmployerUtility 
    {
        public static Vector3 GetForwardPositionTowardsHarvestable(this Transform monsterT, Harvestable harvestable, float offset)
        {
            if (monsterT == null)
            {
                Debug.LogError("monsterT == null");
                return Vector3.zero;
            }
            else if (harvestable == null)
            {
                Debug.LogError("harvestable == null");
                return Vector3.zero;
            }

            Vector3 targetPos = harvestable.transform.position;
            Vector3 direction = (monsterT.position - targetPos).normalized;
            Vector3 resultPos = targetPos + direction * offset;

            return resultPos;
        }

        public static bool IsWorkablePos(this EmployerData data, Harvestable targetHarvestable)
        {
            Vector3 targetPos = data._Transform.GetForwardPositionTowardsHarvestable(targetHarvestable, 1f);
            float distance = Vector3.Distance(data._Transform.position, targetPos);

            return distance <= data._HandlingDistance;
        }
    }
}