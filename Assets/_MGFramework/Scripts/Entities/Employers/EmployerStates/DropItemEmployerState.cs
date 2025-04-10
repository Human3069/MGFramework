using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class DropItemEmployerState : IEmployerState
    {
        private EmployerData _data;
        private BasePayloader targetPayloader = null;

        public DropItemEmployerState(EmployerData data)
        {
            this._data = data;
            this.targetPayloader = null;
        }

        public void Enter()
        {
#if false
            // ���� ����� Payloader�� ã�´�.
            BasePayloader[] payloaders = Object.FindObjectsOfType<BasePayloader>(false);
            BasePayloader nearestPayloader = null;
            float nearestDistance = Mathf.Infinity;

            foreach (BasePayloader payloader in payloaders)
            {
                float distance = Vector3.Distance(_data._Transform.position, payloader.transform.position);
                if (distance < nearestDistance)
                {
                    nearestPayloader = payloader;
                    nearestDistance = distance;
                }
            }
#endif

            // ���� �� Input�� ���� Payloader�� ã�´�.
            BasePayloader[] payloaders = Object.FindObjectsOfType<BasePayloader>(false);
            BasePayloader poorestPayloader = null;
            int poorestInputCount = int.MaxValue;

            foreach (BasePayloader payloader in payloaders)
            {
                if (payloader.TotalInputCount < poorestInputCount)
                {
                    poorestPayloader = payloader;
                    poorestInputCount = payloader.TotalInputCount;
                }
            }

            targetPayloader = poorestPayloader;

            // ��ġ ����
            Collider payloaderCollider = targetPayloader.GetComponent<Collider>();
            Vector3 payloaderDest = payloaderCollider.ClosestPoint(_data._Transform.position);
            _data._Agent.destination = payloaderDest;
        }

        public void Exit()
        {

        }

        public void SlowTick()
        {
            float stopDistance = _data._Agent.stoppingDistance;
            float distance = Vector3.Distance(_data._Transform.position, _data._Agent.destination);

            // payloader ���� ������, �κ��丮�� ����ٸ� �ٽ� ���� ����
            if (distance < stopDistance &&
                _data._Inventory.IsInventoryNull == true)

            {
                _data._Employer._EmployerState = EmployerState.Seek;
            }
        }

        public void FixedTick()
        {

        }
    }
}