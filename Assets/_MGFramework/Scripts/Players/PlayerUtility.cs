using UnityEngine;

namespace MGFramework
{
    public static class PlayerUtility 
    {
        public static float DistanceWithPlayer(this Damageable damageable)
        {
            if (Player.Instance == null)
            {
                Debug.LogError("Player == null");
                return 0f;
            }
            else
            {
                Vector3 playerPos = Player.Instance.transform.position;
                Vector3 closestPoint = damageable.GetClosestPoint(playerPos);

                return Vector3.Distance(closestPoint, playerPos);
            }
        }
    }
}
