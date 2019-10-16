using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class ObjLightFish : ObjBase
    {


        public override void ObjAction()
        {
            base.ObjAction();
            SetAnimation("effect_1", true);
        }
        public override void ObjRestAction()
        {
            base.ObjRestAction();
            AnimationState.SetEmptyAnimation(0, 0);
        }
    }
}


