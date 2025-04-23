using _KMH_Framework;
using _KMH_Framework.Pool;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// ~ Ű�� ������ CheatConsole�� ���� cheatCommand�� �Է��Ͽ� Cheat�� ����� �� �ֽ��ϴ�. Ȥ�� binding�� Ű�� ������ ����� �� �ֽ��ϴ�.
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

        private bool _isNoclip = false;
        private bool IsNoclip
        {
            get
            {
                return _isNoclip;
            }
            set
            {
                if (_isNoclip != value)
                {
                    _isNoclip = value;
                    if (value == true)
                    {
                        NoclipAsync().Forget();
                    }

                    OnNoclipStateChanged?.Invoke(value);
                }
            }
        }

        private async UniTaskVoid NoclipAsync()
        {
            while (IsNoclip == true)
            {
                if (Input.GetMouseButtonDown(1) == true)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) == true)
                    {
                        CharacterController controller = Player.Instance.GetComponent<CharacterController>();
                        controller.enabled = false;

                        Player.Instance.transform.position = hit.point;
                        controller.enabled = true;
                    }
                }

                await UniTask.Yield();
            }
        }

        public delegate void CheatConsoleDelegate(bool isOn);
        public event CheatConsoleDelegate OnCheatConsoleStateChanged;

        public delegate void SubmitCheatCommandDelegate(string command);
        public event SubmitCheatCommandDelegate OnSubmitCheatCommand;

        public delegate void NoclipDelegate(bool isOn);
        public event NoclipDelegate OnNoclipStateChanged;

        private void Awake()
        {
            playerInventory = Player.Instance.GetComponent<Inventory>();
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

            if (IsOn == false)
            {
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
        }

        public bool TrySubmit(string inputText)
        {
            Regex giveGoldPatternRegex = new Regex(@"^give_gold_([0-9]+)$");
            Regex singlePatternRegex = new Regex(@"^give_([a-zA-Z0-9]+)_([0-9]+)$");
            Regex multiplePatternRegex = new Regex(@"^give_([a-zA-Z0-9]+)_([0-9]+)_([0-9]+)$");
        
            Match giveGoldMatch = giveGoldPatternRegex.Match(inputText);
            Match singlesMatch = singlePatternRegex.Match(inputText);
            Match multiplesMatch = multiplePatternRegex.Match(inputText);
      
            if (giveGoldMatch.Success == true)
            {
                int gaveAmount = int.Parse(giveGoldMatch.Groups[1].Value);

                GivePlayerGold(gaveAmount);
                OnSubmitCheatCommand?.Invoke(inputText);
                return true;
            }
            else if (multiplesMatch.Success == true)
            {
                string gaveTarget = multiplesMatch.Groups[1].Value;
                int gaveType = int.Parse(multiplesMatch.Groups[2].Value);
                uint gaveCount = uint.Parse(multiplesMatch.Groups[3].Value);

                OnSubmitCheatCommand?.Invoke(inputText);
                return TryIntermediator(gaveTarget, gaveType, gaveCount);
            }
            else if (singlesMatch.Success == true)
            {
                string gaveTarget = singlesMatch.Groups[1].Value;
                int gaveType = int.Parse(singlesMatch.Groups[2].Value);

                OnSubmitCheatCommand?.Invoke(inputText);
                return TryIntermediator(gaveTarget, gaveType, 1);
            }
            else if (inputText.Equals("noclip") == true)
            {
                IsNoclip = !IsNoclip;
                return true;
            }
            else if (inputText.Equals("enqueue_customer") == true)
            {
                GameManager.Instance.CustomerManager.EnqueueCustomer();
                OnSubmitCheatCommand?.Invoke(inputText);
                return true;
            }
            else if (inputText.Equals("dequeue_customer") == true)
            {
                RemoveCustomer();
                OnSubmitCheatCommand?.Invoke(inputText);
                return true;
            }
            else
            {
                Debug.LogError("�߸��� Command�Դϴ� : " + inputText);
                return false;
            }
        }

        /// <summary>
        /// Regex�� ���� �Ѱܹ��� ���� ó���մϴ�.
        /// </summary>
        /// <param name="gaveTarget">player Ȥ�� employee �Ǻ�</param>
        /// <param name="gaveType">������ Ÿ��</param>
        /// <param name="gaveCount">������ ����</param>
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
                Debug.LogError("�߸��� Target�Դϴ� : " + gaveTarget);
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
                Debug.LogError("�߸��� PoolType�Դϴ� : " + gaveType);
                return false;
            }

            if (gaveCount == 0)
            {
                Debug.LogErrorFormat("�߸��� �����Դϴ� : " + gaveCount);
                return false;
            }
            else if (gaveCount > 100)
            {
                Debug.LogErrorFormat("������ 100���� ���� �� �����ϴ� : " + gaveCount);
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
                Debug.LogError("�ش� �Լ��� Stackable Ÿ�Ը� �����մϴ�.");
                return;
            }

            inventory.Push(poolType);
        }

        public void RemoveCustomer()
        {
            CustomerWaitingLine waitingLine = GameManager.Instance.CustomerManager.WaitingLine;
            if (waitingLine.TryDequeue(out Customer customer) == true)
            {
                customer.gameObject.DisablePool(PoolType.Customer);
            }
        }

        public void GivePlayerGold(int gold)
        {
            GameManager.Instance.GoldManager.Add(gold);
        }
    }
}
