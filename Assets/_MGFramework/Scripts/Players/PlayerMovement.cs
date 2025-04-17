using UnityEngine;

namespace MGFramework
{
    public class PlayerMovement
    {
        private PlayerContext _context;
        private PlayerData _data;

        private bool isInput = false;
        private Vector3 currentDesiredMovement;

        public PlayerMovement(PlayerContext context, PlayerData data)
        {
            this._context = context;
            this._data = data;

            this._context.Behaviour.OnTargetStay += OnTargetStay;
        }

        private void OnTargetStay(Damageable damageable)
        {
            Vector3 targetDirection = (damageable.transform.position - _data.Transform.position).normalized;
            Quaternion desiredRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            _data.Transform.rotation = Quaternion.RotateTowards(_data.Transform.rotation, desiredRotation, _data.LookSpeed * Time.deltaTime);
        }

        public void OnInputDown()
        {
            isInput = true;
        }

        public void OnInput(Vector2 input)
        {
            // 중력에 의한 처리
            float yAmount = -2f;
            if (_context.Controller.isGrounded == false || _context.Controller.velocity.y >= 0f)
            {
                yAmount += Physics.gravity.y * Time.deltaTime;
            }

            // 카메라 회전 각도에 대한 처리
            float cameraYAngle = Camera.main.transform.eulerAngles.y;
            Quaternion cameraPerformedRotation = Quaternion.Euler(0f, cameraYAngle, 0f);

            Vector3 direction = cameraPerformedRotation * new Vector3(input.x, 0f, input.y);
            Vector3 movement = (direction * _data.MoveSpeed * Time.deltaTime) + (Vector3.up * yAmount);
            currentDesiredMovement = Vector3.Lerp(currentDesiredMovement, movement, _data.MoveLerpPower * Time.deltaTime);

            // 회전에 대한 처리
            if (direction != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                _data.Transform.rotation = Quaternion.RotateTowards(_data.Transform.rotation, desiredRotation, _data.LookSpeed * Time.deltaTime);
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
                currentDesiredMovement = Vector3.Lerp(currentDesiredMovement, Vector3.zero, _data.MoveLerpPower * Time.deltaTime);
            }

            _context.Controller.Move(currentDesiredMovement);
        }
    }
}
