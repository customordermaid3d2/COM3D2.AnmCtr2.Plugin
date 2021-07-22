using COM3D2.LillyUtill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.AnmCtr2.Plugin
{
    class AnmCtr2GUI : LillyUtill.MyGUI
    {
        private int seleted, seleted2;
        private float time;
        private int seleted3;

        public override void Update()
        {
            AnmCtr2Utill.TimeSet();
        }

        public override void WindowFunctionBody(int id)
        {
            //base.WindowFunctionBody(id);
            GUI.changed = false;

            GUILayout.Label("maid select");
            // 여기는 출력된 메이드들 이름만 가져옴
            // seleted 가 이름 위치 번호만 가져온건데
            seleted = GUILayout.SelectionGrid(seleted, MaidActivePatch.maidNames, 3);

            if (GUI.changed)
            {
                AnmCtr2Utill.MaidChg(seleted);
                GUI.changed = false;
            }

            GUILayout.Label("Animation select");
            // 여기는 출력된 메이드들 이름만 가져옴
            // seleted 가 이름 위치 번호만 가져온건데
            seleted2 = GUILayout.SelectionGrid(seleted2, AnmCtr2Utill.anmNm, 1);

            if (GUI.changed)
            {
                AnmCtr2Utill.AnmChg(seleted2);
                GUI.changed = false;
            }

            GUILayout.Label("time");
            time =GUILayout.HorizontalSlider(AnmCtr2Utill.time, 0, AnmCtr2Utill.length);

            if (GUI.changed)
            {
                AnmCtr2Utill.TimeChg(time);
                GUI.changed = false;
            }

            GUILayout.Label("WrapMode select");
            // 여기는 출력된 메이드들 이름만 가져옴
            // seleted 가 이름 위치 번호만 가져온건데
            seleted3 = GUILayout.SelectionGrid(seleted3, AnmCtr2Utill.wrapModeNm, 2);

            if (GUI.changed)
            {
                AnmCtr2Utill.WrapModeChg(seleted3);
                GUI.changed = false;
            }
        }
    }
}
