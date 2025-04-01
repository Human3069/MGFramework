using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class MonsterStateMachine 
    {
        private IMonsterState currentState;

#if UNITY_EDITOR
        private bool _isShowLog = false;

        public void OnIsShowLogValueChange(bool isShowLog)
        {
            if (currentState != null)
            {
                this._isShowLog = isShowLog;
            }
        }
#endif

        public void ChangeState(IMonsterState newState)
        {
            if (_isShowLog == true)
            {
                Debug.Log("°õ »óÅÂ º¯°æ : " + newState.GetType());
            }

            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void SlowTick()
        {
            currentState?.SlowTick();
        }

        public void Tick()
        {
            currentState?.Tick();
        }
    }
}