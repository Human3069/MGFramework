using _KMH_Framework.Pool;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class CustomerData 
    {
        public bool IsShowLog;
        public float LookAtSpeed;
        public float FoodConsumeSpeed;
        [MinMaxSlider(1f, 60f)]
        public Vector2 EatingDurationRange;
        public int GoldPerFood;

        [Space(10)]
        [SerializedDictionary("Type", "Count")]
        public SerializedDictionary<PoolType, Vector2Int> RequirementDic = new SerializedDictionary<PoolType, Vector2Int>();

        [Space(10)]
        public Canvas DesiredCanvas;
        public TextMeshProUGUI DesiredText;
    }
}