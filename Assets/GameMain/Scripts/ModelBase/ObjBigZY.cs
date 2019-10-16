using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
namespace Penny
{
    public class ObjBigZY: ObjBig
    {
        
        public override void ObjAction()
        {
            base.ObjAction();
            SetAnimation("dzy_zhua3", false);
        }

        public override void ObjRestAction()
        {
            base.ObjRestAction();
            if (te.IsComplete)
            {
                if (isReplay)
                {
                    te = SetAnimation("dzy_zhua3", false);
                    isReplay = false;
                }
                else
                {
                    SetState(ZYState.Normal);
                    clipTime = 0;
                }

            }
        }
    }
}


