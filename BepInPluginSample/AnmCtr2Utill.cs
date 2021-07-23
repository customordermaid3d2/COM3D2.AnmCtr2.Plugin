using COM3D2.LillyUtill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.AnmCtr2.Plugin
{
    class AnmCtr2Utill
    {
		public static string[] anmNm=new string[] { };

		public static int seleted;

		public static Animation anm = null;
		public static AnimationState anmst=null;

		public static float time;
        public static float speed;
        public static float length;
        public static WrapMode wrapMode;

		public static string[] wrapModeNm;//= new string[] { };

		internal static void init()
        {
			wrapModeNm=Enum.GetNames(typeof(WrapMode));
        }


		internal static void MaidChg(int seleted)
        {
			AnmCtr2Utill.seleted = seleted;

            if (!MaidActivePatch.maids[seleted])
            {
				anm = null;
				anmst = null;
				return;
            }

            anm = MaidActivePatch.maids[seleted].GetAnimation();
			IEnumerator enumerator = anm.GetEnumerator();

			List<string> l = new List<string>();

			while (enumerator.MoveNext())
			{
				AnimationState animationState = (AnimationState)enumerator.Current;
				l.Add(animationState.name);
			}

			anmNm = l.ToArray();

			AnmCtr2Utill.AnmChg(0);
		}

        internal static void AnmChg(int seleted2)
        {
            anmst =anm[anmNm[seleted2]];
			time = anmst.time ;
			speed = anmst.speed ;
			length = anmst.length ;
			wrapMode = anmst.wrapMode ;
		}

        internal static void TimeChg(float time)
        {
            if (anmst == null)
            {
				return;
            }
			anmst.time = time;
		}

		internal static void TimeSet()
		{
			if (anmst == null)
			{
				return;
			}
			time = anmst.time;
		}

        internal static void WrapModeChg(int seleted3)
        {
			anmst.wrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), wrapModeNm[seleted3]);
		}
    }
}
