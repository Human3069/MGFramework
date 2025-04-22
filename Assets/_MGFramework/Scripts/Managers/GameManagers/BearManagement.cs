using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class BearManagement 
    {
        [SerializeField]
        private Transform enablePoint;
        [SerializeField]
        private float enableRadius;

        [Space(10)]
        [SerializeField]
        [MinMaxSlider(1f, 60f)]
        private Vector2 spawnDelayRange = new Vector2(5f, 10f);
        [SerializeField]
        private int spawnCount = 8;

        private GameManager _manager;
        private List<Damageable> alivedBearList = new List<Damageable>();

        public void OnAwake(GameManager manager)
        {
            this._manager = manager;

            OnAwakeAsync().Forget();
        }

        private async UniTaskVoid OnAwakeAsync()
        {
            while (_manager.enabled == true)
            {
                float spawnDelay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
                await UniTask.WaitForSeconds(spawnDelay);

                if (alivedBearList.Count < spawnCount)
                {
                    EnableBear();
                }
            }
        }

        public void DrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enablePoint.position, enableRadius);
        }

        public void EnableBear()
        {
            Vector3 randomizedSphere = Random.insideUnitSphere * enableRadius;
            Vector3 randomizedPos = enablePoint.position + randomizedSphere;

            Ray upsideRay = new Ray(randomizedPos, Vector3.up);
            Ray downsideRay = new Ray(randomizedPos, Vector3.down);

            int layerMask = 1 << 3;
            Vector3? hitPoint = null;
            if (Physics.Raycast(upsideRay, out RaycastHit upsideHit, Mathf.Infinity, layerMask) == true)
            {
                hitPoint = upsideHit.point;
            }
            else if (Physics.Raycast(downsideRay, out RaycastHit downsideHit, Mathf.Infinity, layerMask) == true)
            {
                hitPoint = downsideHit.point;
            }

            if (hitPoint != null)
            {
                PoolType.Bear.EnablePool(OnBeforeEnablePool);
                void OnBeforeEnablePool(GameObject obj)
                {
                    obj.transform.position = hitPoint.Value;
                    obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);

                    Damageable damageable = obj.GetComponent<Damageable>();
                    damageable.OnDeadEvent += OnBearDead;
                    void OnBearDead()
                    {
                        damageable.OnDeadEvent -= OnBearDead;
                        alivedBearList.Remove(damageable);
                    }

                    damageable.Alive();
                    alivedBearList.Add(damageable);
                }
            }
        }

       
    }
}
