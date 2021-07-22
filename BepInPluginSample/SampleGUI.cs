using BepInEx;
using BepInEx.Configuration;
using COM3D2.LillyUtill;
using COM3D2API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BepInPluginSample
{


    class SampleGUI : MonoBehaviour
    {
        public static SampleGUI instance;

        private static ConfigFile config;

        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        private static int windowId = new System.Random().Next();

        private static Vector2 scrollPosition;

        // 위치 저장용 테스트 json
        public static MyWindowRect myWindowRect;

        public static bool IsOpen
        {
            get => myWindowRect.IsOpen;
            set => myWindowRect.IsOpen = value;            
        }

        // GUI ON OFF 설정파일로 저장
        private static ConfigEntry<bool> IsGUIOn;

        public static bool isGUIOn
        {
            get => IsGUIOn.Value;
            set => IsGUIOn.Value = value;
        }

        internal static SampleGUI Install(GameObject parent,ConfigFile config)
        {
            SampleGUI. config = config;
            instance = parent.GetComponent<SampleGUI>();
            if (instance == null)
            {
                instance = parent.AddComponent<SampleGUI>();
                MyLog.LogMessage("GameObjectMgr.Install", instance.name);                
            }
            return instance;
        }

        public void Awake()
        {
            myWindowRect = new MyWindowRect(config , MyAttribute.PLAGIN_FULL_NAME+"win");
            IsGUIOn = config.Bind("GUI", "isGUIOn", false);
            ShowCounter = config.Bind("GUI", "isGUIOnKey", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha9, KeyCode.LeftControl));
            SystemShortcutAPI.AddButton(MyAttribute.PLAGIN_FULL_NAME, new Action(delegate () { SampleGUI.isGUIOn = !SampleGUI.isGUIOn; }), MyAttribute.PLAGIN_NAME + " : " + ShowCounter.Value.ToString(), MyUtill.ExtractResource(BepInPluginSample.Properties.Resources.icon));
        }

        public void OnEnable()
        {
            MyLog.LogMessage("OnEnable");

            SampleGUI.myWindowRect.load();
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        public void Start()
        {
            MyLog.LogMessage("Start");            
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SampleGUI.myWindowRect.save();
        }

        private void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            if (ShowCounter.Value.IsUp())
            {
                isGUIOn = !isGUIOn;
                MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            }
        }

        public void OnGUI()
        {
            if (!isGUIOn)
                return;

            //GUI.skin.window = ;

            //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
            //GUI.skin.box.
            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        public void WindowFunction(int id)
        {
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Label(MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(),GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { IsOpen = !IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn = false; }
            GUILayout.EndHorizontal();

            if (!IsOpen)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                if (GUILayout.Button("sample1 Start Coroutine"))
                {
                    Debug.Log("Button1");
                    Sample.sample.StartCoroutine(MyCoroutine());
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("sample5"))
                {
                    Debug.Log("Button5");
                }
                GUILayout.BeginVertical();
                if (GUILayout.Button("sample7"))
                {
                    Debug.Log("Button7");
                }
                GUI.enabled = false;
                if (GUILayout.Button("sample3"))
                {
                    Debug.Log("Button3");
                }
                GUILayout.EndVertical();
                
                if (GUILayout.Button("sample4"))
                {
                    Debug.Log("Button4");
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                if (GUILayout.Button("sample2 Stop Coroutine"))
                {
                    Debug.Log("Button2");
                    isCoroutine = false;
                }
                GUILayout.EndScrollView();

                if (GUI.changed)
                {
                    Debug.Log("changed");
                }

            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        public void OnDisable()
        {
            SampleGUI.isCoroutine = false;
            SampleGUI.myWindowRect.save();
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        public static bool isCoroutine = false;
        public static int CoroutineCount = 0;

        private IEnumerator MyCoroutine()
        {
            isCoroutine = true;
            while (isCoroutine)
            {
                MyLog.LogMessage("MyCoroutine ", ++CoroutineCount);
                //yield return null;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
