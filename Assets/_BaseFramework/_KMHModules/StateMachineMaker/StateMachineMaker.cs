using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace _KMH_Framework
{
    public class StateMachineMaker : EditorWindow
    {
        private const string LOG_FORMAT = "<color=white><b>[StateMachineMaker]</b></color> {0}";

        private string entityName = "Monster";
        private string namespaceName = "";
        private List<string> stateNames = new List<string> { "Idle", "Attack" };
        private string folderPath = "Assets/Scripts/Generated";

        [MenuItem("Tools/State Machine Maker")]
        public static void OpenWindow()
        {
            GetWindow<StateMachineMaker>("State Machine Maker");
        }

        private void OnGUI()
        {
            GUILayout.Label("State Machine Generator", EditorStyles.boldLabel);

            entityName = EditorGUILayout.TextField("Entity Name", entityName);
            namespaceName = EditorGUILayout.TextField("Namespace", namespaceName);
            folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

            GUILayout.Space(10);
            GUILayout.Label("State Names", EditorStyles.label);

            for (int i = 0; i < stateNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                stateNames[i] = EditorGUILayout.TextField($"State {i + 1}", stateNames[i]);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    stateNames.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add State"))
            {
                stateNames.Add("");
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Owner"))
            {
                GenerateOwner();
            }

            if (GUILayout.Button("Generate Interface"))
            {
                GenerateInterface();
            }

            if (GUILayout.Button("Generate States"))
            {
                GenerateStates();
            }

            if (GUILayout.Button("Generate StateMachine"))
            {
                GenerateStateMachine();
            }

            if (GUILayout.Button("Generate Context"))
            {
                GenerateContext();
            }

            if (GUILayout.Button("Generate Data"))
            {
                GenerateData();
            }

            if (GUILayout.Button("Generate AnimationController"))
            {
                GenerateAnimationController();
            }

            if (GUILayout.Button("Generate Utility"))
            {
                GenerateUtility();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Generate All"))
            {
                GenerateAll();
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                Debug.LogError("Entity Name을 입력해주세요.");
                return false;
            }

            if (stateNames.Count == 0)
            {
                Debug.LogError("최소 하나 이상의 상태명을 입력해주세요.");
                return false;
            }

            return true;
        }

        private void GenerateOwner()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + entityName + ".cs";
                string generatedText = StateMachineTempletes.Owner(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateInterface()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/I" + entityName + "State.cs";
                string generatedText = StateMachineTempletes.Interface(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateStates()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                for (int i = 0; i < stateNames.Count; i++)
                {
                    string stateName = stateNames[i];
                    string className = i + "_" + stateName + entityName + "State.cs";
                    string path = folderPath + "/" + className;
                    string generatedText = StateMachineTempletes.State(i, stateName, entityName, namespaceName);

                    File.WriteAllText(path, generatedText);
                }

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateStateMachine()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + entityName + "StateMachine.cs";
                string generatedText = StateMachineTempletes.StateMachine(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateContext()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + entityName + "Context.cs";
                string generatedText = StateMachineTempletes.Context(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateData()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + entityName + "Data.cs";
                string generatedText = StateMachineTempletes.Data(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateAnimationController()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + entityName + "AnimationController.cs";
                string generatedText = StateMachineTempletes.AnimationController(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateUtility()
        {
            if (Validate() == true)
            {
                Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + entityName + "Utility.cs";
                string generatedText = StateMachineTempletes.Utility(entityName, namespaceName);
                File.WriteAllText(path, generatedText);

                AssetDatabase.Refresh();
                Debug.LogFormat(LOG_FORMAT, "상태머신 생성 완료");
            }
            else
            {
                Debug.LogWarningFormat(LOG_FORMAT, "상태머신 생성 실패");
            }
        }

        private void GenerateAll()
        {
            GenerateOwner();
            GenerateInterface();
            GenerateStates();
            GenerateStateMachine();
            GenerateContext();
            GenerateData();
            GenerateAnimationController();
            GenerateUtility();
        }
    }
}