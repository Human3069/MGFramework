using UnityEngine.AI;

namespace MGFramework
{
    public static class AgentUtility 
    {
        public static bool IsArrived(this NavMeshAgent agent)
        {
            if (agent == null || agent.isOnNavMesh == false)
            {
                return false;
            }

            bool isWithinDistance = agent.remainingDistance <= agent.stoppingDistance;
            bool isPathPending = agent.pathPending;

            return isWithinDistance == true &&
                   isPathPending == false;
        }
    }
}
