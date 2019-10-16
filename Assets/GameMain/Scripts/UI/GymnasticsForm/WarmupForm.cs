using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class WarmupForm : UGuiForm
    {

        private int m_WarmupViceForm = -1;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_WarmupViceForm = (int)GameEntry.UI.OpenUIForm(UIFormId.WarmupViceForm);

        }
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

        }

        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_WarmupViceForm))
                GameEntry.UI.CloseUIForm(m_WarmupViceForm);

            base.OnClose(userData);
        }
    }
}