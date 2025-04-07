using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerStateController
    {
        private PlayerData _data;

        private Damageable _handlingDamageable = null;
        private Damageable HandlingDamageable
        {
            get
            {
                return _handlingDamageable;
            }
            set
            {
                if (_handlingDamageable != value)
                {
                    _handlingDamageable = value;
                    OnHandlingDamageableChangedAsync().Forget();

                    _data._Agent.destination = _data._PlayerT.position;
                }
            }
        }

        private bool _isCanMining = false;
        private bool IsCanMining
        {
            get
            {
                return _isCanMining;
            }
            set
            {
                if (_isCanMining != value)
                {
                    _isCanMining = value;

                    if (value == true)
                    {
                        _data._Anime.StartMining();
                    }
                    else
                    {
                        _data._Anime.StopMining();
                    }
                }
            }
        }

        private Collider[] overlappedColliders;

        private async UniTaskVoid OnHandlingDamageableChangedAsync()
        {
            while (HandlingDamageable != null)
            {
                Vector3 direction = (HandlingDamageable.transform.position - _data._PlayerT.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Vector3 targetToEuler = targetRotation.eulerAngles;
                Vector3 upwardedEuler = new Vector3(0f, targetToEuler.y, 0f);
                Quaternion upwardedRotation = Quaternion.Euler(upwardedEuler);
                _data._PlayerT.rotation = Quaternion.RotateTowards(_data._PlayerT.rotation, upwardedRotation, lookAtSpeed);

                IsCanMining = (_data._Anime.IsMoving == false);

                await UniTask.WaitForFixedUpdate();
            }

            IsCanMining = false;
        }

        [SerializeField]
        private KeyframeReceiver keyframeReceiver;

        [Space(10)]
        [SerializeField]
        private float handlingRadius = 1.5f;
        [SerializeField]
        private float handlingDamage = 40f;
        [SerializeField]
        private float lookAtSpeed = 5f;

        public void OnAwake(PlayerData data)
        {
            this._data = data;

            overlappedColliders = new Collider[5];
            keyframeReceiver.OnKeyframeReachedEvent += OnKeyframeReached;
        }

        private void OnKeyframeReached(int index)
        {
            if (index == 0)
            {
                if (HandlingDamageable != null &&
                    HandlingDamageable.IsDead == false)
                {
                    HandlingDamageable.CurrentHealth -= handlingDamage;
                }
            }
        }

        public void SlowTick()
        {
            bool gotDamageable = false;
            int overlapCount = Physics.OverlapSphereNonAlloc(_data._PlayerT.position, handlingRadius, overlappedColliders);
   
            for (int i = 0; i < overlapCount; i++)
            {
                Collider overlappedCollider = overlappedColliders[i];
                if (overlappedCollider.transform != _data._PlayerT &&
                    overlappedCollider.TryGetComponent(out Damageable damageable) == true &&
                    damageable.IsDead == false)
                {
                    gotDamageable = true;
                    HandlingDamageable = damageable;

                    return;
                }
            }

            if (gotDamageable == false)
            {
                HandlingDamageable = null;
            }
        }

        public void Tick()
        {
         
        }
    }
}