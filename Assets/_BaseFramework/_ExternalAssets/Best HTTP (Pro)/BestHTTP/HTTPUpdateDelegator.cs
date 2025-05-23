/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEditor;
using UnityEngine;

#if NETFX_CORE || BUILD_FOR_WP8
    using System.Threading.Tasks;
#endif

namespace BestHTTP
{
    /// <summary>
    /// Delegates some U3D calls to the HTTPManager.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class HTTPUpdateDelegator : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance of the HTTPUpdateDelegator
        /// </summary>
        public static HTTPUpdateDelegator Instance { get; private set; }

        /// <summary>
        /// True, if the Instance property should hold a valid value.
        /// </summary>
        public static bool IsCreated { get; private set; }

        /// <summary>
        /// Set it true before any CheckInstance() call, or before any request send to dispatch callbacks on another thread.
        /// </summary>
        public static bool IsThreaded { get; set; }

        /// <summary>
        /// It's true if the dispatch thread running.
        /// </summary>
        public static bool IsThreadRunning { get; private set; }

        /// <summary>
        /// How much time the plugin should wait between two update call. Its default value 100 ms.
        /// </summary>
        public static int ThreadFrequencyInMS { get; set; }

        static HTTPUpdateDelegator()
        {
            ThreadFrequencyInMS = 100;
        }

        /// <summary>
        /// Will create the HTTPUpdateDelegator instance and set it up.
        /// </summary>
        public static void CheckInstance()
        {
            try
            {
                if (!IsCreated)
                {
                    GameObject go = GameObject.Find("HTTP Update Delegator");
                    if (go != null)
                        Instance = go.GetComponent<HTTPUpdateDelegator>();

                    if (Instance == null)
                    {
                        go = new GameObject("HTTP Update Delegator");
                        go.hideFlags = HideFlags.HideAndDontSave;
                        //go.hideFlags = HideFlags.DontSave;
                        GameObject.DontDestroyOnLoad(go);

                        Instance = go.AddComponent<HTTPUpdateDelegator>();
                    }
                    IsCreated = true;

#if UNITY_EDITOR
                    if (EditorApplication.isPlaying == false)
                    {
                        EditorApplication.update -= Instance.Update;
                        EditorApplication.update += Instance.Update;
                    }

                    EditorApplication.pauseStateChanged -= Instance.OnPlayModeStateChanged;
                    EditorApplication.pauseStateChanged += Instance.OnPlayModeStateChanged;
#endif
                }
            }
            catch
            {
                HTTPManager.Logger.Error("HTTPUpdateDelegator", "Please call the BestHTTP.HTTPManager.Setup() from one of Unity's event(eg. awake, start) before you send any request!");
            }
        }

        void Awake()
        {
#if !BESTHTTP_DISABLE_CACHING && (!UNITY_WEBGL || UNITY_EDITOR)
            Caching.HTTPCacheService.SetupCacheFolder();
#endif

#if !BESTHTTP_DISABLE_COOKIES && (!UNITY_WEBGL || UNITY_EDITOR)
            Cookies.CookieJar.SetupFolder();
            Cookies.CookieJar.Load();
#endif

#if UNITY_WEBGL
            // Threads are not implemented in WEBGL builds, disable it for now.
            IsThreaded = false;
#endif
            if (IsThreaded)
            {
#if NETFX_CORE
                Windows.System.Threading.ThreadPool.RunAsync(ThreadFunc);
#else
                new System.Threading.Thread(ThreadFunc)
                    .Start();
#endif
            }
        }

#if NETFX_CORE
        async
#endif
        void ThreadFunc(object obj)
        {
            IsThreadRunning = true;
            while(IsThreadRunning)
            {
                HTTPManager.OnUpdate();

#if NETFX_CORE
                await Task.Delay(ThreadFrequencyInMS);
#else
                System.Threading.Thread.Sleep(ThreadFrequencyInMS);
#endif
            }
        }

        void Update()
        {
            if (!IsThreaded)
                HTTPManager.OnUpdate();
        }

#if UNITY_EDITOR
        void OnPlayModeStateChanged(PauseState state)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.update -= Update;
            }
            else if (!EditorApplication.isPlaying)
            {
                EditorApplication.update += Update;
            }
        }
#endif

        void OnDisable()
        {
            OnApplicationQuit();
        }

        void OnApplicationQuit()
        {
            if (IsCreated == false)
            {
                return;
            }

            IsCreated = false;
            IsThreadRunning = true;

            HTTPManager.OnQuit();

#if UNITY_EDITOR
            EditorApplication.update -= Update;
            EditorApplication.pauseStateChanged -= OnPlayModeStateChanged;
#endif
        }
    }
}
