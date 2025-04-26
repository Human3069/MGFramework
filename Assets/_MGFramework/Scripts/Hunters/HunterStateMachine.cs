using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class HunterStateMachine
    {
        private const string LOG_FORMAT = "<color=white><b>[HunterMachine]</b></color> {0}";
        
        private HunterContext _context;
        private HunterData _data;
        
        private IHunterState currentState;
        private CancellationTokenSource source = null;
        
        public delegate void StateChangedDelegate(IHunterState oldState, IHunterState newState);
        public event StateChangedDelegate OnStateChangedEvent;
        
        public HunterStateMachine(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;
        }

        public IHunterState GetState()
        {
            return currentState;
        }

        public void ChangeState(IHunterState newState)
        {
            IHunterState oldState = currentState;
            
            #if UNITY_EDITOR
            if (_data.IsShowLog == true)
            {
                string oldStateName = oldState == null ? "Null" : oldState.GetType().Name;
                string newStateName = newState == null ? "Null" : newState.GetType().Name;
                
                oldStateName = oldStateName.Replace("HunterState", "");
                newStateName = newStateName.Replace("HunterState", "");
                
                Debug.LogFormat(LOG_FORMAT, "<color=yellow>" + oldStateName + " => " + newStateName + "</color>");
            }
            #endif
            
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
        
        public void LogCurrentState()
        {
            if (currentState == null)
            {
                Debug.LogFormat(LOG_FORMAT, "No current state.");
            }
            else
            {
                string currentStateName = currentState.GetType().Name;
                currentStateName = currentStateName.Replace("HunterState", "");
                
                Debug.LogFormat(LOG_FORMAT, "current : " + currentStateName);
            }
        }
    }
}