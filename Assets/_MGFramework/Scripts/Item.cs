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
                Debug.LogError("Stack ������Ʈ�� StackPoolType�� �ݵ�� Stack�� ���ԵǴ� ���� �����ؾ� �մϴ�.");
            }
        }
#endif
    }
}
