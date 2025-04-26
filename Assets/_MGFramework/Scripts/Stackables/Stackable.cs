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
                Debug.LogError("Stackable 컴포넌트의 StackablePoolType은 반드시 Stackable 포함되는 것을 지정해야 합니다.");
                Selection.objects = new Object[] { this.gameObject };
            }
        }
#endif
    }
}
