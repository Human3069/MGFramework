using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class UI_Main : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private UI_Healthbar healthbarPrefab;
        [SerializeField]
        private Transform parentT;

        [Space(10)]
        [SerializeField]
        private float fadeDuration;
        [SerializeField]
        private float damagedFillSpeed;

        private void Start()
        {
            BaseDamageable[] damageables = FindObjectsByType<BaseDamageable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (BaseDamageable damageable in damageables)
            {
                if (damageable.IsDead == false &&
                    damageable.gameObject.activeInHierarchy == true)
                {
                    HealthbarInfo info = new HealthbarInfo(_camera, damageable, fadeDuration, damagedFillSpeed);

                    UI_Healthbar uiInstance = Instantiate(healthbarPrefab, parentT);
                    uiInstance.Initialize(info);
                }
            }
        }

        public struct HealthbarInfo
        {
            public Camera _Camera;
            public BaseDamageable _Damageable;
            public float _FadeDuration;
            public float _DamagedFillSpeed;

            public HealthbarInfo(Camera camera, BaseDamageable damageable, float fadeDuration, float damagedFillSpeed)
            {
                this._Camera = camera;
                this._Damageable = damageable;
                this._FadeDuration = fadeDuration;
                this._DamagedFillSpeed = damagedFillSpeed;
            }
        }
    }
}