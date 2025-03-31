using _KMH_Framework.Pool;
using UnityEngine;

namespace _KMH_Framework.Pool
{
    [CreateAssetMenu(fileName = "PoolableScriptableObject", menuName = "_KMH_Framework/PoolableSO", order = 0)]
    public class PoolableScriptableObject : ScriptableObject
    {
        [SerializeField]
        internal Poolable poolable;
    }
}