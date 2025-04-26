using _KMH_Framework.Pool;
using UnityEditor;
using UnityEngine;

namespace MGFramework
{
    public class Stackable : MonoBehaviour
    {
        public PoolType StackablePoolType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            string poolTypeName = StackablePoolType.ToString();
            if (poolTypeName.Contains("Stackable") == false)
            {
                Debug.LogError("Stackable ������Ʈ�� StackablePoolType�� �ݵ�� Stackable ���ԵǴ� ���� �����ؾ� �մϴ�.");
                Selection.objects = new Object[] { this.gameObject };
            }
        }
#endif
    }
}
