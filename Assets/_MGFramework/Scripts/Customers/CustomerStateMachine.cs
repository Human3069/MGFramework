using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class CustomerStateMachine
    {
        private CustomerContext _context;
        private CustomerData _data;

        private ICustomerState currentState;

        private CancellationTokenSource source = null;

        public delegate void StateChangedDelegate(ICustomerState oldState, ICustomerState newState);
        public event StateChangedDelegate OnStateChangedEvent;

        public CustomerStateMachine(CustomerContext context, CustomerData data)
        {
            this._context = context;
            this._data = data;
        }

        public ICustomerState GetState()
        {
            return currentState;
        }

        public void ChangeState(ICustomerState newState)
        {
            ICustomerState oldState = currentState;
            if (_data.IsShowLog == true)
            {
                string oldStateName = oldState != null ? oldState.GetType().Name : "";
                string newStateName = newState != null ? newState.GetType().Name : "";

                oldStateName = oldStateName.Replace("CustomerState", "");
                newStateName = newStateName.Replace("CustomerState", "");

                Debug.Log("CustomerStateMachine " + oldStateName + " => " + newStateName);
            }

            currentState?.Exit();
            currentState = newState;
            currentState?.Enter(_context, _data);

            source?.Cancel();
            source = new CancellationTokenSource();
            if (newState != null)
            {
                FixedTickProvider(source.Token).Forget();
                SlowTickProvider(source.Token).Forget();
            }

            OnStateChangedEvent?.Invoke(oldState, newState);
        }

        private async UniTask FixedTickProvider(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                currentState?.FixedTick();

                await UniTask.WaitForFixedUpdate(cancellationToken: token);
            }
        }

        private async UniTask SlowTickProvider(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                currentState?.SlowTick();

                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            }
        }
    }
}
