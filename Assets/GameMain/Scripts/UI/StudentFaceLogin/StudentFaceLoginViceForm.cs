using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class StudentFaceLoginViceForm : UGuiForm
    {
        [SerializeField]
        private Image m_ButtonImage = null;

        private StudentFaceLoginForm m_StudentFaceLoginForm = null;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_StudentFaceLoginForm = (StudentFaceLoginForm)userData;
            //GameEntry.Event.Subscribe(LeiDaGameObjectEventArgs.EventId, OnLidarHitEvent);
            GameEntry.Windows.SubscribeUIGroundEvent(OnLidarHitEvent);

        }

        protected override void OnClose(object userData)
        {
            m_StudentFaceLoginForm = null;
            //GameEntry.Event.Unsubscribe(LeiDaGameObjectEventArgs.EventId, OnLidarHitEvent);
            GameEntry.Windows.UnSubscribeUIGroundEvent(OnLidarHitEvent);
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                GameEntry.Windows.GroundUICameraRay(Input.mousePosition);
        }

        private void OnLidarHitEvent(GameObject go, Vector3 vec)
        {
            //LeiDaGameObjectEventArgs ne = (LeiDaGameObjectEventArgs)e;
            if (go == null) return;

            if (m_ButtonImage.gameObject == go)
            {
                if (m_StudentFaceLoginForm)
                    m_StudentFaceLoginForm.TakePhoto();
            }
        }

    }
}