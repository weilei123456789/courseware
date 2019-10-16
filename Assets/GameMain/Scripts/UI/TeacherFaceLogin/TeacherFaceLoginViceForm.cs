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
    public class TeacherFaceLoginViceForm : UGuiForm
    {
        [SerializeField]
        private Image m_ButtonImage = null;

        private TeacherFaceLoginForm m_TeacherFaceLoginForm = null;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_TeacherFaceLoginForm = (TeacherFaceLoginForm)userData;
            //GameEntry.Event.Subscribe(LeiDaGameObjectEventArgs.EventId, OnLidarHitEvent);
            GameEntry.Windows.SubscribeUIGroundEvent(OnLidarHitEvent);
        }

        protected override void OnClose(object userData)
        {
            //GameEntry.Event.Unsubscribe(LeiDaGameObjectEventArgs.EventId, OnLidarHitEvent);
            GameEntry.Windows.UnSubscribeUIGroundEvent(OnLidarHitEvent);
            m_TeacherFaceLoginForm = null;
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
            if (go == null) return;
            //Log.Info(ne.ChangeGo.name);

            if (m_ButtonImage.gameObject == go)
            {
                if (m_TeacherFaceLoginForm)
                    m_TeacherFaceLoginForm.TakePhoto();
            }
        }

    }
}