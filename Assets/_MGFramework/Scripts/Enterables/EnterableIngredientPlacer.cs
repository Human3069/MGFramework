using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public class EnterableIngredientPlacer : BaseEnterable
    {
        protected PlayerInventory currentInventory;

        [SerializeField]
        protected ItemType itemType;
        [SerializeField]
        protected float placePerSecond = 0.1f;

        [Space(10)]
        [SerializeField]
        protected float distancePerRow = 0.4f;
        [SerializeField]
        protected float distancePerColumn = 0.3f;
        [SerializeField]
        protected int stackPerColumn; // 0 => unlimited
        [SerializeField]
        protected int limitCount = 32;

        [Space(10)]
        [SerializeField]
        protected Transform stackParent;

        protected Stack<GameObject> objStack = new Stack<GameObject>();

        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<PlayerInventory>(out PlayerInventory inventory) == true)
            {
                currentInventory = inventory;
                PostOnTriggerEnter().Forget();
            }
        }

        protected virtual async UniTaskVoid PostOnTriggerEnter()
        {
            while (objStack.Count < limitCount &&
                   currentInventory != null &&
                   currentInventory.TryPop(itemType, out GameObject obj) == true)
            {
                PushStackObj(obj).Forget();

                await UniTask.WaitForSeconds(placePerSecond);
            }
        }

        protected virtual async UniTaskVoid PushStackObj(GameObject obj)
        {
            obj.name += "_Popping";
            objStack.Push(obj);

            Animation itemAnimation = obj.GetComponent<Animation>();
            AnimationState state = itemAnimation.PlayQueued("OnDisable");

            await UniTask.WaitForSeconds(state.length);

            int parentCount = stackParent.childCount;
            obj.transform.parent = stackParent;

            float rowResult;
            float columnResult;
            if (stackPerColumn == 0)
            {
                rowResult = parentCount * distancePerRow;
                columnResult = 0f;
            }
            else
            {
                rowResult = (parentCount / stackPerColumn) * distancePerRow;
                columnResult = (parentCount % stackPerColumn) * distancePerColumn;
            }

            obj.transform.localPosition = new Vector3(0, columnResult, rowResult);
            obj.transform.eulerAngles = Vector3.zero;

            itemAnimation.PlayQueued("OnEnable");
            obj.name = obj.name.Replace("_Popping", "");
        }

        protected override void OnTriggerExit(Collider collider)
        {
            currentInventory = null;
        }
    }
}