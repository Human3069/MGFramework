using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class IdleMonsterState : IMonsterState
    {
        private MonsterContext _context;
        private MonsterData _data;

        private Collider[] overlapColliders = new Collider[10];

        public void Enter(MonsterContext context, MonsterData data)
        {
            this._context = context;
            this._data = data;

            _context.AnimationController.PlaySleepOrSit();
        }

        public void Exit()
        {
            // 상태 종료 처리
        }

        public void FixedTick()
        {

        }

        public void SlowTick()
        {
            if (_context.TargetDamageable == null)
            {
                int layerMask = ~(1 << 3);
                int overlapCount = Physics.OverlapSphereNonAlloc(_context.Transform.position, _data.MaxAlertRange, overlapColliders, layerMask);

                for (int i = 0; i < overlapCount; i++)
                {
                    Collider overlapCollider = overlapColliders[i];

                    if (overlapCollider.TryGetComponent(out Damageable damageable) == true &&
                        damageable.transform != _context.Transform &&
                        damageable.GetOwnerType() == OwnerType.Players &&
                        _context.IsInAlertRange(_data, damageable.transform.position) == true)
                    {
                        _context.TargetDamageable = damageable;
                        _context.StateMachine.ChangeState(new AlertMonsterState());
                    }
                }
            }
            else
            {
                if (_context.IsInAlertRange(_data) == false)
                {
                    _context.TargetDamageable = null;
                }
                else
                {
                    _context.StateMachine.ChangeState(new AlertMonsterState());
                }
            }
        }
    }
}