using _KMH_Framework;
using _KMH_Framework.Pool;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// ~ 키를 눌러서 CheatConsole를 열어 cheatCommand를 입력하여 Cheat를 사용할 수 있습니다. 혹은 binding된 키를 눌러서 사용할 수 있습니다.
    /// </summary>
    public class CheatController : MonoSingleton<CheatController>
    {
        [SerializeField, SerializedDictionary("KeyCode", "Command")]
        private SerializedDictionary<KeyCode, string> bindedCommandDic = new SerializedDictionary<KeyCode, string>();

        private Inventory playerInventory;
        private Inventory employeeInventory;

        private bool _isOn = false;
        public bool IsOn
        {
            get
            {
                return _isOn;
            }
            set
            {
                if (_isOn != value)
                {
                    _isOn = value;
                    OnCheatConsoleStateChanged?.Invoke(value);
                }
            }
        }

        public delegate void CheatConsoleDelegate(bool isOn);
        public event CheatConsoleDelegate OnCheatConsoleStateChanged;

        public delegate void SubmitCheatCommandDelegate(string command);
        public event SubmitCheatCommandDelegate OnSubmitCheatCommand;

        private void Awake()
        {
            playerInventory = Player.Instance.GetComponent<Inventory>();
            employeeInventory = FindObjectOfType<Employee>().GetComponent<Inventory>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) == true)
            {
                IsOn = !IsOn;
            }

            else if (Input.GetKeyDown(KeyCode.Insert))
            {
                Time.timeScale += 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Time.timeScale = 1f;
            }

            foreach (KeyValuePair<KeyCode, string> pair in bindedCommandDic)
            {
                if (Input.GetKeyDown(pair.Key) == true)
                {
                    if (TrySubmit(pair.Value) == true)
                    {
                        break;
                    }
                    else
                    {
                        Debug.LogError("Cheat command not found : " + pair.Value);
                    }
                }
            }
        }

        public bool TrySubmit(string inputText)
        {
            Regex singlePatternRegex = new Regex(@"^give_([a-zA-Z0-9]+)_([0-9]+)$");
            Regex multiplePatternRegex = new Regex(@"^give_([a-zA-Z0-9]+)_([0-9]+)_([0-9]+)$");

            Match singlesMatch = singlePatternRegex.Match(inputText);
            Match multiplesMatch = multiplePatternRegex.Match(inputText);

            if (multiplesMatch.Success == true)
            {
                string gaveTarget = multiplesMatch.Groups[1].Value;
                int gaveType = int.Parse(multiplesMatch.Groups[2].Value);
                uint gaveCount = uint.Parse(multiplesMatch.Groups[3].Value);

                return TryIntermediator(gaveTarget, gaveType, gaveCount);
            }
            else if (singlesMatch.Success == true)
            {
                string gaveTarget = singlesMatch.Groups[1].Value;
                int gaveType = int.Parse(singlesMatch.Groups[2].Value);

                return TryIntermediator(gaveTarget, gaveType, 1);
            }
            else if (inputText.Equals("enqueue_customer") == true)
            {
                EnqueueCustomer();
                return true;
            }
            else if (inputText.Equals("dequeue_customer") == true)
            {
                DequeueCustomer();
                return true;
            }
            else
            {
                Debug.LogError("잘못된 Command입니다 : " + inputText);
                return false;
            }
        }

        /// <summary>
        /// Regex로 부터 넘겨받은 값을 처리합니다.
        /// </summary>
        /// <param name="gaveTarget">player 혹은 employee 판별</param>
        /// <param name="gaveType">아이템 타입</param>
        /// <param name="gaveCount">아이템 갯수</param>
        private bool TryIntermediator(string gaveTarget, int gaveType, uint gaveCount)
        {
            Inventory gaveInventory = null;
            if (gaveTarget.Equals("player") == true)
            {
                gaveInventory = playerInventory;
            }
            else if (gaveTarget.Equals("employee") == true)
            {
                gaveInventory = employeeInventory;
            }
            else
            {
                Debug.LogError("잘못된 Target입니다 : " + gaveTarget);
                return false;
            }

            PoolType poolType = PoolType.None;
            if (Enum.IsDefined(typeof(PoolType), gaveType) == true &&
               ((PoolType)gaveType).GetPoolCategory() == PoolCategory.Stackable)
            {
                poolType = (PoolType)gaveType;
            }
            else
            {
                Debug.LogError("잘못된 PoolType입니다 : " + gaveType);
                return false;
            }

            if (gaveCount == 0)
            {
                Debug.LogErrorFormat("잘못된 갯수입니다 : " + gaveCount);
                return false;
            }
            else if (gaveCount > 100)
            {
                Debug.LogErrorFormat("갯수는 100개를 넘을 수 없습니다 : " + gaveCount);
                return false;
            }

            for (int i = 0; i < gaveCount; i++)
            {
                GiveItem(gaveInventory, poolType);
            }

            return true;
        }

        public void GiveItem(Inventory inventory, PoolType poolType)
        {
            PoolCategory poolCategory = poolType.GetPoolCategory();
            if (poolCategory != PoolCategory.Stackable)
            {
                Debug.LogError("해당 함수는 Stackable 타입만 가능합니다.");
                return;
            }

            inventory.Push(poolType);
        }

        public void EnqueueCustomer()
        {
            PoolType.Customer.EnablePool(OnBeforeEnablePool);
            void OnBeforeEnablePool(GameObject customerObj)
            {
                Transform enableTransform = GameManager.Instance.CustomerEnablePoint;
                customerObj.transform.position = enableTransform.position;
                customerObj.transform.forward = enableTransform.forward;

                Customer customer = customerObj.GetComponent<Customer>();
                CustomerWaitingLine waitingLine = GameManager.Instance.WaitingLine;

                if (waitingLine.TryEnqueue(customer) == false)
                {
                    customerObj.DisablePool(PoolType.Customer);
                }
            }
        }

        public void DequeueCustomer()
        {
            CustomerWaitingLine waitingLine = GameManager.Instance.WaitingLine;
            if (waitingLine.TryDequeue(out Customer customer) == true)
            {
                customer.gameObject.DisablePool(PoolType.Customer);
            }
        }
    }
}
