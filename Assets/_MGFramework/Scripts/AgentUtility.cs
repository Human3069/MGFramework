using UnityEngine.AI;

namespace MGFramework
{
    public static class AgentUtility 
    {
        public static bool IsArrived(this NavMeshAgent agent)
        {
            bool isWithinDistance = agent.remainingDistance <= agent.stoppingDistance;
            bool isPathPending = agent.pathPending;

            return isWithinDistance == true &&
                   isPathPending == false;
        }
    }
}
