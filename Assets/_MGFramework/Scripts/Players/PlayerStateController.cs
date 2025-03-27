using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerStateController
    {
        private Transform _playerT;
        private PlayerAnimationController _anime;

        private BaseDamageable _handlingDamageable = null;
        private BaseDamageable HandlingDamageable
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
                        _anime.StartMining();
                    }
                    else
                    {
                        _anime.StopMining();
                    }
                }
            }
        }

        private async UniTaskVoid OnHandlingDamageableChangedAsync()
        {
            while (HandlingDamageable != null)
            {
                Vector3 direction = (HandlingDamageable.transform.position - _playerT.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Vector3 targetToEuler = targetRotation.eulerAngles;
                Vector3 upwardedEuler = new Vector3(0f, targetToEuler.y, 0f);
                Quaternion upwardedRotation = Quaternion.Euler(upwardedEuler);
                _playerT.rotation = Quaternion.RotateTowards(_playerT.rotation, upwardedRotation, lookAtSpeed);

                IsCanMining = (_anime.IsMoving == false);

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

        public void OnAwake(Transform playerT, PlayerAnimationController anime)
        {
            _playerT = playerT;
            _anime = anime;

            keyframeReceiver.OnKeyframeReachedEvent += OnKeyframeReachedEvent;
        }

        private void OnKeyframeReachedEvent(int index)
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

        // Overlap과 GetComponent가 매 프레임 호출되고 있음. 오버헤드 우려됨.
        public void OnUpdate()
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(_playerT.position, handlingRadius);
            bool gotDamageable = false;

            foreach (Collider overlappedCollider in overlappedColliders)
            {
                if (overlappedCollider.TryGetComponent(out BaseDamageable damageable) == true &&
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
    }
}