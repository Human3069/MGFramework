using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CampfirePayloader : BasePayloader, IProgressable
    {
        [Header("=== IProgressable")]
        [SerializeField]
        private float _maxProgress = 1f;
        public float MaxProgress => _maxProgress;

        [ReadOnly]
        [SerializeField]
        private float _currentProgress = 0f;
        public float _CurrentProgress
        {
            get => _currentProgress;
            set => _currentProgress = value;
        }

        private IProgressable.ProgressChangedDelegate _onProgressChangedEvent;
        public IProgressable.ProgressChangedDelegate OnProgressChangedEvent
        {
            get => _onProgressChangedEvent;
            set => _onProgressChangedEvent = value;
        }

        [SerializeField]
        private float _offsetHeight = 0f;
        public float OffsetHeight => _offsetHeight;

        [Header("=== CampfirePayloader ===")]
        [SerializeField]
        private Transform flameT;
        [SerializeField]
        private Light flameLight;
        [SerializeField]
        private ParticleSystem flameParticle;

        [Space(10)]
        [SerializeField]
        private float decreasePerSec = 0.01f;
        [SerializeField]
        private float increasePerCount = 0.4f;

        [Space(10)]
        [SerializeField]
        private float storingHeight = 0.25f;
        [SerializeField]
        private float removeDelay = 0.5f;

        private Dictionary<PoolType, List<Item>> itemInstanceDic = new Dictionary<PoolType, List<Item>>();
        private float maxLightIntensity;
        private float maxParticleRateOverTime;

        private IProgressable thisProgressable;
        private bool isComsuming = false;

        private void Awake()
        {
            isComsuming = false;

            maxLightIntensity = flameLight.intensity;
            maxParticleRateOverTime = flameParticle.emission.rateOverTime.constant;

            thisProgressable = this;
            thisProgressable.CurrentProgress = MaxProgress;
        }

        private void Update()
        {
            thisProgressable.CurrentProgress = Mathf.Clamp01(thisProgressable.CurrentProgress - decreasePerSec * Time.deltaTime);

            RemoveStore(PoolType.Wood, ItemTypeToAction(PoolType.Wood), () => ItemTypeToPredicate(PoolType.Wood)).Forget();
            RemoveStore(PoolType.RawMeat, ItemTypeToAction(PoolType.RawMeat), () => ItemTypeToPredicate(PoolType.RawMeat)).Forget();

            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Time.timeScale += 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Time.timeScale = 1f;
            }
        }

        public void OnProgressChanged()
        {
#pragma warning disable CS0618
            flameLight.intensity = maxLightIntensity * thisProgressable.CurrentProgress;
            flameParticle.emissionRate = maxParticleRateOverTime * thisProgressable.CurrentProgress;
#pragma warning restore CS0618
        }

        protected override void OnInput(PoolType type)
        {
            AddStore(type);
        }

        private void AddStore(PoolType type)
        {
            int itemIndex = inputDic[type].Count - 1;
            if (itemInstanceDic.ContainsKey(type) == false)
            {
                itemInstanceDic.Add(type, new List<Item>());
            }

            ItemData data = inputDic[type];
            Transform storeT = data.StoreT;

            type.EnablePool(OnBeforeEnable);
            void OnBeforeEnable(GameObject poolObj)
            {
                Item poolItem = poolObj.GetComponent<Item>();
                poolItem.IsOnInventory = true;

                Vector3 pos = storeT.position + (storeT.up * storingHeight * itemIndex);
                poolObj.transform.parent = storeT;
                poolObj.transform.position = pos;
                poolObj.transform.localScale = Vector3.one;
                poolObj.transform.localEulerAngles = Vector3.zero;

                itemInstanceDic[type].Add(poolItem);
            }
        }

        private async UniTaskVoid RemoveStore(PoolType type, Action onRemovedAction, Func<bool> predicate)
        {
            if (predicate.Invoke() == false)
            {
                return;
            }

            isComsuming = true;
            if (itemInstanceDic.ContainsKey(type) == true)
            {
                List<Item> itemList = itemInstanceDic[type];
                if (itemList.Count > 0)
                {
                    Item item = itemList[itemList.Count - 1];
                    itemList.Remove(item);

                    item.DisableAsync(isImmediately: true).Forget();
                }

                onRemovedAction.Invoke();
                inputDic[type].Count--;
            }

            await UniTask.WaitForSeconds(removeDelay);

            isComsuming = false;
            if (predicate.Invoke() == true)
            {
                RemoveStore(type, onRemovedAction, predicate).Forget();
            }
        }

        private Action ItemTypeToAction(PoolType type)
        {
            switch (type)
            {
                case PoolType.Wood:
                    return OnRemovedWoodAction;

                case PoolType.RawMeat:
                    return OnRemovedRawMeatAction;

                default:
                    throw new NotImplementedException();
            }
        }

        private bool ItemTypeToPredicate(PoolType type)
        {
            switch (type)
            {
                case PoolType.Wood:
                    return thisProgressable.CurrentProgress <= (1f - increasePerCount) &&
                           isComsuming == false &&
                           inputDic[PoolType.Wood].Count > 0;

                case PoolType.RawMeat:
                    return thisProgressable.CurrentProgress > 0f &&
                           isComsuming == false &&
                           inputDic[PoolType.RawMeat].Count > 0;

                default:
                    throw new NotImplementedException();
            }
        }

        private void OnRemovedWoodAction()
        {
            thisProgressable.CurrentProgress = Mathf.Clamp01(thisProgressable.CurrentProgress + increasePerCount);
        }

        private void OnRemovedRawMeatAction()
        {
            foreach (KeyValuePair<PoolType, ItemData> pair in outputDic)
            {
                Transform storeT = pair.Value.StoreT;
                int itemIndex = pair.Value.Count;
                pair.Value.Count++;

                pair.Key.EnablePool(OnBeforeEnable);
                void OnBeforeEnable(GameObject poolObj)
                {
                    Item poolItem = poolObj.GetComponent<Item>();
                    poolItem.IsOnInventory = true;

                    Vector3 pos = storeT.position + (storeT.up * storingHeight * itemIndex);
                    poolObj.transform.parent = storeT;
                    poolObj.transform.position = pos;
                    poolObj.transform.localScale = Vector3.one;
                    poolObj.transform.localEulerAngles = Vector3.zero;

                    if (itemInstanceDic.ContainsKey(pair.Key) == false)
                    {
                        itemInstanceDic.Add(pair.Key, new List<Item>());
                    }

                    itemInstanceDic[pair.Key].Add(poolItem);
                }
            }
        }
    }
}