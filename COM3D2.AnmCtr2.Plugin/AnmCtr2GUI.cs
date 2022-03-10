using BepInEx.Configuration;

using COM3D2API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.AnmCtr2.Plugin
{
    class AnmCtr2GUI : MonoBehaviour// : LillyUtill.MyGUI
    {
        private int seleted, anm;
        private float time;
        private int wrap;

        public ConfigFile config;

        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        private Vector2 scrollPosition;

        // 위치 저장용 테스트 json
        public WindowRectUtill myWindowRect;

        public Maid maid = null;

        // public Bitmap icon;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="config"></param>
        /// <param name="windowName"></param>
        /// <param name="icon"></param>
        /// <param name="keyboardShortcut">new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftControl)</param>
        /// <returns></returns>
        public static AnmCtr2GUI Install(GameObject parent, ConfigFile config, string FullName, string ShortName, Bitmap icon, KeyboardShortcut keyboardShortcut) //where T : MyGUI
        {
            var instance = parent.GetComponent<AnmCtr2GUI>();
            if (instance == null)
            {

                instance = parent.AddComponent<AnmCtr2GUI>();
                //calls Start() on the object and initializes it.

                instance.StartAfterSetup(config, FullName, ShortName, keyboardShortcut);

                // 이건 기어메뉴 아이콘
                SystemShortcutAPI.AddButton(
                    FullName
                    , new Action(delegate ()
                    { // 기어메뉴 아이콘 클릭시 작동할 기능
                        instance.isGUIOn = !instance.isGUIOn;
                    })
                    , FullName // 표시될 툴팁 내용                               
                , MyUtill.ExtractResource(icon));// 표시될 아이콘
                // 아이콘은 이렇게 추가함

            }
            return instance;
        }

        public void StartAfterSetup(ConfigFile config, string FullName, string ShortName, KeyboardShortcut keyboardShortcut)
        {
            this.config = config;
            //this.keyboardShortcut = keyboardShortcut;
            this.IsGUIOn = config.Bind("GUI", "isGUIOn", false); // 이건 베핀 설정값 지정용                                                                         
            this.ShowCounter = config.Bind("GUI", "isGUIOnKey", keyboardShortcut);// 이건 단축키
            this.myWindowRect = new MyWindowRect(config, FullName, FullName, ShortName);

            MaidActivePatch.setActiveMaid2 += setActiveMaid2;
            MaidActivePatch.deactivateMaid += setActiveMaid2;
        }

        // 로딩중에 사용하면 안됨
        private void setActiveMaid2(int maidn)
        {
            if (seleted == maidn)
            {
                maid = MaidActivePatch.GetMaid(maidn);

                AnmCtr2Utill.MaidChg(seleted);

                //AnmCtr2Utill.AnmChg(seleted2);
                //AnmCtr2Utill.TimeChg(time);
                //AnmCtr2Utill.WrapModeChg(seleted3);
            }
        }

        /*
/// <summary>
/// 아까 부모 PresetExpresetXmlLoader 에서 봤던 로직이랑 같음
/// </summary>
public virtual void Awake()
{
   //MyLog.LogMessage("PresetExpresetXmlLoaderGUI.OnEnable");
}
// 이렇게 해서 플러그인 실행 직후는 작동 완료
*/
        public virtual void OnEnable()
        {
            //MyLog.LogMessage("PresetExpresetXmlLoaderGUI.OnEnable");

            myWindowRect?.load();// 이건 창 위치 설정하는건데 소스 열어서 따로 공부해볼것
            //SceneManager.sceneLoaded += this.OnSceneLoaded;
        }
        /*
        public virtual void Start()
        {
            //MyLog.LogMessage("PresetExpresetXmlLoaderGUI.Start");
            //myWindowRect = new MyWindowRect(config, windowName);
        }
        public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            myWindowRect.save();// 장면 이동시 GUI 창 위치 저장
        }
        */


        public void Update()
        {
            AnmCtr2Utill.TimeSet();
            if (ShowCounter.Value.IsUp())// 단축키가 일치할때
            {
                isGUIOn = !isGUIOn;// 보이거나 안보이게. 이런 배열이였네 지웠음
                //MyLog.LogMessage("IsUp", ShowCounter.Value.MainKey);
            }
        }


        // 매 화면 갱신할때마다(update 말하는게 아님)
        public virtual void OnGUI()
        {
            if (!isGUIOn)
                return;

            //GUI.skin.window = ;

            //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
            // 별도 창을 띄우고 WindowFunction 를 실행함. 이건 스킨 설정 부분인데 따로 공부할것
            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        string[] type = new string[] { "one", "all" };

        // 창일 따로 뜬 부분에서 작동
        public virtual void WindowFunction(int id)
        {
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
            // 라벨 추가
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { IsOpen = !IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn = false; }
            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!IsOpen)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

                //base.WindowFunctionBody(id);
                GUI.changed = false;

                GUILayout.Label("maid select");
                // 여기는 출력된 메이드들 이름만 가져옴
                // seleted 가 이름 위치 번호만 가져온건데
                if (MaidActivePatch.maidNames.Length > 0)
                {
                    //seleted = GUILayout.SelectionGrid(seleted, MaidActivePatch.maidNames, 3, GUILayout.Width(265));
                    seleted = MaidActivePatch.SelectionGrid3(seleted, 3, 265, false);
                    if (GUI.changed)
                    {
                        maid = MaidActivePatch.GetMaid(seleted);
                        AnmCtr2Utill.MaidChg(seleted);
                        anm = 0;
                        //anmst.wrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), wrapModeNm[seleted3]);
                        //wrap = Array.FindIndex(AnmCtr2Utill.wrapModeNm, i => i == AnmCtr2Utill.wrapMode.ToString());
                        wrap = AnmCtr2Utill.wrap;
                        //wrap= AnmCtr2Utill.wrapModeNm.indexof( AnmCtr2Utill.wrapMode.ToString());
                        GUI.changed = false;
                    }

                    if (!maid )
                    {
                        GUILayout.Label("maid null or anmNm.Length==0");
                        if (maid)
                        {
                            AnmCtr2Utill.MaidChg(seleted);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Animation select");
                        // 여기는 출력된 메이드들 이름만 가져옴
                        // seleted 가 이름 위치 번호만 가져온건데

                        anm = GUILayout.SelectionGrid(anm, AnmCtr2Utill.anmNm, 1, GUILayout.Width(265));

                        if (GUI.changed)
                        {
                            AnmCtr2Utill.AnmChg(anm);
                            GUI.changed = false;
                        }

                        GUILayout.Label($"time , {AnmCtr2Utill.time} , {AnmCtr2Utill.length}");
                        //time = GUILayout.HorizontalSlider(AnmCtr2Utill.time, 0, AnmCtr2Utill.length, GUILayout.Width(265));
                        time = GUILayout.HorizontalSlider(AnmCtr2Utill.time % 1f, 0, 1, GUILayout.Width(265));

                        if (GUI.changed)
                        {
                            AnmCtr2Utill.TimeChg(time);
                            GUI.changed = false;
                        }

                        GUILayout.BeginHorizontal();// 가로 정렬

                        if (GUILayout.Button("Play")) { AnmCtr2Utill.Play(); }
                        if (GUILayout.Button("Stop")) { AnmCtr2Utill.Stop(); }

                        GUILayout.EndHorizontal();// 가로 정렬 끝

                        GUILayout.Label("WrapMode select");
                        // 여기는 출력된 메이드들 이름만 가져옴
                        // seleted 가 이름 위치 번호만 가져온건데
                        wrap = GUILayout.SelectionGrid(wrap, AnmCtr2Utill.wrapModeNm, 2, GUILayout.Width(265));

                        if (GUI.changed)
                        {
                            AnmCtr2Utill.WrapModeChg(wrap);
                            GUI.changed = false;
                        }

                    }

                }


                GUILayout.EndScrollView();

            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        /*
        /// <summary>
        /// 게임 X 버튼 눌렀을때 반응
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            myWindowRect?.save();
            //MyLog.LogMessage("OnApplicationQuit");
        }

        /// <summary>
        /// 게임 종료시에도 호출됨
        /// </summary>
        public virtual void OnDisable()
        {
            myWindowRect?.save();
            //SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }
        */
    }
}
