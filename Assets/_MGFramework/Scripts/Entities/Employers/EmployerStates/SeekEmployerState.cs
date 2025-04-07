using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class SeekEmployerState : IEmployerState
    {
        private EmployerData _data;

        public SeekEmployerState(EmployerData data)
        {
            this._data = data;
        }

        public void Enter()
        {
            this._data._Employer.TargetHarvestable = null;
        }

        public void Exit()
        {
            this._data._Agent.destination = _data._Transform.position;
        }

        public void SlowTick()
        {
            // ����� Harvestable�� ã�´�.
            if (_data._Employer.TargetHarvestable == null)
            {
                Harvestable[] foundHarvestables = Object.FindObjectsByType<Harvestable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                Harvestable nearestHarvestable = null;
                float nearestDistance = Mathf.Infinity;

                foreach (Harvestable foundHarvestable in foundHarvestables)
                {
                    if (foundHarvestable.IsHarvestable == true)
                    {
                        float distance = Vector3.Distance(_data._Transform.position, foundHarvestable.transform.position);
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestHarvestable = foundHarvestable;
                        }
                    }
                }

                _data._Employer.TargetHarvestable = nearestHarvestable;
            }
            else
            {
                // Ÿ�� ������ �̵�
                Vector3 targetPos = _data._Transform.GetForwardPositionTowardsHarvestable(_data._Employer.TargetHarvestable, 1f);
                _data._Agent.destination = targetPos;

                // �����ϸ� ���� ��ȯ
                if (_data.IsWorkablePos(_data._Employer.TargetHarvestable) == true)
                {
                    _data._Employer._EmployerState = EmployerState.Chop;
                }
            }
        }

        public void FixedTick()
        {

        }
    }
}