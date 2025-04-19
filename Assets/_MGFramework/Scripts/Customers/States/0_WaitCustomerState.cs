using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class WaitCustomerState : ICustomerState
    {
        private CustomerContext _context;
        private CustomerData _data;

        // randomized values
        private Dictionary<PoolType, int> requirementDic;

        private CustomerWaitingLine waitingLine;
        private bool isFirstCustomer;

        public void Enter(CustomerContext context, CustomerData data)
        {
            this._context = context;
            this._data = data;

            requirementDic = new Dictionary<PoolType, int>();
            foreach (KeyValuePair<PoolType, Vector2Int> pair in _data.RequirementDic)
            {
                int randomCount = Random.Range(pair.Value.x, pair.Value.y + 1);
                requirementDic.Add(pair.Key, randomCount);
            }

            waitingLine = GameManager.Instance.WaitingLine;
        }

        public void Exit()
        {
            
        }

        public void FixedTick()
        {
            bool isArrived = _context.Agent.IsArrived();
            Vector3 desiredDirection = _context.DesiredDirection;

            if (isArrived == true &&
                desiredDirection != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
                _context.Transform.rotation = Quaternion.RotateTowards(_context.Transform.rotation, desiredRotation, _data.LookAtSpeed);
            }
        }

        public void SlowTick()
        {
            if (waitingLine.IsFirstCustomer(_context.Customer) == true &&
                _context.Agent.IsArrived() == true &&
                _context.IsCustomerInitialized == true &&
                isFirstCustomer == false)
            {
                isFirstCustomer = true;
                OnBecomeFirstCustomerAsync().Forget();
            }
        }

        private async UniTaskVoid OnBecomeFirstCustomerAsync()
        {
            int desiredFoodCount = 0;
            foreach (KeyValuePair<PoolType, int> pair in requirementDic)
            {
                desiredFoodCount += pair.Value;
            }

            _data.DesiredText.text = desiredFoodCount.ToString();
            _data.DesiredCanvas.gameObject.SetActive(true);

            await UniTask.WaitForSeconds(_data.FoodConsumeSpeed);

            while (desiredFoodCount != 0)
            {
                foreach (KeyValuePair<PoolType, int> pair in requirementDic)
                {
                    Payload counterPayload = waitingLine.CounterPayload;
                    if (counterPayload.TryPopInputStore(pair.Key) == true)
                    {
                        GameManager.Instance.Gold += _data.GoldPerFood;
                        desiredFoodCount--;

                        _data.DesiredText.text = desiredFoodCount.ToString();
                    }
                }

                await UniTask.WaitForSeconds(_data.FoodConsumeSpeed);
            }

            _data.DesiredCanvas.gameObject.SetActive(false);
            _context.StateMachine.ChangeState(new FindSeatCustomerState());
        }
    }
}
