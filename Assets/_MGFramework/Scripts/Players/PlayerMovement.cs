using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerMovement
    {
        private PlayerData _data;

        [SerializeField]
        private CharacterController controller;

        [Space(10)]
        [SerializeField]
        private float moveSpeed = 1f;
        [SerializeField]
        private float moveLerpPower = 5f;
        [SerializeField]
        private float lookSpeed = 1f;

        private bool isInput = false;
        private Vector3 currentDesiredMovement;

        public void OnAwake(PlayerData data)
        {
            this._data = data;
            this._data.Behaviour.OnTargetStay += OnTargetStay;
        }

        private void OnTargetStay(Damageable damageable)
        {
            Vector3 targetDirection = (damageable.transform.position - _data._Transform.position).normalized;
            Quaternion desiredRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            _data._Transform.rotation = Quaternion.RotateTowards(_data._Transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
        }

        public void OnInputDown()
        {
            isInput = true;
        }

        public void OnInput(Vector2 input)
        {
            // 중력에 의한 처리
            float yAmount = -2f;
            if (controller.isGrounded == false || controller.velocity.y >= 0f)
            {
                yAmount += Physics.gravity.y * Time.deltaTime;
            }

            // 카메라 회전 각도에 대한 처리
            float cameraYAngle = Camera.main.transform.eulerAngles.y;
            Quaternion cameraPerformedRotation = Quaternion.Euler(0f, cameraYAngle, 0f);

            Vector3 direction = cameraPerformedRotation * new Vector3(input.x, 0f, input.y);
            Vector3 movement = (direction * moveSpeed * Time.deltaTime) + (Vector3.up * yAmount);
            currentDesiredMovement = Vector3.Lerp(currentDesiredMovement, movement, moveLerpPower * Time.deltaTime);

            // 회전에 대한 처리
            if (direction != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                _data._Transform.rotation = Quaternion.RotateTowards(_data._Transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
            }
        }

        public void OnInputUp()
        {
            isInput = false;
        }

        public void Tick()
        {
            if (isInput == false)
            {
                currentDesiredMovement = Vector3.Lerp(currentDesiredMovement, Vector3.zero, moveLerpPower * Time.deltaTime);
            }

            controller.Move(currentDesiredMovement);
        }
    }
}
