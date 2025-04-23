using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class PlayerBehaviour
    {
        private PlayerContext _context;
        private PlayerData _data;

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
                    if (_targetDamageable != null)
                    {
                        _targetDamageable.Exit();
                    }

                    _targetDamageable = value;
                    
                    if (_targetDamageable != null)
                    {
                        _targetDamageable.Enter();
                    }

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

        public PlayerBehaviour(PlayerContext context, PlayerData data)
        {
            this._context = context;
            this._data = data;

            this._context.Receiver.OnKeyframeReachedEvent += OnKeyframeReached;
            this._context.Damageable.OnAlivedEvent += OnAlived;
            this._context.Damageable.OnDeadEvent += OnDead;

            OnAlived(); // 최초에 강제 호출
        }

        private void OnKeyframeReached(int index)
        {
            if (TargetDamageable != null)
            {
                TargetDamageable.TakeDamage(_data.AttackDamage, _context.Damageable);
            }
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
                    Vector3 middlePoint = _data.Transform.position;

                    int layerMask = ~(1 << 3);
                    int overlapCount = Physics.OverlapSphereNonAlloc(middlePoint, _data.AttackRange.x, overlapCollider, layerMask);
                    for (int i = 0; i < overlapCount; i++)
                    {
                        Collider collider = overlapCollider[i];

                        if (collider.TryGetComponent(out Damageable damageable) == true &&
                            damageable.transform != _data.Transform &&
                            damageable.IsDead == false &&
                            damageable.GetOwnerType() != OwnerType.Players)
                        {
                            TargetDamageable = damageable;
                        }
                    }
                }
                else if (TargetDamageable.IsDead == true ||
                         TargetDamageable.DistanceWithPlayer() > _data.AttackRange.y)
                {
                    TargetDamageable = null;
                }

                await UniTask.WaitForSeconds(_data.SlowTickRate);
            }
        }

        public void OnDead()
        {
            isAlived = false;
        }
    }
}
