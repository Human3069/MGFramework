using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class CustomerData 
    {
        public bool IsShowLog;
        [MinMaxSlider(1, 10)]
        public Vector2Int DesiredFoodCountRange;
        public float LookAtSpeed;
    }
}