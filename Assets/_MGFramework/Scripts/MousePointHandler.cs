using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public class MousePointHandler : MonoPoolable
    {
        [Header("=== MousePointHandler ===")]
        [SerializeField]
        protected PlayerController player;
        [SerializeField]
        protected Camera targetCamera;

        [Space(10)]
        [SerializeField]
        protected Transform continuousPoint;

        public delegate void MouseDown(Vector3 worldPos);
        public event MouseDown OnMouseDown;

        protected override void Awake()
        {
            base.Awake();

            player.OnStopped += OnStopped;
        }

        protected virtual void OnDestroy()
        {
            player.OnStopped -= OnStopped;
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(1) == true)
            {
                MoveToMousePoint();
            }
        }

        protected virtual void MoveToMousePoint()
        {
            Vector2 mousePos = Input.mousePosition;
            Ray ray = targetCamera.ScreenPointToRay(mousePos);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.isStatic == true)
                {
                    Vector3 hitPoint = hit.point;

                    if (OnMouseDown != null)
                    {
                        OnMouseDown(hitPoint);
                    }

                    EnableObject(hitPoint);
                    continuousPoint.position = hitPoint;

                    break;
                }
            }
        }

        protected virtual void OnStopped(bool isStopped)
        {
            continuousPoint.gameObject.SetActive(isStopped == false);
        }
    }
}