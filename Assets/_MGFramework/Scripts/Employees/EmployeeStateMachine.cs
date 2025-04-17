using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class EmployeeStateMachine 
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        private IEmployeeState currentState;
     
        private CancellationTokenSource source = null;

        public delegate void StateChangedDelegate(IEmployeeState oldState, IEmployeeState newState);
        public event StateChangedDelegate OnStateChangedEvent;

        public EmployeeStateMachine(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;
        }

        public void ChangeState(IEmployeeState newState)
        {
            IEmployeeState oldState = currentState;

            if (_data.IsShowLog == true)
            {
                string oldStateName = oldState != null ? oldState.GetType().Name : "";
                string newStateName = newState != null ? newState.GetType().Name : "";

                oldStateName = oldStateName.Replace("EmployeeState", "");
                newStateName = newStateName.Replace("EmployeeState", "");

                Debug.Log("EmployeeStateMachine <color=white><b>" + oldStateName + " => " + newStateName + "</b></color>");
            }

            currentState?.Exit();
            currentState = newState;
            currentState?.Enter(_context, _data);

            source?.Cancel();
            source = new CancellationTokenSource();
            if (newState != null)
            {
                SlowTickProvider(source.Token).Forget();
            }

            OnStateChangedEvent?.Invoke(oldState, newState);
        }

        private async UniTask SlowTickProvider(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                currentState?.SlowTick();

                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            }
        }

        public void LogCurrentState()
        {
            string currentStateName = currentState != null ? currentState.GetType().Name : "";
            currentStateName = currentStateName.Replace("EmployeeState", "");

            Debug.Log("EmployeeStateMachine <color=white><b>" + currentStateName + "</b></color>");
        }
    }
}
