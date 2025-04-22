using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    public static class StateMachineTempletes 
    {
        public static string Owner(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"using UnityEngine;

{nsStart}public class {entityName} : MonoBehaviour
{{
    [SerializeField]
    private {entityName}Data data;

    private {entityName}Context context;
    private {entityName}StateMachine stateMachine;

    private void Awake()
    {{
        context = new {entityName}Context(this);
        stateMachine = new {entityName}StateMachine(context, data);
        context.Initialize(stateMachine);
    }}
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string Interface(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"{nsStart}public interface I{entityName}State
{{
    void Enter({entityName}Context context, {entityName}Data data);
    void Exit();
    void SlowTick(); // 0.5초마다 호출
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string State(int index, string stateName, string entityName, string namespaceName, bool isFormatted = true)
        {
            string className = $"{stateName}{entityName}State";
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"{nsStart}public class {className} : I{entityName}State
{{
    private {entityName}Context _context;
    private {entityName}Data _data;

    public void Enter({entityName}Context context, {entityName}Data data)
    {{
        this._context = context;
        this._data = data;
    }}

    public void Exit()
    {{
        // 상태 종료 처리
    }}

    public void SlowTick()
    {{
        // 0.5초 간격 상태 유지 루프
    }}
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string StateMachine(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

{nsStart}public class {entityName}StateMachine
{{
    private const string LOG_FORMAT = ""<color=white><b>[{entityName}Machine]</b></color> {{0}}"";

    private {entityName}Context _context;
    private {entityName}Data _data;

    private I{entityName}State currentState;
    private CancellationTokenSource source = null;

    public delegate void StateChangedDelegate(I{entityName}State oldState, I{entityName}State newState);
    public event StateChangedDelegate OnStateChangedEvent;

    public {entityName}StateMachine({entityName}Context context, {entityName}Data data)
    {{
        this._context = context;
        this._data = data;
    }}

    public void ChangeState(I{entityName}State newState)
    {{
        I{entityName}State oldState = currentState;

#if UNITY_EDITOR
        if (_data.IsShowLog == true)
        {{
            string oldStateName = oldState == null ? ""Null"" : oldState.GetType().Name;
            string newStateName = newState == null ? ""Null"" : newState.GetType().Name;

            oldStateName = oldStateName.Replace(""{entityName}State"", """");
            newStateName = newStateName.Replace(""{entityName}State"", """");

            Debug.LogFormat(LOG_FORMAT, ""<color=yellow>"" + oldStateName + "" => "" + newStateName + ""</color>"");
        }}
#endif

        currentState?.Exit();
        currentState = newState;
        currentState?.Enter(_context, _data);
        
        source?.Cancel();
        source = new CancellationTokenSource();
        if (newState != null)
        {{
                SlowTickProvider(source.Token).Forget();
        }}
        
        OnStateChangedEvent?.Invoke(oldState, newState);
    }}
    
    private async UniTask SlowTickProvider(CancellationToken token)
    {{
        while (token.IsCancellationRequested == false)
        {{
            currentState?.SlowTick();
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
        }}
    }}

    public void LogCurrentState()
    {{
        if (currentState == null)
        {{
            Debug.LogFormat(LOG_FORMAT, ""No current state."");
        }}
        else
        {{
            string currentStateName = currentState.GetType().Name;
            currentStateName = currentStateName.Replace(""{entityName}State"", """");

            Debug.LogFormat(LOG_FORMAT, ""current : "" + currentStateName);
        }}
    }}
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string Context(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"using UnityEngine;
using UnityEngine.AI;

{nsStart}public class {entityName}Context
{{
    public {entityName}StateMachine StateMachine;

    public Transform Transform;
    public NavMeshAgent Agent;
    public {entityName}AnimationController AnimationController;

    public {entityName}Context({entityName} owner)
    {{
        this.Transform = owner.transform;
        this.Agent = owner.GetComponent<NavMeshAgent>();

        Animator animator = owner.GetComponentInChildren<Animator>();
        this.AnimationController = new {entityName}AnimationController(animator);
    }}

    public void Initialize({entityName}StateMachine stateMachine)
    {{
        this.StateMachine = stateMachine;
    }}
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string Data(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"using System;
using UnityEngine;

{nsStart}[Serializable]
public class {entityName}Data
{{
#if UNITY_EDITOR
    public bool IsShowLog = true;
#endif
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string AnimationController(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"using UnityEngine;

{nsStart}public class {entityName}AnimationController
{{
    private Animator _animator;

    public {entityName}AnimationController(Animator animator)
    {{
        this._animator = animator;
    }}

    public void PlayMove(bool isMoving)
    {{
        _animator.SetBool(""IsMoving"", isMoving);
        _animator.SetTrigger(""IsMovingStateChanged"");
    }}
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        public static string Utility(string entityName, string namespaceName, bool isFormatted = true)
        {
            string nsStart = string.IsNullOrWhiteSpace(namespaceName) ? "" : $"namespace {namespaceName}\n{{\n";
            string nsEnd = string.IsNullOrWhiteSpace(namespaceName) ? "" : "\n}";

            string result = $@"using UnityEngine;

{nsStart}public static class {entityName}Utility
{{
}}{nsEnd}";

            if (isFormatted == true)
            {
                result = result.AsFormatted();
            }
            return result;
        }

        private static string AsFormatted(this string rawString)
        {
            string[] lines = rawString.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            List<string> resultList = new List<string>();
            int indentLevel = 0;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed == "}")
                {
                    indentLevel--;
                }

                string indent = new string(' ', indentLevel * 4);
                resultList.Add(indent + trimmed);

                if (trimmed.EndsWith("{") == true)
                {
                    indentLevel++;
                }
            }

            return string.Join("\n", resultList);
        }
    }
}
