
using BepInEx.Configuration;
using HarmonyLib;
using LillyUtill.MyMaidActive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.AnmCtr2.Plugin
{
    public static class AnmCtr2Utill
    {
        public static string[] anmNm = new string[] { };

        public static int seleted;
        public static int wrap;
        public static int seletedAnm;

        public static Animation anm = null;
        public static AnimationState anmst = null;

        public static float time;
        public static float speed;
        public static float length;
        public static WrapMode wrapMode;

        public static string[] wrapModeNm;//= new string[] { };
        public static WrapMode[] wrapModes;//= new string[] { };

        public static Maid maid = null;

        public static ConfigEntry<bool> isLog;

        internal static void init(BepInEx.Logging.ManualLogSource logger, ConfigFile config)
        {
            wrapModeNm = Enum.GetNames(typeof(WrapMode));
            wrapModes = (WrapMode[])Enum.GetValues(typeof(WrapMode));

            isLog = config.Bind("Utill", "isLog", false);
        }

        public static void MaidChg()
        {
            maid = MaidActiveUtill.GetMaid(seleted);

            if (!maid || !maid.body0.m_Bones)
            {
                //AnmCtr2.myLog.LogInfo("MaidChg null");
                anm = null;
                anmst = null;
                anmNm = new string[] { };
                return;
            }
            //AnmCtr2.myLog.LogInfo("MaidChg set");

            anm = maid.GetAnimation();
            IEnumerator enumerator = anm.GetEnumerator();

            List<string> l = new List<string>();

            while (enumerator.MoveNext())
            {
                AnimationState animationState = (AnimationState)enumerator.Current;
                l.Add(animationState.name);
            }

            anmNm = l.ToArray();


            AnmCtr2Utill.AnmChg();
        }

        internal static void AnmChg()
        {
            try
            {
                if (anmNm.Length == 0)
                {
                    anmst = null;
                    return;
                }
                if (anmNm.Length <= seletedAnm)
                {
                    seletedAnm = 0;
                }
                anmst = anm[anmNm[seletedAnm]];
                // https://docs.unity3d.com/kr/530/ScriptReference/AnimationState-time.html
                // 사용 금지. 재생시간 값 이상으로 무한대로 가버림. 즉 얼마간 방치되있는지 확인 용도
                //time = anmst.time ;
                time = anmst.normalizedTime;
                speed = anmst.speed;
                length = anmst.length;
                wrapMode = anmst.wrapMode;
                wrap = Array.IndexOf(wrapModes, wrapMode);
            }
            catch (Exception e)
            {
                AnmCtr2.log.LogError($"AnmChg {e.ToString()}");
            }
        }

        internal static void TimeSet()
        {
            if (anmst == null)
            {
                return;
            }
            anmst.normalizedTime = time;
        }

        internal static void TimeChg()
        {
            if (anmst == null)
            {
                return;
            }
            time = anmst.normalizedTime;
        }

        internal static void WrapModeChg(int seleted3)
        {
            //anmst.wrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), wrapModeNm[seleted3]);
            wrap = seleted3;
            if (anmst == null)
            {
                return;
            }
            anmst.wrapMode = wrapModes[seleted3];
        }

        // 효과 없음
        // public extern void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame);
        [HarmonyPatch(typeof(Animation), "AddClip", typeof(AnimationClip), typeof(string), typeof(int), typeof(int), typeof(bool))]
        [HarmonyPostfix]
        public static void AddClip(Animation __instance, AnimationClip clip, string newName, int firstFrame, int lastFrame, bool addLoopFrame)
        {
#if DEBUG
#endif
            if (isLog.Value)
                AnmCtr2.log.LogInfo($"AddClip , {__instance == anm} , {newName}, {firstFrame}, {lastFrame}, {addLoopFrame}");

            if (__instance == anm)
            {
                MaidChg();
            }
        }

        // 쓰지 말기. 루프 돌음
        //internal extern AnimationState GetState(string name);
        //[HarmonyPatch(typeof(Animation), "GetState",typeof(string))]
        //[HarmonyPostfix]
        //public static void GetState(AnimationState __result, Animation __instance, string name)
        //{            
        //    if (__instance == anm)
        //    {
        //        AnmCtr2.log.LogInfo($"GetState , {name}");
        //        MaidChg();
        //    }
        //}


        // public AnimationState LoadAnime(string tag, AFileSystemBase fileSystem, string filename, bool additive, bool loop)
        [HarmonyPatch(typeof(TBody), "LoadAnime", typeof(string), typeof(AFileSystemBase), typeof(string), typeof(bool), typeof(bool))]
        [HarmonyPostfix]
        public static void LoadAnime(AnimationState __result, TBody __instance, string tag, AFileSystemBase fileSystem, string filename, bool additive, bool loop)
        {
#if DEBUG
#endif
            try
            {
                if (maid == null || maid.boMAN)
                {
                    return;
                }
                if (isLog.Value)
                    AnmCtr2.log.LogInfo($"LoadAnime1 , {maid.body0 == __instance}  , {tag}, {filename}, {additive}, {loop}");
                if (maid.body0 == __instance)
                {
                    MaidChg();
                }
            }
            catch (Exception e)
            {
                AnmCtr2.log.LogError($"LoadAnime1 {e}");
            }
        }

        // public AnimationState LoadAnime(string tag, byte[] byte_data, bool additive, bool loop)
        [HarmonyPatch(typeof(TBody), "LoadAnime", typeof(string), typeof(byte[]), typeof(bool), typeof(bool))]
        [HarmonyPostfix]
        public static void LoadAnime(AnimationState __result, TBody __instance, string tag, byte[] byte_data, bool additive, bool loop)
        {
#if DEBUG
#endif
            try
            {
                if (maid == null || maid.boMAN)
                {
                    return;
                }
                if (isLog.Value)
                    AnmCtr2.log.LogInfo($"LoadAnime2 , {maid.body0 == __instance} , {tag}, {byte_data.Length}, {additive}, {loop}");
                if (maid.body0 == __instance)
                {
                    MaidChg();
                }
            }
            catch (Exception e)
            {
                AnmCtr2.log.LogError($"LoadAnime2 {e}");
            }
        }

        // 효과 없음
        /*
 [HarmonyPatch(typeof(CharacterMgr), "SetActive")]
 [HarmonyPostfix]
 public static void SetActive(Maid f_maid, int f_nActiveSlotNo, bool f_bMan)
 {
     if (f_maid !=null && f_maid == maid)
     {
         AnmCtr2.log.LogInfo($"SetActive , {f_maid.status.fullNameEnStyle}, {f_nActiveSlotNo}, {f_bMan}");
         MaidChg();
     }
 }        
        */



        internal static void Stop()
        {
            anmst.speed = 0;
        }

        internal static void Play()
        {
            anmst.speed = 1;

        }
    }
}
