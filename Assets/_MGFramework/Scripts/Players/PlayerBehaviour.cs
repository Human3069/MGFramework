using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerBehaviour
    {
        private PlayerData _data;

        [SerializeField]
        private float slowTickRate = 0.5f;

        [Space(10)]
        [SerializeField]
        private float minAttackRange = 1f;
        [SerializeField]
        private float maxAttackRange = 1.25f;

        [Space(10)]
        [SerializeField]
        private float attackDamage = 30f;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private Damageable _targetDamageable;
        private Damageable TargetDamageable
        {
            get
            {
                return _targetDamageable;
            }
            set
            {
                if (_targetDamageable != value)
                {
                    _targetDamageable = value;

                    OnTargetChanged?.Invoke(value);
                    OnTargetChangedAsync().Forget();
                }
            }
        }

        private async UniTaskVoid OnTargetChangedAsync()
        {
            while (TargetDamageable != null)
            {
                OnTargetStay?.Invoke(TargetDamageable);
                await UniTask.Yield();
            }
        }

        private bool isAlived = false;
        private Collider[] overlapCollider = new Collider[5];

        public delegate void TargetChangedDelegate(Damageable damageable);
        public event TargetChangedDelegate OnTargetChanged;

        public delegate void TargetStayDelegate(Damageable damageable);
        public event TargetStayDelegate OnTargetStay; // 매 프레임 호출

        public void OnAwake(PlayerData data)
        {
            this._data = data;
        }

        public void OnAlived()
        {
            isAlived = true;
            OnAlivedAsync().Forget();
        }

        private async UniTaskVoid OnAlivedAsync()
        {
            while (isAlived == true)
            {
                if (TargetDamageable == null)
                {
                    Vector3 middlePoint = _data._Transform.position;
                    int overlapCount = Physics.OverlapSphereNonAlloc(middlePoint, maxAttackRange, overlapCollider);
                    for (int i = 0; i < overlapCount; i++)
                    {
                        Collider collider = overlapCollider[i];
                        if (collider.TryGetComponent(out Damageable damageable) == true &&
                            damageable.transform != _data._Transform &&
                            damageable.IsDead == false)
                        {
                            TargetDamageable = damageable;
                        }
                    }
                }
                else if (TargetDamageable.IsDead == true ||
                         TargetDamageable.DistanceWithPlayer() > maxAttackRange)
                {
                    TargetDamageable = null;
                }

                await UniTask.WaitForSeconds(slowTickRate);
            }
        }

        public void OnDead()
        {
            isAlived = false;
        }

        public void OnAttacked()
        {
            if (TargetDamageable != null)
            {
                TargetDamageable.CurrentHealth -= attackDamage;
            }
        }
    }
}
