using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerData 
    {
        [HideInInspector]
        public PlayerAnimationController _Anime;

        public Transform _PlayerT;
        public NavMeshAgent _Agent;
    }
}