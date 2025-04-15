using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private PlayerData data;

        [Space(10)]
        [SerializeField]
        private PlayerMovement _movement;
        [SerializeField]
        private PlayerAnimator _animator;
        [SerializeField]
        private PlayerCamera _camera;

        private void Awake()
        {
            _movement.OnAwake(data);
            _animator.OnAwake(data);
            _camera.OnAwake(data);

            AwakeAsync().Forget();
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
