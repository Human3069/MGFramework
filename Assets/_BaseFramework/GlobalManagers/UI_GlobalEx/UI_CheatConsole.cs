using System;
using TMPro;
using UnityEngine;

namespace MGFramework
{
    public class UI_CheatConsole : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private TextMeshProUGUI logText;
        [SerializeField]
        private GameObject consolePanelObj;

        [Space(10)]
        [SerializeField]
        private int logFontSize = 36;
        [SerializeField]
        private int datetimeFontSize = 30;

        private void Awake()
        {
            consolePanelObj.gameObject.SetActive(false);
            CheatController.Instance.OnCheatConsoleStateChanged += OnCheatConsoleStateChanged;

            inputField.onSubmit.AddListener(OnInputFieldSubmit);
        }

        private void OnInputFieldSubmit(string inputText)
        {
            string command = inputText.Replace(" ", "");
            if (string.IsNullOrEmpty(command) == false)
            {
                if (CheatController.Instance.TrySubmit(command) == true)
                {
                    DateTime currentDateTime = DateTime.Now;
                    string currentTimeText = "<size=" + datetimeFontSize + "><color=#BFBFBF>" + " - " + currentDateTime.ToString("HH:mm:ss") + "</color></size>";

                    logText.text += "<size=" + logFontSize + "><b>" + command + "</b></size>" + currentTimeText + "\n";
                }
            }

            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }

        private void OnCheatConsoleStateChanged(bool isOn)
        {
            consolePanelObj.gameObject.SetActive(isOn);

            if (isOn == true)
            {
                inputField.ActivateInputField();
            }
        }
    }
}
