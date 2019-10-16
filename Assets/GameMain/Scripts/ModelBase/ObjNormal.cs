using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class ObjNormal : ObjBase
    {
        /// <summary>
        /// 动画之外控制是否可触碰
        /// </summary>
        public bool CanTouch=false;
        public override void ObjAction()
        {
            base.ObjAction();
            SetAnimation("effect_2", true);
            if (gameObject.name== "Tubiao")
            {
                ModelPressEventArgs ne = new ModelPressEventArgs(true);
                GameEntry.Event.Fire(this, ne);
            }
        }
        public override void ObjRestAction()
        {
            base.ObjRestAction();
            SetAnimation("effect_1", true);
        }

        public void SetAction(string name,bool isLoop)
        {
            SetAnimation(name, isLoop);
        }
    }
}

