using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using COM3D2API;
using HarmonyLib;
using LillyUtill.MyMaidActive;
using LillyUtill.MyWindowRect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.AnmCtr2.Plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "AnmCtr2";
        public const string PLAGIN_VERSION = "22.3.10";
        public const string PLAGIN_FULL_NAME = "COM3D2.AnmCtr2.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임    
    [BepInProcess("COM3D2x64.exe")]
    public class AnmCtr2 : BaseUnityPlugin
    {
        // 단축키 설정파일로 연동
        //private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        Harmony harmony;

        internal static ManualLogSource log;

        public WindowRectUtill myWindowRect;
        Vector2 scrollPosition;

        Maid maid = null;

        int seleted, anm;
        float time;
        int wrap;
        string[] type = new string[] { "one", "all" };

        /// <summary>
        ///  게임 실행시 한번만 실행됨
        /// </summary>
        public void Awake()
        {
            log = Logger;
            log.LogMessage("Awake");

            this.myWindowRect = new WindowRectUtill(Config, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, "AC2");

            MaidActiveUtill.setActiveMaidNum += setActiveMaid2;
            MaidActiveUtill.deactivateMaidNum += setActiveMaid2;

            AnmCtr2Utill.init();

        }

        private void setActiveMaid2(int maidn)
        {
            if (seleted == maidn)
            {
                maid = MaidActiveUtill.GetMaid(maidn);

                AnmCtr2Utill.MaidChg(seleted);
            }
        }

        public void OnEnable()
        {
            log.LogMessage("OnEnable");

          //  SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            harmony = Harmony.CreateAndPatchAll(typeof(AnmCtr2Utill));

        }

        /*
        */
        /// <summary>
        /// 게임 실행시 한번만 실행됨
        /// </summary>
        public void Start()
        {
            log.LogMessage("Start");


            // 이건 기어메뉴 아이콘
            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME
                , new Action(delegate ()
                { // 기어메뉴 아이콘 클릭시 작동할 기능
                    myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;
                })
                , MyAttribute.PLAGIN_FULL_NAME // 표시될 툴팁 내용                               
            , Properties.Resources.icon);// 표시될 아이콘
                                             // 아이콘은 이렇게 추가함
        }

        private void OnGUI()
        {
            if (!myWindowRect.IsGUIOn)
                return;

            //GUI.skin.window = ;

            //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
            // 별도 창을 띄우고 WindowFunction 를 실행함. 이건 스킨 설정 부분인데 따로 공부할것
            myWindowRect.WindowRect = GUILayout.Window(myWindowRect.winNum, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        public virtual void WindowFunction(int id)
        {
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
            // 라벨 추가
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsGUIOn = false; }
            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!myWindowRect.IsOpen)
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
                if (MaidActiveUtill.maidNames.Length > 0)
                {
                    //seleted = GUILayout.SelectionGrid(seleted, MaidActivePatch.maidNames, 3, GUILayout.Width(265));
                    seleted = MaidActiveUtill.SelectionGrid(seleted);
                    if (GUI.changed)
                    {
                        maid = MaidActiveUtill.GetMaid(seleted);
                        AnmCtr2Utill.MaidChg(seleted);
                        anm = 0;
                        //anmst.wrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), wrapModeNm[seleted3]);
                        //wrap = Array.FindIndex(AnmCtr2Utill.wrapModeNm, i => i == AnmCtr2Utill.wrapMode.ToString());
                        wrap = AnmCtr2Utill.wrap;
                        //wrap= AnmCtr2Utill.wrapModeNm.indexof( AnmCtr2Utill.wrapMode.ToString());
                        GUI.changed = false;
                    }

                    if (!maid)
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

        public void OnDisable()
        {
            log.LogMessage("OnDisable");

          //  SceneManager.sceneLoaded -= this.OnSceneLoaded;

            harmony.UnpatchSelf();// ==harmony.UnpatchAll(harmony.Id);

        }

    }
}
