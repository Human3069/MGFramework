using Cysharp.Threading.Tasks;
using System.Threading;

namespace MGFramework
{
    public class EmployeeStateMachine 
    {
        private Employee _employee;
        private IEmployeeState currentState;
     
        private CancellationTokenSource source = null;

        public EmployeeStateMachine(Employee employee)
        {
            this._employee = employee;
        }

        public void ChangeState(IEmployeeState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter(_employee);

            source?.Cancel();
            source = new CancellationTokenSource();
            if (newState != null)
            {
                SlowTickProvider(source.Token).Forget();
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

        public void Tick()
        {
            currentState?.Tick();
        }

        public void FixedTick()
        {
            currentState?.FixedTick();
        }
    }
}
