using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Penny
{
    public class ObjHaiDanNormal : ObjBase
    {

       

       

        public override void ObjAction()
        {
            base.ObjAction();
            SetAnimation("effect_2", false).Complete += ObjHaiDan_Complete; ;
        }

        private void ObjHaiDan_Complete(Spine.TrackEntry trackEntry)
        {
            
            SetAnimation("effect_1", true);
            gameObject.SetActive(false);
            isCanTouch = true;
            
            
            
            
        }
        

    }
}

