using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using geniikw.DataRenderer2D.Hole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CampfirePayloader : BasePayloader
    {
        [Header("=== CampfirePayloader ===")]
        [SerializeField]
        private Transform flameT;
        [SerializeField]
        private Transform storeT;
        [SerializeField]
        private Light flameLight;
        [SerializeField]
        private ParticleSystem flameParticle;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private float _flameIntensityNormal = 1f;
        private float FlameIntensityNormal
        {
            get
            {
                return _flameIntensityNormal;
            }
            set
            {
                if (_flameIntensityNormal != value)
                {
                    _flameIntensityNormal = value;

                    if (inputDic[ItemType.Wood] > 0 &&
                        value <= (1f - increasePerCount))
                    {
                        RemoveStore(ItemType.Wood);
                    }

#pragma warning disable CS0618
                    flameLight.intensity = maxLightIntensity * value;
                    flameParticle.emissionRate = maxParticleRateOverTime * value;
#pragma warning restore CS0618
                }
            }
        }


        [SerializeField]
        private float decreasePerSec = 0.01f;
        [SerializeField]
        private float increasePerCount = 0.4f;

        [Space(10)]
        [SerializeField]
        private float storingHeight = 0.25f;
        [SerializeField]
        private float removeDelay = 0.5f;

        private Dictionary<ItemType, List<Item>> instanceDic = new Dictionary<ItemType, List<Item>>();
        private float maxLightIntensity;
        private float maxParticleRateOverTime;

        private void Awake()
        {
            maxLightIntensity = flameLight.intensity;
            maxParticleRateOverTime = flameParticle.emission.rateOverTime.constant;
        }

        private void Update()
        {
            FlameIntensityNormal = Mathf.Clamp01(_flameIntensityNormal - decreasePerSec * Time.deltaTime);
          
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Time.timeScale += 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Time.timeScale = 1f;
            }
        }

        protected override void OnInput()
        {
            AddStore(ItemType.Wood);

            if (inputDic[ItemType.Wood] == 1 &&
                _flameIntensityNormal <= (1f - increasePerCount))
            {
                RemoveStore(ItemType.Wood);
            }
        }

        private void AddStore(ItemType type)
        {
            int itemIndex = inputDic[type] - 1;
            if (instanceDic.ContainsKey(type) == false)
            {
                instanceDic.Add(type, new List<Item>());
            }

            type.EnablePool<Item>(OnBeforeEnable);
            void OnBeforeEnable(Item item)
            {
                item.IsOnInventory = true;

                Vector3 pos = storeT.position + (storeT.up * storingHeight * itemIndex);
                item.transform.parent = storeT;
                item.transform.position = pos;
                item.transform.localScale = Vector3.one;
                item.transform.localEulerAngles = Vector3.zero;

                instanceDic[type].Add(item);
            }
        }

        private void RemoveStore(ItemType type)
        {
            if (instanceDic.ContainsKey(type) == true)
            {
                List<Item> itemList = instanceDic[type];
                if (itemList.Count > 0)
                {
                    Item item = itemList[itemList.Count - 1];
                    itemList.Remove(item);

                    item.DisableAsync(isImmediately : true).Forget();
                }

                _flameIntensityNormal = Mathf.Clamp01(_flameIntensityNormal + increasePerCount);
                inputDic[type]--;
            }

            RemoveStoreAsync(type).Forget();
        }

        private async UniTaskVoid RemoveStoreAsync(ItemType type)
        {
            if (inputDic[type] > 0 &&
                _flameIntensityNormal <= (1f - increasePerCount))
            {
                await UniTask.WaitForSeconds(removeDelay);

                RemoveStore(type);
            }
        }
    }
}