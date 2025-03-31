using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private UI_Manabar manabarPrefab;
        [SerializeField]
        private Transform parentT;

        [Space(10)]
        [SerializeField]
        private float fadeDuration;
        [SerializeField]
        private float damagedFillSpeed;

        private void Start()
        {
            Damageable[] damageables = FindObjectsOfType<Damageable>();
            foreach (Damageable damageable in damageables)
            {
                if (damageable.IsDead == false &&
                    damageable.gameObject.activeInHierarchy == true)
                {
                    HealthbarInfo info = new HealthbarInfo(_camera, damageable, fadeDuration, damagedFillSpeed);

                    UI_Healthbar uiInstance = Instantiate(healthbarPrefab, parentT);
                    uiInstance.Initialize(info);
                }
            }

            IProgressable[] progressables = FindObjectsOfType<MonoBehaviour>().OfType<IProgressable>().ToArray();
            foreach (IProgressable progressable in progressables)
            {
                if (progressable.gameObject.activeInHierarchy == true)
                {
                    ManabarInfo info = new ManabarInfo(_camera, progressable, fadeDuration, damagedFillSpeed);

                    UI_Manabar uiInstance = Instantiate(manabarPrefab, parentT);
                    uiInstance.Initialize(info);
                }
            }
        }

        public struct HealthbarInfo
        {
            public Camera _Camera;
            public Damageable _Damageable;
            public float _FadeDuration;
            public float _DamagedFillSpeed;

            public HealthbarInfo(Camera camera, Damageable damageable, float fadeDuration, float damagedFillSpeed)
            {
                this._Camera = camera;
                this._Damageable = damageable;
                this._FadeDuration = fadeDuration;
                this._DamagedFillSpeed = damagedFillSpeed;
            }
        }

        public struct ManabarInfo
        {
            public Camera _Camera;
            public IProgressable _Progressable;
            public float _FadeDuration;
            public float _DamagedFillSpeed;

            public ManabarInfo(Camera camera, IProgressable progressable, float fadeDuration, float damagedFillSpeed)
            {
                this._Camera = camera;
                this._Progressable = progressable;
                this._FadeDuration = fadeDuration;
                this._DamagedFillSpeed = damagedFillSpeed;
            }
        }
    }
}