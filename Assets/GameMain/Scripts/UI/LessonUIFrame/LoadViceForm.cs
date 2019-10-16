using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class LoadViceForm : UGuiForm
    {
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //设置UI界面层级
            if (OriginalDepth < (int)UIFormId.LoadViceForm)
            {
                OriginalDepth += (int)UIFormId.LoadViceForm;
            }
        }

        protected override void OnClose(object userData)
        {
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}