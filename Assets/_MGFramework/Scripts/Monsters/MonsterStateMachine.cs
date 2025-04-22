using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class MonsterStateMachine
    {
        private const string LOG_FORMAT = "<color=white><b>[MonsterMachine]</b></color> {0}";
        
        private MonsterContext _context;
        private MonsterData _data;
        
        private IMonsterState currentState;
        private CancellationTokenSource source = null;
        
        public delegate void StateChangedDelegate(IMonsterState oldState, IMonsterState newState);
        public event StateChangedDelegate OnStateChangedEvent;
        
        public MonsterStateMachine(MonsterContext context, MonsterData data)
        {
            this._context = context;
            this._data = data;
        }
        
        public IMonsterState GetState()
        {
            return currentState;
        }

        public void ChangeState(IMonsterState newState)
        {
            IMonsterState oldState = currentState;
            
            #if UNITY_EDITOR
            if (_data.IsShowLog == true)
            {
                string oldStateName = oldState == null ? "Null" : oldState.GetType().Name;
                string newStateName = newState == null ? "Null" : newState.GetType().Name;
                
                oldStateName = oldStateName.Replace("MonsterState", "");
                newStateName = newStateName.Replace("MonsterState", "");
                
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
                SlowTickProvider(source.Token).Forget();
                FixedTickProvider(source.Token).Forget();
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

        private async UniTask FixedTickProvider(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                currentState?.FixedTick();
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
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
                currentStateName = currentStateName.Replace("MonsterState", "");
                
                Debug.LogFormat(LOG_FORMAT, "current : " + currentStateName);
            }
        }
    }
}