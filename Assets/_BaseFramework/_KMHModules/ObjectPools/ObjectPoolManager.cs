using _KMH_Framework.Pool.Internal;
using System.Reflection;
using UnityEngine;

namespace _KMH_Framework.Pool
{
    public enum ItemType
    {
        None = -1,

        Wood = 0
    }

    public enum FxType
    {
        PlayerMarker = 0
    }

    public enum UnitType
    {
        //
    }

    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private const string LOG_FORMAT = "<color=white><b>[ObjectPoolManager]</b></color> {0}";

        [SerializeField]
        protected EnumerablePooler<ItemType> itemPooler;
        [SerializeField]
        protected EnumerablePooler<FxType> fxPooler;
        [SerializeField]
        protected EnumerablePooler<UnitType> unitPooler;
        
        protected virtual void Awake()
        {
            FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo info in fieldInfos)
            {
                if (info.FieldType.Name.Contains(nameof(EnumerablePooler)) == true)
                {
                    EnumerablePooler enumerablePooler = info.GetValue(this) as EnumerablePooler;

                    GameObject enumerablePoolerObj = new GameObject("EnumerablePooler_" + enumerablePooler.GetEnumType().Name);
                    enumerablePoolerObj.transform.SetParent(this.transform);

                    enumerablePooler.OnAwake(enumerablePoolerObj.transform);
                }
            }
        }

        public PoolHandler GetPoolHandler(ItemType type)
        {
            return itemPooler.GetPoolHandler(type);
        }

        public PoolHandler GetPoolHandler(FxType type)
        {
            return fxPooler.GetPoolHandler(type);
        }

        public PoolHandler GetPoolHandler(UnitType type)
        {
            return unitPooler.GetPoolHandler(type);
        }
    }
}