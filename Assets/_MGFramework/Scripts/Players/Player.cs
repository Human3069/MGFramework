using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoSingleton<Player>
    {
        [SerializeField]
        private KeyframeReceiver receiver;
        [SerializeField]
        private Damageable damageable;
        [SerializeField]
        private PlayerData data;

        [Space(10)]
        [SerializeField]
        private PlayerMovement _movement;
        [SerializeField]
        private PlayerBehaviour _behaviour;
        [SerializeField]
        private PlayerAnimator _animator;
        [SerializeField]
        private PlayerCamera _camera;

        private void Awake()
        {
            damageable.OnAlivedEvent += OnAlived;
            damageable.OnDamagedEvent += OnDamaged;
            damageable.OnDeadEvent += OnDead;

            data.Initialize(_movement, _behaviour, _animator, _camera);

            _movement.OnAwake(data);
            _behaviour.OnAwake(data);
            _animator.OnAwake(data);
            _camera.OnAwake(data);

            receiver.OnKeyframeReachedEvent += OnKeyframeReached;

            OnAlived(); // 강제 호출
            AwakeAsync().Forget();
        }

        private void OnKeyframeReached(int index)
        {
            _behaviour.OnAttacked();
        }

        private void OnAlived()
        {
            _behaviour.OnAlived();
        }

        private void OnDamaged()
        {
            
        }

        private void OnDead()
        {
            _behaviour.OnDead();
        }

        private async UniTaskVoid AwakeAsync()
        {
            await UniTask.WaitWhile(() => UI_MobileJoystick.Instance == null);

            UI_MobileJoystick joystick = UI_MobileJoystick.Instance;
            joystick.OnInputDown += OnInpuDown;
            joystick.OnInput += OnInput;
            joystick.OnInputUp += OnInputUp;
        }

        private void OnInpuDown()
        {
            _movement.OnInputDown();
            _animator.OnInputDown();
        }

        private void OnInput(Vector2 input)
        {
            _movement.OnInput(input);
            _animator.OnInput(input);
        }

        private void OnInputUp()
        {
            _movement.OnInputUp();
            _animator.OnInputUp();
        }

        private void Update()
        {
            _movement.Tick();
            _animator.Tick();
            _camera.Tick();
        }
    }
}
