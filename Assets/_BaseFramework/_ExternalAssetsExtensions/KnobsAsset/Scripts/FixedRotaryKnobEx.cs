﻿using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace _KMH_Framework
{
    [ExecuteInEditMode]
    public class FixedRotaryKnobEx : BaseKnobEx
    {
        protected enum InteractionTypes
        {
            VERTICAL_DRAG,
            ROTATION_DRAG
        }

        protected enum RotateType
        {
            Fixed,
            MoveToward,
            Lerp
        }

        [Header("=== FixedRotaryKnobEx ===")]
        [SerializeField]
        protected InteractionTypes InteractionType = InteractionTypes.VERTICAL_DRAG;
        [SerializeField]
        protected RotateType rotateType = RotateType.Fixed;

        [SerializeField]
        protected float rotatedAngle = 0f;
        [ReadOnly]
        [SerializeField]
        protected int _rotatedLapIndex = 0;
        public int RotatedLapIndex
        {
            get
            {
                return _rotatedLapIndex;
            }
            protected set
            {
                if (_rotatedLapIndex != value)
                {
                    _rotatedLapIndex = value;
                    OnValueChanged();
                }
            }
        }

        [ReadOnly]
        [SerializeField]
        protected int _targetAngleListIndex = 0;
        public int TargetAngleListIndex
        {
            get
            {
                return _targetAngleListIndex;
            }
            protected set
            {
                if (_targetAngleListIndex != value)
                {
                    _targetAngleListIndex = value;

                    if (rotateType == RotateType.Fixed || Application.isPlaying == false)
                    {
                        _handle.localEulerAngles = Vector3.up * fixedAngleList[value];
                    }
                    else
                    {
                        UniTaskEx.Cancel(this, 0);
                        PostOnTargetAngleListIndexValueChanged(Quaternion.Euler(Vector3.up * fixedAngleList[value])).Forget();
                    }
                }
            }
        }

        protected Coroutine _postOnTargetAngleListIndexValueChanged;
        

        protected async UniTaskVoid PostOnTargetAngleListIndexValueChanged(Quaternion angleRotation)
        {
            while (_handle.localRotation != angleRotation)
            {
                if (rotateType == RotateType.MoveToward)
                {
                    _handle.localRotation = Quaternion.RotateTowards(_handle.localRotation, angleRotation, rotateSpeed * Time.deltaTime);
                }
                else
                {
                    _handle.localRotation = Quaternion.Lerp(_handle.localRotation, angleRotation, rotateSpeed * Time.deltaTime);
                }

                await UniTaskEx.NextFrame(this, 0);
            }
        }

        [Space(10)]
        [SerializeField]
        protected float rotateSpeed = 1f;

        [Space(10)]
        [SerializeField]
        protected List<float> fixedAngleList = new List<float>();
        [ReadOnly]
        [SerializeField]
        protected List<float> intermediateAngleList = new List<float>();

        [Space(10)]
        [SerializeField]
        protected bool isInvokeValueChangedOnStart = true;
        [SerializeField]
        protected float mouseDragSensitivity = 50f;

        [Space(10)]
        [SerializeField]
        protected UnityEvent<int> _onKnobGrabbedEvent = default;
        [SerializeField]
        protected UnityEvent<int> _onValueChangedEvent = default;
        [SerializeField]
        protected UnityEvent<int> _onKnobReleasedEvent = default;

        protected Vector3 _prevMouseDirection;
        protected Vector3 _grabbedMouseOffset;

        protected override void Start()
        {
            if (Application.isPlaying == true)
            {
                fixedAngleList.Sort();
                for (int i = 0; i < fixedAngleList.Count - 1; i++)
                {
                    float intermediated = (fixedAngleList[i] + fixedAngleList[i + 1]) / 2f;
                    intermediateAngleList[i] = intermediated;
                }

                base.Start();

                if (isInvokeValueChangedOnStart == true)
                {
                    OnValueChanged();
                }
            }
        }

        protected void Update()
        {
            if (Application.isPlaying == false)
            {
                for (int i = 0; i < fixedAngleList.Count; i++)
                {
                    while (fixedAngleList[i] < 0f)
                    {
                        fixedAngleList[i] += 360f;
                    }

                    while (fixedAngleList[i] > 360f)
                    {
                        fixedAngleList[i] -= 360f;
                    }
                }

                if (fixedAngleList.Count > 0)
                {
                    while (intermediateAngleList.Count < fixedAngleList.Count - 1)
                    {
                        intermediateAngleList.Add(0f);
                    }
                    while (intermediateAngleList.Count > fixedAngleList.Count - 1)
                    {
                        intermediateAngleList.RemoveAt(intermediateAngleList.Count - 1);
                    }
                }

                for (int i = 0; i < fixedAngleList.Count - 1; i++)
                {
                    float intermediated = (fixedAngleList[i] + fixedAngleList[i + 1]) / 2f;
                    intermediateAngleList[i] = intermediated;
                }

                float absoluteAngle = rotatedAngle;
                while (absoluteAngle < 0)
                {
                    absoluteAngle += 360f;
                }
                while (absoluteAngle > 360)
                {
                    absoluteAngle -= 360f;
                }

                if (absoluteAngle < intermediateAngleList[0])
                {
                    TargetAngleListIndex = 0;
                }
                else if (absoluteAngle > intermediateAngleList[intermediateAngleList.Count - 1])
                {
                    TargetAngleListIndex = fixedAngleList.Count - 1;
                }
                else
                {
                    for (int i = 0; i < intermediateAngleList.Count - 1; i++)
                    {
                        if (absoluteAngle > intermediateAngleList[i] && absoluteAngle < intermediateAngleList[i + 1])
                        {
                            TargetAngleListIndex = (i + 1);
                        }
                    }
                }
            }
        }

        public override void OnGrabbed()
        {
            base.OnGrabbed();
            _grabbedMouseOffset = MousePositionOnRelativePlane() - this.transform.position;
            _onKnobGrabbedEvent.Invoke(TargetAngleListIndex);

            PostOnGrabbed().Forget();
        }

        protected async UniTaskVoid PostOnGrabbed()
        {
            while (_isGrabbed == true)
            {
                float angleToRotate;
                switch (InteractionType)
                {
                    case InteractionTypes.VERTICAL_DRAG:
                        angleToRotate = (Input.GetAxis("Mouse Y") / Screen.height) * 100f * mouseDragSensitivity;
                        break;

                    case InteractionTypes.ROTATION_DRAG:
                        Vector3 mousePosOnPlane = MousePositionOnRelativePlane() - _grabbedMouseOffset;
                        angleToRotate = Vector3.SignedAngle(_prevMouseDirection - transform.position, mousePosOnPlane - transform.position, transform.up);
                        _prevMouseDirection = mousePosOnPlane;
                        break;

                    default:
                        Debug.LogException(new System.InvalidOperationException("Invalid InteractionTypes value " + InteractionType), this);
                        return;
                }

                rotatedAngle += angleToRotate;
                RotatedLapIndex = (int)(rotatedAngle / 360f);

                float absoluteAngle = rotatedAngle;
                while (absoluteAngle < 0)
                {
                    absoluteAngle += 360f;
                }
                while (absoluteAngle > 360)
                {
                    absoluteAngle -= 360f;
                }

                if (absoluteAngle < intermediateAngleList[0])
                {
                    TargetAngleListIndex = 0;
                }
                else if (absoluteAngle > intermediateAngleList[intermediateAngleList.Count - 1])
                {
                    TargetAngleListIndex = fixedAngleList.Count - 1;
                }
                else
                {
                    for (int i = 0; i < intermediateAngleList.Count - 1; i++)
                    {
                        if (absoluteAngle > intermediateAngleList[i] && absoluteAngle < intermediateAngleList[i + 1])
                        {
                            TargetAngleListIndex = (i + 1);
                        }
                    }
                }

                await UniTask.NextFrame();
            }
        }

        protected override void OnValueChanged()
        {
            if (Application.isPlaying == true)
            {
                _onValueChangedEvent.Invoke(TargetAngleListIndex);
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();

            _onKnobReleasedEvent.Invoke(TargetAngleListIndex);
        }

        protected override void SetKnobPosition(float normalized)
        {
            // Fixed Knob은 normalized를 지원하지 않습니다.
        }

        protected virtual void OnDrawGizmos()
        {
            Handles.color = Color.yellow;
            for (int i = 0; i < fixedAngleList.Count; i++)
            {
                Vector3 angle = Quaternion.AngleAxis(fixedAngleList[i], this.transform.up) * this.transform.forward;
                Handles.DrawLine(this.transform.position, this.transform.position + (angle * 1f));
            }

            Handles.color = Color.red;

            Vector3 currentAngle = Quaternion.AngleAxis(fixedAngleList[TargetAngleListIndex], this.transform.up) * this.transform.forward;
            Handles.DrawLine(this.transform.position, this.transform.position + (currentAngle * 1.25f));
        }
    }
}
