using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CheatController : MonoSingleton<CheatController>
    {
        private Dictionary<string, System.Action> CHEAT_COMMAND_LIST;

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

        private void Awake()
        {
            playerInventory = Player.Instance.GetComponent<Inventory>();
            employeeInventory = Object.FindObjectOfType<Employee>().GetComponent<Inventory>();

            CHEAT_COMMAND_LIST = new Dictionary<string, System.Action>()
            {
                {"give_player_50", () => GiveItem(playerInventory, PoolType.Stackable_Wood) },
                {"give_player_51", () => GiveItem(playerInventory, PoolType.Stackable_RawMeat) },
                {"give_player_52", () => GiveItem(playerInventory, PoolType.Stackable_CookedMeat) },

                {"give_employee_50", () => GiveItem(employeeInventory, PoolType.Stackable_Wood) },
                {"give_employee_51", () => GiveItem(employeeInventory, PoolType.Stackable_RawMeat) },
                {"give_employee_52", () => GiveItem(employeeInventory, PoolType.Stackable_CookedMeat) },

                {"enqueue_customer", EnqueueCustomer },
                {"dequeue_customer", DequeueCustomer }
            };
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
        }

        public bool TrySubmit(string inputText)
        {
            if (CHEAT_COMMAND_LIST.ContainsKey(inputText) == true)
            {
                CHEAT_COMMAND_LIST[inputText].Invoke();
                return true;
            }
            else
            {
                Debug.LogError("Cheat command not found : " + inputText);
                return false;
            }
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
                Vector3 enablePoint = GameManager.Instance.CustomerEnablePoint.position;
                customerObj.transform.position = enablePoint;
                customerObj.transform.rotation = Quaternion.identity;

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
