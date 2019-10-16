using GameFramework;
using PaintTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class InitializationForm : UGuiForm
    {
        [SerializeField]
        private Text m_Text = null;
        [SerializeField]
        private RawImage m_RawImage = null;
        [SerializeField]
        private PaintView m_PaintView;

        private int m_InitializationViceSerialId = -1;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_RawImage.texture = GameEntry.VideoPlayer.Texture;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameEntry.Windows.SubscribeUIWallEvent(OnLidarHitEvent);
            m_Text.text = "";
            GameEntry.VideoPlayer.PlayLoadMovice("xPiNiMV");
            m_InitializationViceSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.InitializationViceForm, this);
        }

        protected override void OnClose(object userData)
        {
            GameEntry.Windows.UnSubscribeUIWallEvent(OnLidarHitEvent);
            GameEntry.VideoPlayer.Stop();
            if (GameEntry.UI.HasUIForm(m_InitializationViceSerialId))
                GameEntry.UI.CloseUIForm(m_InitializationViceSerialId);
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (GameEntry.GameManager.IsMouseDebug)
                GameEntry.Windows.WallUICameraRay(Input.mousePosition);

            //if (GameEntry.Windows.WallTouchCount <= 0)
            //{
            //    m_PaintView.ClearPosition();
            //}
           
        }

        private void OnLidarHitEvent(GameObject go, Vector3 vec)
        {
            if (go == null) return;

            if (m_PaintView.gameObject == go)
            {
                //m_PaintView.Paint(vec);
            }
        }

    }
}