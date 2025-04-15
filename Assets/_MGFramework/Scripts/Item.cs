using _KMH_Framework.Pool;
using UnityEngine;

namespace MGFramework
{
    public class Item : MonoBehaviour
    {
        public PoolType ItemPoolType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            string poolTypeName = ItemPoolType.ToString();
            if (poolTypeName.Contains("Item") == false)
            {
                Debug.LogError("Stack 컴포넌트의 StackPoolType은 반드시 Stack이 포함되는 것을 지정해야 합니다.");
            }
        }
#endif
    }
}
