using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class EndingForm : UGuiForm
    {

        private int m_EndingViceForm = -1;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_EndingViceForm = (int)GameEntry.UI.OpenUIForm(UIFormId.EndingViceForm);


        }
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
   
        }

        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_EndingViceForm))
                GameEntry.UI.CloseUIForm(m_EndingViceForm);

            base.OnClose(userData);
        }
    }
}