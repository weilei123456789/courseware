using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class GeneralAni : GroundModel
    {
        private GeneralAniData m_GeneralAni;

        private bool m_AniType;
        private string m_AniControl;

        private Animator ani;

        protected override void OnShow(object userData)
        {

            base.OnShow(userData);
            m_GeneralAni = userData as GeneralAniData;
            m_AniType = m_GeneralAni.AniState;
            m_AniControl = m_GeneralAni.ControlName;

             ani = this.GetComponent<Animator>();
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            //base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (CDTime < 0) return;

            if (!m_IsTouch) return;
            m_Time -= Time.deltaTime;
            if (m_Time < 0)
            {
               
                m_IsTouch = false;
                PlayAni(m_AniType);
            }
        }




        protected override void OnHide(object userData)
        {
            base.OnHide(userData);
            PlayAni(m_AniType);
        }




        public override void BeHit()
        {
            Again();
            PlayAni(!m_AniType);
        }

        public void PlayAni(bool Isplay) {
            ani.SetBool(m_AniControl, Isplay);
        }
    
    }
}