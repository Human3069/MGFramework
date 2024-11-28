using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _MG_Framework
{
    public enum DamageableType
    {
        Player,
        Tree,
        Bear
    }

    public class UI_Main : MonoBehaviour
    {
        [SerializeField]
        protected Camera targetCamera;

        [Space(10)]
        [SerializeField]
        protected RectTransform healthbarPanel;
        [SerializeField]
        protected UI_Healthbar healthbarPrefab;

        [Space(10)]
        [SerializeField]
        [SerializedDictionary("Component", "Healthbar Height")]
        protected SerializedDictionary<DamageableType, float> healthbarHeightDic = new SerializedDictionary<DamageableType, float>();

        protected virtual void Start()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            BaseEnterable[] foundEnterables = Resources.FindObjectsOfTypeAll<BaseEnterable>();

            int index = 0;
            foreach (BaseEnterable enterable in foundEnterables)
            {
                if (enterable is IDamageable &&
                    enterable.gameObject.scene == currentScene)
                {
                    IDamageable damageable = enterable as IDamageable;
                    DamageableType type = damageable._Type;

                    float additionalHeight = healthbarHeightDic[type];

                    UI_Healthbar healthbarInstance = Instantiate(healthbarPrefab);
                    healthbarInstance.transform.SetParent(healthbarPanel);
                    healthbarInstance.Initialize(targetCamera, damageable, additionalHeight);
                    healthbarInstance.gameObject.name = "UI_Healthbar_" + index;

                    index++;
                }
            }
        }
    }
}