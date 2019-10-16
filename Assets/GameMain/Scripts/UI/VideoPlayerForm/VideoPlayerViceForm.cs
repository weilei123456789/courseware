using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class VideoPlayerViceForm : UGuiForm
    {

        public Image GroundBG = null;
        private VideoPlayerParams VPParams;

        private DRLesson drl;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            VPParams = userData as VideoPlayerParams;

            drl = VPParams.delesson;

            ShowBGSprite(VPParams.HasGSprite);

        }

        protected override void OnClose(object userData)
        {

            ShowBGSprite(false);

            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        private void ShowBGSprite(bool IsShow)
        {
            if (IsShow)
            {
                //GroundBG.sprite = m_Gsprite;
                ResourceUtility.LoadLessonUISprite(drl.GroundBG, drl.SeasonPath, drl.LessonPath, GroundBG);
            }
            else
            {
                GroundBG.sprite = null;
            }
            GroundBG.gameObject.SetActive(IsShow);
        }

    }
}