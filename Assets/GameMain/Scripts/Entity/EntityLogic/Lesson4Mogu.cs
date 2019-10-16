using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class Lesson4Mogu : GroundModel
    {
        private Animator ani;
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            ani = GetComponent<Animator>();
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (m_Time < 0)
            {
                ani.SetInteger("OutIn", 2);

            }
        }

        public override void BeHit()
        {
            Again();
            GameEntry.Sound.PlaySound(20002);
            ani.SetBool("beAtk", true);
            ani.SetInteger("OutIn", 0);
        }

        public void CloseModule()
        {
            // Debug.Log("动画播放完毕~");
            ani.SetBool("beAtk", false);
        }
    }
}