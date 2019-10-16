using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class FruitCuttingForm : UGuiForm
    {
        [SerializeField]
        private RawImage m_RawImage = null;

        private int m_FruitCuttingViceSerialId = -1;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_FruitCuttingViceSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.FruitCuttingViceForm, this);
        }

        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_FruitCuttingViceSerialId))
                GameEntry.UI.CloseUIForm(m_FruitCuttingViceSerialId);
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (KinectManager.Instance && KinectManager.Instance.IsInitialized())
            {
                if (KinectManager.Instance.IsUserDetected())
                {
                    if (m_RawImage)
                    {
                        m_RawImage.color = Color.white;
                        m_RawImage.texture = KinectManager.Instance.GetUsersLblTex();//获取深度数据流
                    }
                }
            }
        }


    }
}