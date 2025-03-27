using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerInputController
    {
        private NavMeshAgent _agent;

        [SerializeField]
        private Camera mainCamera;

        public void OnAwake(NavMeshAgent agent)
        {
            this._agent = agent;
        }

        public void OnUpdate()
        {
            MoveToInputDestination();
            SetCameraPosition();
        }

        private void MoveToInputDestination()
        {
            if (Input.GetMouseButtonDown(1) == true)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue) == true)
                {
                    _agent.destination = hit.point;

                    FxType.PlayerMarker.EnablePool(OnBeforePoolEnabled);
                    void OnBeforePoolEnabled(GameObject obj)
                    {
                        obj.transform.position = hit.point;
                        OnPoolEnabledAsync(obj).Forget();

                        async UniTaskVoid OnPoolEnabledAsync(GameObject obj)
                        {
                            await UniTask.WaitForSeconds(0.3f);
                            obj.ReturnPool(FxType.PlayerMarker);
                        }
                    }
                }
            }
        }

        private void SetCameraPosition()
        {
            mainCamera.transform.position = _agent.transform.position + mainCamera.transform.forward * -100f;
        }
    }
}