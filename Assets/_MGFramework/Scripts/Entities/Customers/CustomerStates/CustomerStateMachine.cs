using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CustomerStateMachine
    {
        private ICustomerState currentState;

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

        public void ChangeState(ICustomerState newState)
        {
            if (_isShowLog == true)
            {
                Debug.Log("손님 상태 변경 : " + newState.GetType());
            }

            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void SlowTick()
        {
            currentState?.SlowTick();
        }

        public void FixedTick()
        {
            currentState?.FixedTick();
        }
    }
}