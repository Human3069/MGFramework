using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoSingleton<Player>
    {
        [SerializeField]
        private PlayerData data;
        private PlayerContext context;

        public void Upgrade(float value, UpgradeType type)
        {
            if (type == UpgradeType.Health)
            {
                context.Damageable.UpdateMaxHealth(value);
            }
            else if (type == UpgradeType.MoveSpeed)
            {
                data.MoveSpeed = value;
            }
            else if (type == UpgradeType.AttackSpeed)
            {
                data.AttackSpeed = value;
            }
            else if (type == UpgradeType.AttackDamage)
            {
                data.AttackDamage = value;
            }
            else
            {
                throw new System.NotImplementedException("type : " + type);
            }
        }

        private void Awake()
        {
            context = new PlayerContext(data);

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
            context.Movement.OnInputDown();
            context.Animator.OnInputDown();
        }

        private void OnInput(Vector2 input)
        {
            context.Movement.OnInput(input);
            context.Animator.OnInput(input);
        }

        private void OnInputUp()
        {
            context.Movement.OnInputUp();
            context.Animator.OnInputUp();
        }

        private void Update()
        {
            context.Movement.Tick();
            context.Animator.Tick();
            context.Camera.Tick();
        }
    }
}
