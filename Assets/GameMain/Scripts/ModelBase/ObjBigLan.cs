using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
namespace Penny
{
    public class ObjBigLan: ObjBig
    {
        
        public override void ObjAction()
        {
            base.ObjAction();
            SetAnimation("xiaoqing_wait", false);
        }

        public override void ObjRestAction()
        {
            base.ObjRestAction();
            if (te.IsComplete)
            {
                
                    SetState(ZYState.Normal);
                    clipTime = 0;
                

            }
        }
    }
}


